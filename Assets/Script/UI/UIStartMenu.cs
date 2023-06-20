using System.Collections.Generic;
using System.Linq;
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

    DrawMenu();

    base.Localize(_uiDoc.rootVisualElement);
  }

  private void DrawMenu()
  {
    _newGameButton.style.display = DisplayStyle.None;
    _loadGameMenuButton.style.display = DisplayStyle.None;
    if (_gameManager.StateManager.dataGame.completeWords.Count == 0 && _gameManager.StateManager.dataGame.levels.Count == 0)
    {
      _newGameButton.style.display = DisplayStyle.Flex;
    }
    else
    // if (_gameManager.StateManager.dataGame.levels.Count != 0 || _gameManager.StateManager.dataGame.sl.Count != 0)
    {
      _loadGameMenuButton.style.display = DisplayStyle.Flex;
    }
  }

  private void HideMenu()
  {
    _menu.style.display = DisplayStyle.None;
  }
  private void ShowMenu()
  {
    DrawMenu();
    _menu.style.display = DisplayStyle.Flex;
  }

  private async void ClickLoadGameButton()
  {
    HideMenu();
    var operations = new Queue<ILoadingOperation>();
    operations.Enqueue(new GameInitOperation());
    await _gameManager.LoadingScreenProvider.LoadAndDestroy(operations);

    var activeLastLevel = _gameSettings.GameLevels
      .Find(t => t.idLevel == _gameManager.DataManager.DataGame.lastLevel);
    var activeLastLevelWord = activeLastLevel
      .words
      .Find(t => t.idLevelWord == _gameManager.DataManager.DataGame.lastWord);

    _gameManager.LevelManager.InitLevel(activeLastLevel, activeLastLevelWord);

    // var dialogWindow = new UILevelsOperation();
    // var result = await dialogWindow.ShowAndHide();
  }

  private async void ClickNewGameButton()
  {
    HideMenu();
    var operations = new Queue<ILoadingOperation>();
    operations.Enqueue(new GameInitOperation());
    await _gameManager.LoadingScreenProvider.LoadAndDestroy(operations);

    var activeLastLevel = _gameSettings.GameLevels.ElementAt(0);
    var activeLastLevelWord = activeLastLevel.words.ElementAt(0); ;

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
