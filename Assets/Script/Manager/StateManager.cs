using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StateManager : MonoBehaviour
{
  // public DataState dataState;
  public static event Action<DataGame> OnChangeState;
  public DataGame dataGame;
  [SerializeField] public StatePerk statePerk;
  public GameLevel ActiveLevelConfig;
  public GameLevelWord ActiveWordConfig;

  public LevelManager _levelManager => GameManager.Instance.LevelManager;
  public GameManager _gameManager => GameManager.Instance;

  private void Awake()
  {
    // ManagerHiddenWords.OnChangeData += RefreshData;
    SetDefaultPerk();
  }

  private void OnDestroy()
  {
    // ManagerHiddenWords.OnChangeData -= RefreshData;
  }

  private void SetDefaultPerk()
  {
    statePerk = new();
  }

  public void RefreshData()
  {
    dataGame.activeLevel.wordForChars = _levelManager.ManagerHiddenWords.WordForChars;
    dataGame.activeLevel.openWords = _levelManager.ManagerHiddenWords.OpenWords.Keys.ToList();
    // dataGame.activeLevel.openHiddenWords = _levelManager.ManagerHiddenWords.OpenHiddenWords.Keys.ToList();
    dataGame.activeLevel.countWords = _levelManager.ManagerHiddenWords.AllowWords.Count;
    dataGame.activeLevel.hiddenWords = _levelManager.ManagerHiddenWords.hiddenWords.Keys.ToList();
    dataGame.activeLevel.countOpenChars = _levelManager.ManagerHiddenWords.OpenWords.Select(t => t.Key.Length).Sum();

    OnChangeState.Invoke(dataGame);
  }

  public void AddWord()
  {
    dataGame.rate++;
    // if (_levelManager.ManagerHiddenWords.OpenWords.Keys.Count > 10)
    // {
    //   dataGame.activeLevel.hint++;
    // }
    RefreshData();
  }


  public void RunPerk(string word)
  {
    statePerk.countCharInOrder += word.Length;
    statePerk.countWordInOrder += 1;
    statePerk.countCharForBonus += word.Length;
    statePerk.countCharForAddHint += word.Length;

    statePerk.countNotFoundForClearCharBonus = 0;

    // Add bonus index.
    if (statePerk.countCharForBonus >= _gameManager.GameSettings.PlayerSetting.countCharForBonus)
    {
      statePerk.countCharForBonus -= _gameManager.GameSettings.PlayerSetting.countCharForBonus;
      dataGame.activeLevel.index++;
    }

    // Add hint.
    if (statePerk.countCharForAddHint >= _gameManager.GameSettings.PlayerSetting.countCharForAddHint)
    {
      statePerk.countCharForAddHint -= _gameManager.GameSettings.PlayerSetting.countCharForAddHint;
      dataGame.activeLevel.hint++;
    }

    RefreshData();
  }


  public void DeRunPerk(string word)
  {

    if (word.Length > 1)
    {
      statePerk.countWordInOrder = 0;
      statePerk.countCharInOrder = 0;
      statePerk.countNotFoundForClearCharBonus++;
    }

    if (statePerk.countNotFoundForClearCharBonus == _gameManager.GameSettings.PlayerSetting.countNotFoundForClearCharBonus)
    {
      statePerk.countCharForBonus = 0;
      statePerk.countCharForAddHint = 0;
      statePerk.countNotFoundForClearCharBonus = 0;
    }
    RefreshData();
  }


  public void SetActiveLevel(GameLevel levelConfig, GameLevelWord wordConfig)
  {
    ActiveLevelConfig = levelConfig;
    ActiveWordConfig = wordConfig;
    dataGame.lastActiveLevelId = levelConfig.id;
    dataGame.lastActiveWordId = wordConfig.id;

    if (dataGame.Levels.Find(t => t.id == levelConfig.id && t.idWord == wordConfig.id) == null)
    {
      dataGame.Levels.Add(new DataLevel()
      {
        id = levelConfig.id,
        idWord = wordConfig.id
      });
    }
    var indexActiveLevel = dataGame.Levels.FindIndex(t => t.id == levelConfig.id && t.idWord == wordConfig.id);
    dataGame.activeLevel = dataGame.Levels.ElementAt(indexActiveLevel);

    SetDefaultPerk();
  }

  public void LoadState(DataGame data)
  {
    if (data == null)
    {
      data = new DataGame();
    }
    dataGame = data;
  }

  public DataGame GetData()
  {
    // RefreshData();
    return dataGame;
  }

  public void UseHint()
  {
    dataGame.activeLevel.hint--;
    RefreshData();
  }
}


[System.Serializable]
public struct StatePerk
{
  public int countCharInOrder;
  public int countWordInOrder;
  public int countCharForBonus;
  public int countCharForAddHint;
  public int countNotFoundForClearCharBonus;
}