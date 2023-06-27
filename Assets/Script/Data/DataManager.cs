using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
  [DllImport("__Internal")]
  private static extern void SaveExtern(string data);
  [DllImport("__Internal")]
  private static extern void LoadExtern();
  [DllImport("__Internal")]
  private static extern void GetName();
  [DllImport("__Internal")]
  private static extern void GetPhoto();

  public static event System.Action<DataGame> OnLoadData;
  public static event System.Action<string> OnLoadName;
  public static event System.Action<string> OnLoadPhoto;
  [Header("File Storage Config")]
  [SerializeField] private string fileNameGame;
  [SerializeField] private string fileNameMap;
  [SerializeField] private bool useEncryption;
  private FileDataHandler _fileDataHandler;
  private CancellationTokenSource cancelTokenSource;
  private GameManager _gameManager => GameManager.Instance;
  private DataGame _dataGame;
  public DataGame DataGame { get { return _dataGame; } }


  public void Init(string idDevice)
  {
    cancelTokenSource = new CancellationTokenSource();

    _fileDataHandler = new FileDataHandler(Application.persistentDataPath, idDevice, useEncryption);
  }

  public async UniTask<DataGame> Load()
  {
    _dataGame = await _fileDataHandler.LoadData();
    Debug.Log($"{name}::: JSON ::: Load {JsonUtility.ToJson(_dataGame)}");

    OnLoadData?.Invoke(_dataGame);
    return _dataGame;
  }


  public void LoadAsYsdk()
  {
    LoadExtern();
  }

  public DataGame SetPlayerData(string data)
  {
    _dataGame = JsonUtility.FromJson<DataGame>(data);
    Debug.Log($"{name}::: YSDK ::: LoadPlayerData {JsonUtility.ToJson(_dataGame)}");

    OnLoadData?.Invoke(_dataGame);
    return _dataGame;
  }

  public void LoadNameAsYsdk()
  {
    GetName();
  }

  public void SetName(string name)
  {
    Debug.Log($"{name}::: YSDK ::: SetName {name}");

    OnLoadName?.Invoke(name);
  }

  public void LoadPhotoAsYsdk()
  {
    GetPhoto();
  }

  public void SetPhoto(string photo)
  {
    Debug.Log($"{name}::: YSDK ::: SetPhoto {photo}");

    OnLoadPhoto?.Invoke(photo);
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

      _dataGame = _gameManager.StateManager.GetData(); //.dataGame;

#if android
      _fileDataHandler.SaveData(_dataGame);
#endif

#if ysdk && !UNITY_EDITOR
      string jsonString = JsonUtility.ToJson(_dataGame);
      SaveExtern(jsonString);
#endif


      Debug.Log("Saved complete successfully!");
    }
  }
}
