using System;
using System.Linq;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
  public static event Action<DataGame> OnLoadData;
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

  public void New()
  {
    _dataGame = new DataGame();

  }

  public DataGame Load()
  {

    _dataGame = _fileDataHandler.LoadData();
    OnLoadData?.Invoke(_dataGame);
    return _dataGame;
  }

  public void Save()
  {
    New();
    var levelManager = GameManager.Instance.LevelManager;
    _dataGame.Level = 1;
    _dataGame.potentialWords = levelManager.ManagerHiddenWords.PotentialWords.Keys.ToList();
    _dataGame.openHiddenWords = levelManager.ManagerHiddenWords.OpenHiddenWords.Keys.ToList();
    _dataGame.openPotentialWords = levelManager.ManagerHiddenWords.OpenPotentialWords.Keys.ToList();
    _dataGame.hiddenWords = levelManager.ManagerHiddenWords.hiddenWords.Keys.ToList();
    _dataGame.wordForChars = levelManager.ManagerHiddenWords.WordForChars;
    _dataGame.dataState = GameManager.Instance.StateManager.dataState;

    _fileDataHandler.SaveData(_dataGame);
  }

}
