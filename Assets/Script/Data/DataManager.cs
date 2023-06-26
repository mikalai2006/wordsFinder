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

  // public static event Action<DataGame> OnLoadData;
  [Header("File Storage Config")]
  [SerializeField] private string fileNameGame;
  [SerializeField] private string fileNameMap;
  [SerializeField] private bool useEncryption;
  private FileDataHandler _fileDataHandler;
  private CancellationTokenSource cancelTokenSource;
  private GameManager _gameManager => GameManager.Instance;
  private DataGame _dataGame;
  public DataGame DataGame { get { return _dataGame; } }

  public void Init()
  {
    cancelTokenSource = new CancellationTokenSource();

    _fileDataHandler = new FileDataHandler(Application.persistentDataPath, _gameManager.AppInfo.DeviceId, useEncryption);
  }

  public async UniTask<DataGame> Load()
  {
    _dataGame = await _fileDataHandler.LoadData();
    Debug.Log($"{name}::: Load {JsonUtility.ToJson(_dataGame)}");
    // OnLoadData?.Invoke(_dataGame);
    return _dataGame;
  }


  public void LoadPlayerData(string data)
  {
    Debug.Log($"{name}::: LoadPlayerData {data}");
    _dataGame = JsonUtility.FromJson<DataGame>(data);
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

      _fileDataHandler.SaveData(_dataGame);

      string jsonString = JsonUtility.ToJson(_dataGame);

      Debug.Log("Saved complete successfully!");
    }
  }
}
