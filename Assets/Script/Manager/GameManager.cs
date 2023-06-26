using System;
using UnityEngine;
using User;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Collections.Generic;
using Loader;

public class GameManager : StaticInstance<GameManager>
{
  public static event Action OnChangeTheme;
  public AppInfoContainer AppInfo;
  public GameSetting GameSettings;
  public GameTheme Theme { get; private set; }
  public AudioManager audioManager;
  public DataManager DataManager;
  public StateManager StateManager;
  public static event Action<GameState> OnBeforeStateChanged;
  public static event Action<GameState> OnAfterStateChanged;

  [HideInInspector] public Words Words;
  public LoadingScreenProvider LoadingScreenProvider { get; private set; }
  public LoginWindowProvider LoginWindowProvider { get; private set; }
  public AssetProvider AssetProvider { get; private set; }
  public GameState State { get; private set; }
  public ResourceSystem ResourceSystem { get; internal set; }

  public LevelManager LevelManager;
  public GamePlayerSetting PlayerSetting;

  public SceneInstance environment;
  public InputManager InputManager;
  public LineManager LineManager;

  void Start()
  {
    // #if UNITY_EDITOR
    //     Debug.unityLogger.logEnabled = true;
    // #else
    //         Debug.unityLogger.logEnabled = false;
    // #endif

    ChangeState(GameState.StartApp);
    InputManager = new();
  }

  public void Init()
  {
    LoadingScreenProvider = new LoadingScreenProvider();
    LoginWindowProvider = new LoginWindowProvider();
    AssetProvider = new AssetProvider();
  }

  public void ChangeState(GameState newState, object Params = null)
  {
    OnBeforeStateChanged?.Invoke(newState);

    State = newState;
    switch (newState)
    {
      case GameState.StartApp:
        HandleStartApp();
        break;
      case GameState.CreateGame:
        HandleCreateGame();
        break;
      case GameState.RunLevel:
        HandleRunLevel();
        break;
      case GameState.CloseLevel:
        HandleCloseLevel();
        break;
      case GameState.StopEffect:
        break;
      case GameState.StartEffect:
        break;
      case GameState.ShowMenu:
        break;
      case GameState.LoadLevel:
        HandleLoadLevel();
        break;
      default:
        // throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        break;
    }

    OnAfterStateChanged?.Invoke(newState);
  }

  private void HandleStartApp()
  {
    Theme = GameSettings.ThemeDefault;

    ChangeTheme();
  }

  private void ChangeTheme()
  {
    Camera.main.backgroundColor = Theme.bgColor;
  }

  private async void HandleRunLevel()
  {
    await AssetProvider.UnloadAdditiveScene(environment);
  }
  private async void HandleCloseLevel()
  {
    await AssetProvider.UnloadAdditiveScene(environment);
  }
  private async void HandleCreateGame()
  {
    var operations = new Queue<ILoadingOperation>();
    operations.Enqueue(new GameInitOperation());
    await LoadingScreenProvider.LoadAndDestroy(operations);
    // LevelManager.CreateLevel();
  }
  private async void HandleLoadLevel()
  {
    var operations = new Queue<ILoadingOperation>();
    operations.Enqueue(new GameInitOperation());
    await LoadingScreenProvider.LoadAndDestroy(operations);
    //var dataGame = DataManager.Load();
    // LevelManager.LoadLevel();
  }


  public void SetTheme(GameTheme newTheme)
  {
    AppInfo.userSettings.theme = newTheme.name;

    Theme = newTheme;

    ChangeTheme();

    DataManager.Save();

    OnChangeTheme?.Invoke();
  }
}

[Serializable]
public enum GameState
{
  StartApp = 0,
  CreateGame = 1,
  StartEffect = 2,
  StopEffect = 3,
  ShowMenu = 4,
  RunLevel = 5,
  CloseLevel = 6,
  LoadLevel = 7,
}