using System.Linq;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
  [Header("File Storage Config")]
  [SerializeField] private string fileNameGame;
  [SerializeField] private string fileNameMap;
  [SerializeField] private bool useEncryption;

  private FileDataHandler _fileDataHandler;

  private DataPlay _dataPlay;
  public DataPlay DataPlay { get { return _dataPlay; } }

  public void Init()
  {
    _fileDataHandler = new FileDataHandler(Application.persistentDataPath, GameManager.Instance.AppInfo.UserInfo.DeviceId, useEncryption);

  }

  public void New()
  {
    _dataPlay = new DataPlay();

  }

  public void Load()
  {

    _dataPlay = _fileDataHandler.LoadData();

  }

  public void Save()
  {
    New();
    var levelManager = GameManager.Instance.LevelManager;
    _dataPlay.Level = 1;
    _dataPlay.potentialWords = levelManager.ManagerHiddenWords.PotentialWords.Keys.ToList();
    _dataPlay.openHiddenWords = levelManager.ManagerHiddenWords.OpenHiddenWords.Keys.ToList();
    _dataPlay.openPotentialWords = levelManager.ManagerHiddenWords.OpenPotentialWords.Keys.ToList();
    _dataPlay.hiddenWords = levelManager.ManagerHiddenWords.hiddenWords.Keys.ToList();
    _dataPlay.wordSymbol = levelManager.ManagerHiddenWords.wordSymbol;

    _fileDataHandler.SaveData(_dataPlay);

  }

}
