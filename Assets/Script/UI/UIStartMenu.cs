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
  private VisualElement _leaderBoard;
  // private Button _loadGameMenuButton;
  [SerializeField] private AudioManager _audioManager => GameManager.Instance.audioManager;

  private void Awake()
  {
    UISettings.OnChangeLocale += RefreshMenu;
    GameManager.OnAfterStateChanged += AfterStateChanged;
    LevelManager.OnInitLevel += HideMenu;
    GameManager.OnChangeTheme += RefreshMenu;
    DataManager.OnLoadLeaderBoard += DrawLeaderListBlok;
    StateManager.OnChangeState += SetValue;
  }

  private void OnDestroy()
  {
    UISettings.OnChangeLocale -= RefreshMenu;
    GameManager.OnAfterStateChanged -= AfterStateChanged;
    LevelManager.OnInitLevel -= HideMenu;
    GameManager.OnChangeTheme -= RefreshMenu;
    DataManager.OnLoadLeaderBoard -= DrawLeaderListBlok;
    StateManager.OnChangeState -= SetValue;
  }

  private void SetValue(StateGame state)
  {
    _userInfoBlok.Q<Label>("UserCoin").text = string.Format("{0}", state.coins);
    _userInfoBlok.Q<Label>("UserRate").text = string.Format("{0}", state.rate);
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

    _leaderBoard = _uiDoc.rootVisualElement.Q<VisualElement>("LeaderBoard");
    _leaderBoard.style.display = DisplayStyle.None;

    _exitButton = _menu.Q<Button>("ExitBtn");
    _exitButton.clickable.clicked += () =>
    {
      ClickExitButton();
    };

    // DrawLeaderListBlok();

#if ysdk
    GetLeaderBoard();
#endif
    // _gameManager.DataManager.GetLeaderBoard("{\"leaderboard\":{\"title\":[{\"lang\":\"ru\",\"value\":\"Лидеры по количеству слов\"}]},\"userRank\":1,\"entries\":[{\"rank\":1,\"score\":90,\"name\":\"Mikalai P.\",\"lang\":\"ru\",\"photo\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/66VOVRVF2GJAXS5VWT3X54YATTEZAJLGXTPIXJTG3465T5HXLNQFMZIOJ7WYALX2PEC2DIAHLM6FC7ABRLOA27IRF55DP6DXJU7JDS4IFW63KJWT4IFLT2I26N44GVCAAX6FGHPPVKQY65KZZOXXYODUUKJMK2Y25M2VUDFYRPJDR3TS4JVBUOZNWFE2QNABMFRQEVLJRRIODYNB2JKIIK76YMZEEA3VQHV3M6Q=/islands-retina-medium\"}]}");

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
    DrawUserInfoBlok();

    // DrawMenu();

    base.Initialize(_uiDoc.rootVisualElement);
  }


  private async void DrawLeaderListBlok(LeaderBoard leaderBoard)
  {
    if (leaderBoard.entries.Count == 0)
    {
      _leaderBoard.style.display = DisplayStyle.None;
      return;
    }
    else
    {
      _leaderBoard.style.display = DisplayStyle.Flex;
    }

    await LocalizationSettings.InitializationOperation.Task;

    var dataState = _gameManager.StateManager.dataGame;
    LeaderBoardInfoTitle titleBoard = leaderBoard.leaderboard.title.Find((t) => t.lang == LocalizationSettings.SelectedLocale.Identifier.Code);
    if (string.IsNullOrEmpty(titleBoard.value))
    {
      _leaderBoard.Q<Label>("NameBoard").text = titleBoard.value;
    }
    var _leaderList = _leaderBoard.Q<VisualElement>("LeaderList");
    _leaderList.Clear();


    for (int i = 0; i < leaderBoard.entries.Count; i++)
    {
      var blok = LeaderDoc.Instantiate();

      var leader = leaderBoard.entries[i];

      var rank = blok.Q<Label>("Rank");
      rank.text = leader.rank.ToString();

      var name = blok.Q<Label>("Name");
      name.text = leader.name;

      var ava = blok.Q<VisualElement>("Ava");
      Texture2D avatarTexture = await Helpers.LoadTexture(leader.photo);
      if (avatarTexture != null)
      {
        ava.style.backgroundImage = new StyleBackground(avatarTexture);
      }
      else
      {
        // ava.style.display = DisplayStyle.None;
      }

      var score = blok.Q<Label>("Score");
      score.text = leader.score.ToString();

      // blok.Q<Label>("Ava").style.backgroundImage = new StyleBackground(avaSprite);
      _leaderList.Add(blok);
    }


  }

  private async void DrawUserInfoBlok()
  {
    await LocalizationSettings.InitializationOperation.Task;
    _userInfoBlok.Clear();

    var stateGame = _gameManager.StateManager.stateGame;
    var dataGame = _gameManager.StateManager.dataGame;
    if (string.IsNullOrEmpty(dataGame.rank)) return;


    var blok = UserInfoDoc.Instantiate();

    var progress = blok.Q<VisualElement>("ProgressBar");
    progress.style.backgroundColor = new StyleColor(_gameManager.Theme.colorAccent);

    var nameFile = blok.Q<Label>("NameFile");
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


    var titleFile = _gameManager.GameSettings.wordFiles.Find(t => t.locale.Identifier.Code == LocalizationSettings.SelectedLocale.Identifier.Code);
    nameFile.text = await Helpers.GetLocaledString(titleFile.text.title);

    var percentFindWords = (dataGame.rate * 100 / _gameManager.PlayerSetting.countFindWordsForUp);
    progress.style.width = new StyleLength(new Length(percentFindWords, LengthUnit.Percent));

    var playerSettings = _gameSettings.PlayerSetting.Find(t => t.idPlayerSetting == dataGame.rank);
    status.text = await Helpers.GetLocaledString(playerSettings.text.title);

    var textCountWords = await Helpers.GetLocalizedPluralString(
          "foundwords",
           new Dictionary<string, object> {
            {"count",  dataGame.rate},
            {"count2",  playerSettings.countFindWordsForUp},
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


    // Set short info user.
    var configCoin = _gameManager.ResourceSystem.GetAllEntity().Find(t => t.typeEntity == TypeEntity.Coin);

    var userCoin = blok.Q<Label>("UserCoin");
    userCoin.text = _gameManager.StateManager.stateGame.coins.ToString();
    var userCoinImg = blok.Q<VisualElement>("UserCoinImg");
    userCoinImg.style.backgroundImage = new StyleBackground(configCoin.sprite);

    var userRate = blok.Q<Label>("UserRate");
    userRate.text = _gameManager.StateManager.stateGame.rate.ToString();
    var userRateImg = blok.Q<VisualElement>("UserRateImg");
    userRateImg.style.backgroundImage = new StyleBackground(_gameSettings.spriteRate);

    var userName = blok.Q<Label>("UserName");
    userName.text = await Helpers.GetName();

    userCoinImg.style.unityBackgroundImageTintColor = new StyleColor(_gameManager.Theme.colorSecondary);
    userRateImg.style.unityBackgroundImageTintColor = new StyleColor(_gameManager.Theme.colorSecondary);

    // load avatar
    string placeholder = _gameManager.AppInfo.UserInfo.photo;
#if UNITY_EDITOR
    placeholder = "https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/CGIB4J3KPRTV5JCX6JLC6TAKLL6GYPN27SBYCQDUWEUW2QFCBB5ZNTCHKPVHHHKLTSBRYYIEMFB2C3CB37T4S7GIXKUP4KEL4CHGRPVOLHVSPIW77Z5TUSEOVQK5NDDSPJTVMAJAODQ4DXD6UEKJV4VLEUOPWOPU2Y664NQ5NIQUT2UBNRMVVWCQN52FYLVEI4DWLSZQ4FG6AZWBGKYTD5VJWXXAL46Z7B5XDCI=/islands-retina-medium";
#endif
    var imgAva = blok.Q<VisualElement>("Ava");
    Texture2D avatarTexture = await Helpers.LoadTexture(placeholder);
    if (avatarTexture != null)
    {
      imgAva.style.backgroundImage = new StyleBackground(avatarTexture);
    }
    else
    {
      imgAva.style.display = DisplayStyle.None;
    }

    _userInfoBlok.Add(blok);
    base.Initialize(_userInfoBlok);
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
    _gameManager.ChangeState(GameState.LoadLevel);
  }

  private async void ClickNewGameButton()
  {
    AudioManager.Instance.Click();
    await LocalizationSettings.InitializationOperation.Task;

    HideMenu();
    var operations = new Queue<ILoadingOperation>();
    operations.Enqueue(new GameInitOperation());
    await _gameManager.LoadingScreenProvider.LoadAndDestroy(operations);

    GameLevel firstLevel = _gameSettings.GameLevels.Where(t => t.locale.Identifier.Code == LocalizationSettings.SelectedLocale.Identifier.Code).OrderBy(t => t.minRate).FirstOrDefault();
    string activeLastLevelWord = "";
    if (firstLevel != null)
    {
      Debug.Log($"firstLevel={firstLevel.name} for locale {LocalizationSettings.SelectedLocale.name}");
      activeLastLevelWord = firstLevel.levelWords.ElementAt(0);
    }

    _gameManager.LevelManager.InitLevel(activeLastLevelWord);
    _gameManager.ChangeState(GameState.LoadLevel);
  }

  private void ClickExitButton()
  {
    AudioManager.Instance.Click();
    Debug.Log("ClickExitButton");
  }

}
