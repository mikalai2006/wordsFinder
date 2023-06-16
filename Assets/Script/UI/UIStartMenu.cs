using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Loader;
using UnityEngine;
using UnityEngine.UIElements;

public class UIStartMenu : UILocaleBase
{
  [SerializeField] private UIDocument _uiDoc;
  private VisualElement _menu;
  private Button _exitButton;
  private Button _newGameButton;
  private Button _loadGameMenuButton;
  private GameSetting GameSetting;
  [SerializeField] private AudioManager _audioManager => GameManager.Instance.audioManager;

  private void Awake()
  {
    UISettings.OnChangeLocale += ChangeLocale;
    GameManager.OnAfterStateChanged += AfterStateChanged;
    LevelManager.OnInitLevel += HideMenu;
  }

  private void OnDestroy()
  {
    UISettings.OnChangeLocale -= ChangeLocale;
    GameManager.OnAfterStateChanged -= AfterStateChanged;
    LevelManager.OnInitLevel -= HideMenu;
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

    GameSetting = GameManager.Instance.GameSettings;

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

    base.Localize(_uiDoc.rootVisualElement);
  }

  private async void ClickLoadGameButton()
  {
    // HideMenu();
    // GameManager.Instance.ChangeState(GameState.LoadLevel);

    var dialogWindow = new UILevelsOperation();
    var result = await dialogWindow.ShowAndHide();
  }

  private void HideMenu()
  {
    _menu.style.display = DisplayStyle.None;
  }
  private void ShowMenu()
  {
    _menu.style.display = DisplayStyle.Flex;
  }

  private async void ClickNewGameButton()
  {
    _menu.style.display = DisplayStyle.None;
    // GameManager.Instance.ChangeState(GameState.CreateGame);
    var operations = new Queue<ILoadingOperation>();
    operations.Enqueue(new GameInitOperation());
    await _gameManager.LoadingScreenProvider.LoadAndDestroy(operations);

    var activeLastLevel = _gameSettings.GameLevels
      .Find(t => t.id == _gameManager.DataManager.DataGame.lastActiveLevelId);
    var activeLastLevelWord = activeLastLevel
      .words
      .Find(t => t.id == _gameManager.DataManager.DataGame.lastActiveWordId);

    _gameManager.LevelManager.InitLevel(activeLastLevel, activeLastLevelWord);
  }

  private void ClickExitButton()
  {
    Debug.Log("ClickExitButton");
  }

  private void ChangeLocale()
  {
    base.Localize(_uiDoc.rootVisualElement);
  }

}
