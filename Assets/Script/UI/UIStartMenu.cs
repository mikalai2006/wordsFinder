using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Loader;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

public class UIStartMenu : UILocaleBase
{
  [DllImport("__Internal")]
  private static extern void GetLeaderBoard();
  [SerializeField] private UIDocument _uiDoc;
  [SerializeField] private VisualTreeAsset UserInfoDoc;
  [SerializeField] private VisualTreeAsset LeaderDoc;
  private VisualElement _menu;
  private Button _exitButton;
  // private Button _newGameButton;
  private VisualElement _userInfoBlok;
  private VisualElement _leaderBlock;
  // private Button _loadGameMenuButton;
  [SerializeField] private AudioManager _audioManager => GameManager.Instance.audioManager;

  private void Awake()
  {
    UISettings.OnChangeLocale += RefreshMenu;
    GameManager.OnAfterStateChanged += AfterStateChanged;
    LevelManager.OnInitLevel += HideMenu;
    GameManager.OnChangeTheme += RefreshMenu;
  }

  private void OnDestroy()
  {
    UISettings.OnChangeLocale -= RefreshMenu;
    GameManager.OnAfterStateChanged -= AfterStateChanged;
    LevelManager.OnInitLevel -= HideMenu;
    GameManager.OnChangeTheme -= RefreshMenu;
  }

  private void AfterStateChanged(GameState state)
  {
    switch (state)
    {
      case GameState.ShowMenu:
      case GameState.CloseLevel:
        ShowMenu();
        break;
    }
  }

  public virtual void Start()
  {
    _menu = _uiDoc.rootVisualElement.Q<VisualElement>("MenuBlok");
    _userInfoBlok = _uiDoc.rootVisualElement.Q<VisualElement>("UserInfoBlok");

    _leaderBlock = _uiDoc.rootVisualElement.Q<VisualElement>("LeaderBoard");
    _leaderBlock.style.display = DisplayStyle.None;

    _exitButton = _menu.Q<Button>("ExitBtn");
    _exitButton.clickable.clicked += () =>
    {
      ClickExitButton();
    };

    // DrawLeaderListBlok();

#if ysdk
    GetLeaderBoard();
#endif
    RefreshMenu();

    var diffDate = string.IsNullOrEmpty(_gameManager.StateManager.stateGame.lastTime)
      ? (DateTime.Now - DateTime.Now)
      : (DateTime.Now - DateTime.Parse(_gameManager.StateManager.stateGame.lastTime));
    // Debug.Log($"diffDate={diffDate}|||[{DateTime.Now}:::::{_gameManager.StateManager.stateGame.lastTime}]");
    if (string.IsNullOrEmpty(_gameManager.StateManager.stateGame.lastTime) || diffDate.TotalHours > _gameSettings.countHoursDailyGift)
    {
      ShowDailyDialog();
    }

  }

  private async void ShowDailyDialog()
  {
    var title = await Helpers.GetLocaledString("dailycoins_t");
    var message = await Helpers.GetLocalizedPluralString("dailycoins_d", new Dictionary<string, object>() {
      { "count", _gameSettings.countHoursDailyGift },
    });
    var configCoin = _gameManager.ResourceSystem.GetAllEntity().Find(t => t.typeEntity == TypeEntity.Coin);
    var configStar = _gameManager.ResourceSystem.GetAllEntity().Find(t => t.typeEntity == TypeEntity.Star);
    var entities = new List<ShopItem<GameEntity>>() {
          new ShopItem<GameEntity>(){
            entity = configCoin,
            cost = 0,
            count = 500
          },
          new ShopItem<GameEntity>(){
            entity = configStar,
            cost = 0,
            count = 3
          }
        };

    var dialog = new DialogProvider(new DataDialog()
    {
      title = title,
      message = message,
      entities = entities,
      showCancelButton = false,
    });

    _gameManager.InputManager.Disable();
    var result = await dialog.ShowAndHide();
    if (result.isOk)
    {
      foreach (var entityItem in entities)
      {
        if (entityItem.entity.typeEntity == TypeEntity.Coin)
        {
          _gameManager.StateManager.IncrementTotalCoin(entityItem.count);
        }
        else
        {
          _gameManager.StateManager.BuyHint(entityItem);
        }
      }
      _gameManager.StateManager.SetLastTime();
    }
    _gameManager.InputManager.Enable();
  }

  private void RefreshMenu()
  {
    Debug.Log("RefreshMenu");

    DrawUserInfoBlok();

    // DrawMenu();

    base.Initialize(_uiDoc.rootVisualElement);
  }


  private async void DrawLeaderListBlok(string stringLeaderBoard)
  {
    LeaderBoard leaderBoard = JsonUtility.FromJson<LeaderBoard>(stringLeaderBoard);

    var dataState = _gameManager.StateManager.dataGame;
    var _leaderBlock = _uiDoc.rootVisualElement.Q<VisualElement>("LeaderList");
    _leaderBlock.Clear();

    if (leaderBoard.entries.Count == 0)
    {
      _leaderBlock.style.display = DisplayStyle.None;
    }
    else
    {
      _leaderBlock.style.display = DisplayStyle.Flex;

    }

    for (int i = 0; i < leaderBoard.entries.Count; i++)
    {
      var blok = LeaderDoc.Instantiate();

      var leader = leaderBoard.entries[i];

      var rank = blok.Q<Label>("Rank");
      rank.text = leader.rank.ToString();

      var name = blok.Q<Label>("Name");
      name.text = leader.name;

      var ava = blok.Q<Label>("Ava");
      ava.style.backgroundImage = await Helpers.LoadTexture(leader.photo);

      var score = blok.Q<Label>("Score");
      score.text = leader.score.ToString();

      // blok.Q<Label>("Ava").style.backgroundImage = new StyleBackground(avaSprite);
      _leaderBlock.Add(blok);
    }

  }

  private async void DrawUserInfoBlok()
  {
    _userInfoBlok.Clear();

    var stateGame = _gameManager.StateManager.stateGame;
    var dataGame = _gameManager.StateManager.dataGame;
    if (string.IsNullOrEmpty(dataGame.rank)) return;


    var blok = UserInfoDoc.Instantiate();

    var progress = blok.Q<VisualElement>("ProgressBar");
    progress.style.backgroundColor = new StyleColor(_gameManager.Theme.colorAccent);

    var name = blok.Q<Label>("Name");
    var status = blok.Q<Label>("Status");
    var foundWords = blok.Q<Label>("FoundWords");

    // blok.Q<Label>("Rate").text = string.Format("{0}", stateGame.rate);
    // blok.Q<VisualElement>("RateImg").style.backgroundImage = new StyleBackground(_gameSettings.spriteRate);

    // var textCoin = await Helpers.GetLocalizedPluralString(
    //       "coin",
    //        new Dictionary<string, object> {
    //         {"count",  stateGame.coins},
    //       }
    //     );
    // blok.Q<Label>("Coin").text = string.Format("{0} <size=12>{1}</size>", dataGame.activeLevel.coins, textCoin);
    // var configCoin = _gameManager.ResourceSystem.GetAllEntity().Find(t => t.typeEntity == TypeEntity.Coin);
    // blok.Q<VisualElement>("CoinImg").style.backgroundImage = new StyleBackground(configCoin.sprite);


    var titleFile = _gameManager.GameSettings.wordFiles.Find(t => t.locale.Identifier.Code == _gameManager.AppInfo.setting.lang);
    name.text = await Helpers.GetLocaledString(titleFile.text.title);

    var percentFindWords = (dataGame.rate * 100 / _gameManager.PlayerSetting.countFindWordsForUp);
    progress.style.width = new StyleLength(new Length(percentFindWords, LengthUnit.Percent));

    var playerSettings = _gameSettings.PlayerSetting.Find(t => t.idPlayerSetting == dataGame.rank);
    status.text = await Helpers.GetLocaledString(playerSettings.text.title);

    var textCountWords = await Helpers.GetLocalizedPluralString(
          "foundwords",
           new Dictionary<string, object> {
            {"count",  dataGame.rate},
          }
        );
    foundWords.text = string.Format("{0}", textCountWords);


    var _newGameButton = blok.Q<Button>("NewGameBtn");
    _newGameButton.clickable.clicked += () =>
    {
      ClickNewGameButton();
    };
    _newGameButton.style.display = DisplayStyle.None;

    var _loadGameMenuButton = blok.Q<Button>("LoadGameBtn");
    _loadGameMenuButton.clickable.clicked += () =>
    {
      ClickLoadGameButton();
    };
    _loadGameMenuButton.style.display = DisplayStyle.None;

    // _userInfoBlok.style.display = DisplayStyle.None;
    if (_gameManager.StateManager.dataGame.completed.Count == 0 && _gameManager.StateManager.dataGame.levels.Count == 0)
    {
      _newGameButton.style.display = DisplayStyle.Flex;
    }
    else
    // if (_gameManager.StateManager.dataGame.levels.Count != 0 || _gameManager.StateManager.dataGame.sl.Count != 0)
    {
      _loadGameMenuButton.style.display = DisplayStyle.Flex;
      // _userInfoBlok.style.display = DisplayStyle.Flex;
    }

    _userInfoBlok.Add(blok);
    //base.Initialize(_userInfoBlok);
  }

  // private void DrawMenu()
  // {
  //   var _newGameButton = _userInfoBlok.Q<Button>("NewGameBtn");
  //   _newGameButton.clickable.clicked += () =>
  //   {
  //     ClickNewGameButton();
  //   };
  //   _newGameButton.style.display = DisplayStyle.None;

  //   var _loadGameMenuButton = _userInfoBlok.Q<Button>("LoadGameBtn");
  //   _loadGameMenuButton.clickable.clicked += () =>
  //   {
  //     ClickLoadGameButton();
  //   };
  //   _loadGameMenuButton.style.display = DisplayStyle.None;

  //   // _userInfoBlok.style.display = DisplayStyle.None;
  //   if (_gameManager.StateManager.dataGame.completed.Count == 0 && _gameManager.StateManager.dataGame.levels.Count == 0)
  //   {
  //     _newGameButton.style.display = DisplayStyle.Flex;
  //   }
  //   else
  //   // if (_gameManager.StateManager.dataGame.levels.Count != 0 || _gameManager.StateManager.dataGame.sl.Count != 0)
  //   {
  //     _loadGameMenuButton.style.display = DisplayStyle.Flex;
  //     // _userInfoBlok.style.display = DisplayStyle.Flex;
  //   }
  // }

  private void HideMenu()
  {
    _menu.style.display = DisplayStyle.None;
  }
  private void ShowMenu()
  {
    RefreshMenu();
    _menu.style.display = DisplayStyle.Flex;
  }

  private async void ClickLoadGameButton()
  {
    AudioManager.Instance.Click();
    HideMenu();
    var operations = new Queue<ILoadingOperation>();
    operations.Enqueue(new GameInitOperation());
    await _gameManager.LoadingScreenProvider.LoadAndDestroy(operations);

    var activeLastWord = _gameManager.StateManager.dataGame.lastWord;
    // _gameSettings.GameLevels.levelWords
    //   .Find(t => t.name == _gameManager.DataManager.DataGame.lastLevelWord);
    // // var activeLastLevelWord = activeLastWord
    // //   .words
    // //   .Find(t => t == _gameManager.DataManager.DataGame.lastWord);

    _gameManager.LevelManager.InitLevel(activeLastWord);

    // var dialogWindow = new UILevelsOperation();
    // var result = await dialogWindow.ShowAndHide();
  }

  private async void ClickNewGameButton()
  {
    AudioManager.Instance.Click();
    await LocalizationSettings.InitializationOperation.Task;

    HideMenu();
    var operations = new Queue<ILoadingOperation>();
    operations.Enqueue(new GameInitOperation());
    await _gameManager.LoadingScreenProvider.LoadAndDestroy(operations);

    GameLevel firstLevel = _gameSettings.GameLevels.Where(t => t.locale.Identifier.Code == LocalizationSettings.SelectedLocale.Identifier.Code).OrderBy(t => t.minRate).First();
    Debug.Log($"firstLevel={firstLevel.name} for locale {LocalizationSettings.SelectedLocale.name}");
    var activeLastLevelWord = firstLevel.levelWords.ElementAt(0);

    _gameManager.LevelManager.InitLevel(activeLastLevelWord);
  }

  private void ClickExitButton()
  {
    AudioManager.Instance.Click();
    Debug.Log("ClickExitButton");
  }

}
