using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using User;

public class DataManager : Singleton<DataManager>
{
  [DllImport("__Internal")]
  private static extern void SaveExtern(string data);
  [DllImport("__Internal")]
  private static extern void LoadExtern();
  [DllImport("__Internal")]
  private static extern void GetUserInfo();

  public static event System.Action<StateGame> OnLoadData;
  public static event System.Action<UserInfo> OnLoadUserInfo;
  public static event System.Action<LeaderBoard> OnLoadLeaderBoard;

  [Header("File Storage Config")]
  [SerializeField] private string fileNameGame;
  [SerializeField] private string fileNameMap;
  [SerializeField] private bool useEncryption;
  private FileDataHandler _fileDataHandler;
  private CancellationTokenSource cancelTokenSource;
  private GameManager _gameManager => GameManager.Instance;
  [SerializeField] private StateGame _stateGame;
  public StateGame StateGame { get { return _stateGame; } }


  public void Init(string idDevice)
  {
    cancelTokenSource = new CancellationTokenSource();

    _fileDataHandler = new FileDataHandler(Application.persistentDataPath, idDevice, useEncryption);
  }

  public async UniTask<StateGame> Load()
  {
    _stateGame = await _fileDataHandler.LoadData();
    // Debug.Log($"{name}::: JSON ::: Load {JsonUtility.ToJson(_stateGame)}");

    OnLoadData?.Invoke(_stateGame);
    return _stateGame;
  }


  public void LoadAsYsdk()
  {
    LoadExtern();
  }

  public StateGame SetPlayerData(string data)
  {
    _stateGame = JsonUtility.FromJson<StateGame>(data);
    // Debug.Log($"{name}::: YSDK ::: LoadPlayerData {JsonUtility.ToJson(_stateGame)}");

    OnLoadData?.Invoke(_stateGame);
    return _stateGame;
  }

  public void LoadUserInfoAsYsdk()
  {
    GetUserInfo();
  }

  public void SetUserInfo(string stringUserInfo)
  {
    UserInfo userInfo = JsonUtility.FromJson<UserInfo>(stringUserInfo);
    // Debug.Log($"{name}::: YSDK ::: SetUserInfo {stringUserInfo}");

    OnLoadUserInfo?.Invoke(userInfo);
  }

  public void GetLeaderBoard(string stringLeaderBoard)
  {
    LeaderBoard leaderBoard = JsonUtility.FromJson<LeaderBoard>(stringLeaderBoard);
    // Debug.Log($"{name}::: YSDK ::: GetLeaderBoard {stringLeaderBoard}");

    OnLoadLeaderBoard?.Invoke(leaderBoard);
  }

  public void Save()
  {
    if (!cancelTokenSource.Token.IsCancellationRequested)
    {
      cancelTokenSource.Cancel();
      cancelTokenSource.Dispose();
    }
    cancelTokenSource = new CancellationTokenSource();
    Saved(cancelTokenSource.Token).Forget();
  }

  private async UniTask Saved(CancellationToken cancellationToken)
  {
    await UniTask.Delay(_gameManager.GameSettings.debounceTime);

    if (!cancellationToken.IsCancellationRequested)
    {
      var levelManager = _gameManager.LevelManager;

      _stateGame = _gameManager.StateManager.GetData(); //.dataGame;

#if android
      _fileDataHandler.SaveData(_stateGame);
#endif


      string jsonString = JsonUtility.ToJson(_stateGame);
#if ysdk
      SaveExtern(jsonString);
#endif
      Debug.Log("Saved complete successfully!");
    }
  }

  public void SaveSettings()
  {
    string appInfo = JsonUtility.ToJson(_gameManager.AppInfo);
    PlayerPrefs.SetString(_gameManager.namePlayPref, appInfo);
    // Debug.Log($"SaveSettings::: appInfo=[{appInfo}");
  }
}
