using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StateManager : MonoBehaviour
{
  // public DataState dataState;
  public static event Action<DataGame, StatePerk> OnChangeState;
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

  private void ActuallityData()
  {

    dataGame.activeLevel.wordForChars = _levelManager.ManagerHiddenWords.WordForChars;

    dataGame.activeLevel.openChars.Clear();
    foreach (var item in _levelManager.ManagerHiddenWords.OpenChars)
    {
      dataGame.activeLevel.openChars.Add(item.Key, item.Value);
    }

    dataGame.activeLevel.ent.Clear();
    foreach (var item in _levelManager.ManagerHiddenWords.Entities)
    {
      dataGame.activeLevel.ent.Add(item.Key, item.Value);
    }
    dataGame.activeLevel.openWords = _levelManager.ManagerHiddenWords.OpenWords.Keys.ToList();
    // dataGame.activeLevel.openHiddenWords = _levelManager.ManagerHiddenWords.OpenHiddenWords.Keys.ToList();
    dataGame.activeLevel.countWords = _levelManager.ManagerHiddenWords.AllowWords.Count;
    dataGame.activeLevel.hiddenWords = dataGame.activeLevel.openWords.Count == dataGame.activeLevel.countWords
      ? new()
      : _levelManager.ManagerHiddenWords.HiddenWords.Keys.ToList();
    dataGame.activeLevel.countOpenChars = _levelManager.ManagerHiddenWords.OpenWords.Select(t => t.Key.Length).Sum();

  }

  public void RefreshData()
  {
    ActuallityData();
    OnChangeState.Invoke(dataGame, statePerk);
  }

  // public void IncrementRate(int quantity)
  // {
  //   dataGame.rate += quantity;
  //   // if (_levelManager.ManagerHiddenWords.OpenWords.Keys.Count > 10)
  //   // {
  //   //   dataGame.activeLevel.hint++;
  //   // }
  //   RefreshData();
  // }

  public void IncrementCoin(int quantity)
  {
    dataGame.coins += quantity;
    // if (_levelManager.ManagerHiddenWords.OpenWords.Keys.Count > 10)
    // {
    //   dataGame.activeLevel.hint++;
    // }
    RefreshData();
  }

  // public void IncrementWord(string word)
  // {
  //   statePerk.countWordInOrder += 1;

  //   RefreshData();
  // }

  public void OpenHiddenWord(string word)
  {
    dataGame.rate += 1;

    statePerk.countWordInOrder += 1;
    statePerk.countErrorForNullBonus = 0;

    RefreshData();
  }


  public void OpenAllowWord(string word)
  {
    dataGame.rate += 1;

    statePerk.countWordInOrder += 1;
    statePerk.countErrorForNullBonus = 0;

    // statePerk.countWordInOrder += 1;
    // statePerk.countErrorForNullBonus = 0;

    RefreshData();
  }


  public void OpenCharAllowWord(char textChar)
  {
    statePerk.countCharInOrder += 1;
    statePerk.countCharForBonus += 1;
    statePerk.countCharForAddHint += 1;
    statePerk.countCharForAddStar += 1;

    // Add bonus index.
    if (statePerk.countCharForBonus >= _gameManager.GameSettings.PlayerSetting.bonusCount.charBonus)
    {
      statePerk.countCharForBonus -= _gameManager.GameSettings.PlayerSetting.bonusCount.charBonus;
      dataGame.activeLevel.index++;
    }

    // Add hint.
    if (statePerk.countCharForAddHint >= _gameManager.GameSettings.PlayerSetting.bonusCount.charHint)
    {
      statePerk.countCharForAddHint -= _gameManager.GameSettings.PlayerSetting.bonusCount.charHint;
      dataGame.activeLevel.hint++;
    }

    // Check add star to grid.
    if (statePerk.countCharForAddStar >= _gameManager.GameSettings.PlayerSetting.bonusCount.charStar)
    {
      statePerk.countCharForAddStar -= _gameManager.GameSettings.PlayerSetting.bonusCount.charStar;
      statePerk.needCreateStar++;
    }
    RefreshData();
  }

  public void OpenCharHiddenWord(char _char)
  {
    statePerk.countCharForAddCoin += 1;

    // Check add coin to grid.
    if (statePerk.countCharForAddCoin >= _gameManager.GameSettings.PlayerSetting.bonusCount.charCoin)
    {
      statePerk.countCharForAddCoin -= _gameManager.GameSettings.PlayerSetting.bonusCount.charCoin;
      statePerk.needCreateCoin++;
    }

    OpenCharAllowWord(_char);
  }

  public void DeRunPerk(string word)
  {

    if (word.Length > 1)
    {
      statePerk.countErrorForNullBonus++;
    }

    if (statePerk.countErrorForNullBonus == _gameManager.GameSettings.PlayerSetting.bonusCount.errorClear)
    {
      statePerk.countWordInOrder = 0;
      statePerk.countCharInOrder = 0;
      statePerk.countCharForBonus = 0;
      statePerk.countCharForAddCoin = 0;
      statePerk.countCharForAddHint = 0;
      statePerk.countCharForAddStar = 0;
      statePerk.countErrorForNullBonus = 0;
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
        idWord = wordConfig.id,
      });
    }
    var indexActiveLevel = dataGame.Levels.FindIndex(t => t.id == levelConfig.id && t.idWord == wordConfig.id);
    dataGame.activeLevel = dataGame.Levels.ElementAt(indexActiveLevel);
    Debug.Log($"Set active level ={indexActiveLevel}| {dataGame.activeLevel.openChars.Count}");

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
    ActuallityData();
    return dataGame;
  }

  public void UseHint()
  {
    dataGame.activeLevel.hint--;
    RefreshData();
    GameManager.Instance.DataManager.Save();
  }
}


[System.Serializable]
public struct StatePerk
{
  public int countCharInOrder;
  public int countWordInOrder;
  public int countCharForBonus;
  public int countCharForAddHint;
  public int countCharForAddCoin;
  public int countCharForAddStar;
  public int countErrorForNullBonus;

  public int needCreateStar;
  public int needCreateCoin;
}