using System;
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

  private DataGame _dataGame;
  public DataGame DataGame { get { return _dataGame; } }

  public void Init()
  {
    _fileDataHandler = new FileDataHandler(Application.persistentDataPath, GameManager.Instance.AppInfo.UserInfo.DeviceId, useEncryption);

  }

  public DataGame Load()
  {
    _dataGame = _fileDataHandler.LoadData();
    // OnLoadData?.Invoke(_dataGame);
    return _dataGame;
  }

  public void Save()
  {
    var levelManager = GameManager.Instance.LevelManager;

    Debug.Log("Save");

    _dataGame = GameManager.Instance.StateManager.GetData();//.dataGame;

    _fileDataHandler.SaveData(_dataGame);
  }

}
