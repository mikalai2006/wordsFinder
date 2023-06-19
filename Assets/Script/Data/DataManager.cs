using System;
using System.Threading;
using Cysharp.Threading.Tasks;
// using System.Linq;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
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

    _fileDataHandler = new FileDataHandler(Application.persistentDataPath, _gameManager.AppInfo.UserInfo.DeviceId, useEncryption);
  }

  public DataGame Load()
  {
    _dataGame = _fileDataHandler.LoadData();
    // OnLoadData?.Invoke(_dataGame);
    return _dataGame;
  }

  public void Save()
  {
    if (!cancelTokenSource.Token.IsCancellationRequested)
    {
      cancelTokenSource.Cancel();
      cancelTokenSource.Dispose();
      Debug.Log("Cancel saved!");
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

      _dataGame = _gameManager.StateManager.dataGame;

      _fileDataHandler.SaveData(_dataGame);

      Debug.Log("Saved complete successfully!");
    }
  }
}
