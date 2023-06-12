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
  }

  private void OnDestroy()
  {
    UISettings.OnChangeLocale -= ChangeLocale;
    GameManager.OnAfterStateChanged -= AfterStateChanged;
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

    GameSetting = GameManager.Instance.GameSetting;

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

  private void ClickLoadGameButton()
  {
    _menu.style.display = DisplayStyle.None;
    GameManager.Instance.ChangeState(GameState.LoadLevel);
  }

  private void HideMenu()
  {
    _menu.style.display = DisplayStyle.None;
  }
  private void ShowMenu()
  {
    _menu.style.display = DisplayStyle.Flex;
  }

  private void ClickNewGameButton()
  {
    _menu.style.display = DisplayStyle.None;
    GameManager.Instance.ChangeState(GameState.CreateGame);
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
