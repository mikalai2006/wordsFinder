using System;
using UnityEngine;
using User;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Collections.Generic;
using Loader;

public class GameManager : StaticInstance<GameManager>
{
  public AppInfoContainer AppInfo;
  public GameSetting GameSettings;
  public AudioManager audioManager;
  public DataManager DataManager;
  public static event Action<GameState> OnBeforeStateChanged;
  public static event Action<GameState> OnAfterStateChanged;

  [HideInInspector] public Words Words;
  public LoadingScreenProvider LoadingScreenProvider { get; private set; }
  public LoginWindowProvider LoginWindowProvider { get; private set; }
  public AssetProvider AssetProvider { get; private set; }
  public GameState State { get; private set; }
  public LevelManager LevelManager { get; internal set; }

  public SceneInstance environment;

  void Start()
  {
    // #if UNITY_EDITOR
    //     Debug.unityLogger.logEnabled = true;
    // #else
    //         Debug.unityLogger.logEnabled = false;
    // #endif

    ChangeState(GameState.StartApp);
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
      case GameState.NoWord:
        break;
      case GameState.YesWord:
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

  private async void HandleRunLevel()
  {
    await GameManager.Instance.AssetProvider.UnloadAdditiveScene(environment);
  }
  private async void HandleCloseLevel()
  {
    await GameManager.Instance.AssetProvider.UnloadAdditiveScene(environment);
  }
  private async void HandleCreateGame()
  {
    var operations = new Queue<ILoadingOperation>();
    operations.Enqueue(new GameInitOperation());
    await GameManager.Instance.LoadingScreenProvider.LoadAndDestroy(operations);
    GameManager.Instance.LevelManager.CreateLevel();
  }
  private async void HandleLoadLevel()
  {
    var operations = new Queue<ILoadingOperation>();
    operations.Enqueue(new GameInitOperation());
    await GameManager.Instance.LoadingScreenProvider.LoadAndDestroy(operations);
    GameManager.Instance.DataManager.Load();
    GameManager.Instance.LevelManager.LoadLevel();
  }
}

[Serializable]
public enum GameState
{
  StartApp = 0,
  CreateGame = 1,
  YesWord = 2,
  NoWord = 3,
  ShowMenu = 4,
  RunLevel = 5,
  CloseLevel = 6,
  LoadLevel = 7,
}