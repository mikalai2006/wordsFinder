using System;
using System.Collections.Generic;
using System.Linq;
using Loader;
using UnityEngine;
using UnityEngine.UIElements;

public class UIStartMenu : UILocaleBase
{
  [SerializeField] private UIDocument _uiDoc;
  [SerializeField] private VisualTreeAsset UserInfoDoc;
  [SerializeField] private VisualTreeAsset LeaderDoc;
  private VisualElement _menu;
  private Button _exitButton;
  private Button _newGameButton;
  private VisualElement _userInfoBlok;
  private VisualElement _leaderList;
  private Button _loadGameMenuButton;
  [SerializeField] private AudioManager _audioManager => GameManager.Instance.audioManager;

  private void Awake()
  {
    UISettings.OnChangeLocale += ChangeLocale;
    GameManager.OnAfterStateChanged += AfterStateChanged;
    LevelManager.OnInitLevel += HideMenu;
    GameManager.OnChangeTheme += ChangeTheme;
  }

  private void OnDestroy()
  {
    UISettings.OnChangeLocale -= ChangeLocale;
    GameManager.OnAfterStateChanged -= AfterStateChanged;
    LevelManager.OnInitLevel -= HideMenu;
    GameManager.OnChangeTheme -= ChangeTheme;
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
    _leaderList = _uiDoc.rootVisualElement.Q<VisualElement>("LeaderList");

    _exitButton = _menu.Q<Button>("ExitBtn");
    _exitButton.clickable.clicked += () =>
    {
      ClickExitButton();
    };

    _newGameButton = _menu.Q<Button>("NewGameBtn");
    _newGameButton.clickable.clicked += () =>
    {
      ClickNewGameButton();
    };

    _loadGameMenuButton = _menu.Q<Button>("LoadGameBtn");
    _loadGameMenuButton.clickable.clicked += () =>
    {
      ClickLoadGameButton();
    };

    ChangeTheme();
  }


  private void ChangeTheme()
  {
    DrawMenu();
    DrawUserInfoBlok();
    DrawLeaderListBlok();

    base.Initialize(_uiDoc.rootVisualElement);
  }


  private void DrawLeaderListBlok()
  {
    var dataState = _gameManager.StateManager.dataGame;
    _leaderList.Clear();

    for (int i = 0; i < 5; i++)
    {
      var blok = LeaderDoc.Instantiate();
      var name = blok.Q<Label>("Name");
      var count = blok.Q<Label>("count");

      // blok.Q<Label>("Ava").style.backgroundImage = new StyleBackground(avaSprite);
      _leaderList.Add(blok);
    }

  }

  private async void DrawUserInfoBlok()
  {
    var dataState = _gameManager.StateManager.dataGame;
    if (string.IsNullOrEmpty(dataState.rank)) return;
    _userInfoBlok.Clear();


    var blok = UserInfoDoc.Instantiate();

    var progress = blok.Q<VisualElement>("ProgressBar");
    progress.style.backgroundColor = new StyleColor(_gameManager.Theme.colorAccent);

    var name = blok.Q<Label>("Name");
    var status = blok.Q<Label>("Status");
    var foundWords = blok.Q<Label>("FoundWords");

    blok.Q<Label>("Rate").text = string.Format("{0}", dataState.rate);
    blok.Q<VisualElement>("RateImg").style.backgroundImage = new StyleBackground(_gameSettings.spriteRate);

    var textCoin = await Helpers.GetLocalizedPluralString(
          "coin",
           new Dictionary<string, object> {
            {"count",  dataState.coins},
          }
        );
    blok.Q<Label>("Coin").text = string.Format("{0} <size=12>{1}</size>", dataState.coins, textCoin);
    var configCoin = _gameManager.ResourceSystem.GetAllEntity().Find(t => t.typeEntity == TypeEntity.Coin);
    blok.Q<VisualElement>("CoinImg").style.backgroundImage = new StyleBackground(configCoin.sprite);


    name.text = string.IsNullOrEmpty(_gameManager.AppInfo.UserInfo.name)
      ? await Helpers.GetLocaledString(_gameSettings.noName.title)
      : _gameManager.AppInfo.UserInfo.name;


    var percentFindWords = (dataState.rate * 100 / _gameManager.PlayerSetting.countFindWordsForUp);
    progress.style.width = new StyleLength(new Length(percentFindWords, LengthUnit.Percent));

    var playerSettings = _gameSettings.PlayerSetting.Find(t => t.idPlayerSetting == dataState.rank);
    status.text = await Helpers.GetLocaledString(playerSettings.text.title);

    var textCountWords = await Helpers.GetLocalizedPluralString(
          "foundwords",
           new Dictionary<string, object> {
            {"count",  dataState.rate},
          }
        );
    foundWords.text = string.Format("{0}", textCountWords);

    _userInfoBlok.Add(blok);
    base.Initialize(_userInfoBlok);
  }

  private void DrawMenu()
  {
    _newGameButton.style.display = DisplayStyle.None;
    _loadGameMenuButton.style.display = DisplayStyle.None;
    _userInfoBlok.style.display = DisplayStyle.None;
    if (_gameManager.StateManager.dataGame.completed.Count == 0 && _gameManager.StateManager.dataGame.levels.Count == 0)
    {
      _newGameButton.style.display = DisplayStyle.Flex;
    }
    else
    // if (_gameManager.StateManager.dataGame.levels.Count != 0 || _gameManager.StateManager.dataGame.sl.Count != 0)
    {
      _loadGameMenuButton.style.display = DisplayStyle.Flex;
      _userInfoBlok.style.display = DisplayStyle.Flex;
    }
  }

  private void HideMenu()
  {
    _menu.style.display = DisplayStyle.None;
  }
  private void ShowMenu()
  {
    DrawMenu();
    DrawUserInfoBlok();
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
    HideMenu();
    var operations = new Queue<ILoadingOperation>();
    operations.Enqueue(new GameInitOperation());
    await _gameManager.LoadingScreenProvider.LoadAndDestroy(operations);

    var firstLevel = _gameSettings.GameLevels.GroupBy(t => t.minRate).First();

    var activeLastLevelWord = firstLevel.ElementAt(0).levelWords.ElementAt(0);

    _gameManager.LevelManager.InitLevel(activeLastLevelWord);
  }

  private void ClickExitButton()
  {
    AudioManager.Instance.Click();
    Debug.Log("ClickExitButton");
  }

  private void ChangeLocale()
  {
    base.Initialize(_uiDoc.rootVisualElement);
  }
}
