using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using User;

public class StateManager : MonoBehaviour
{
  // public DataState dataState;
  public static event Action<DataGame, StatePerk> OnChangeState;
  public DataGame dataGame;
  [SerializeField] public StatePerk statePerk;
  public GameLevelWord ActiveWordConfig;
  // public string ActiveWordConfig;

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

  public void Init(DataGame data)
  {
    if (data == null || string.IsNullOrEmpty(data.idPlayerSetting))
    {
      var idPlayerSetting = _gameManager.GameSettings.PlayerSetting.ElementAt(0).idPlayerSetting;
      data = new DataGame()
      {
        idPlayerSetting = idPlayerSetting,
      };
    }

    dataGame = data;
    _gameManager.PlayerSetting = _gameManager.GameSettings.PlayerSetting.Find(t => t.idPlayerSetting == dataGame.idPlayerSetting);

    // Load setting user.
    _gameManager.AppInfo.userSettings = dataGame.userSettings;
  }

  public void RefreshData()
  {
    var managerHiddenWords = _levelManager.ManagerHiddenWords;
    // Save setting user.
    dataGame.userSettings = _gameManager.AppInfo.userSettings;

    dataGame.activeLevel.word = managerHiddenWords.WordForChars;

    dataGame.activeLevel.countDopWords = managerHiddenWords.OpenWords.Count - managerHiddenWords.OpenNeedWords.Count;

    dataGame.activeLevel.openChars.Clear();
    foreach (var item in managerHiddenWords.OpenChars)
    {
      dataGame.activeLevel.openChars.Add(item.Key, item.Value);
    }

    dataGame.activeLevel.ent.Clear();
    foreach (var item in managerHiddenWords.Entities)
    {
      dataGame.activeLevel.ent.Add(item.Key, item.Value);
    }
    dataGame.activeLevel.openWords = managerHiddenWords.OpenWords.Keys.ToList();
    // dataGame.activeLevel.countOpenWords = managerHiddenWords.OpenWords.Count;
    dataGame.activeLevel.allowWords = managerHiddenWords.NeedWords.Keys.ToList();
    dataGame.activeLevel.countWords = managerHiddenWords.NeedWords.Count;
    dataGame.activeLevel.hiddenWords = dataGame.activeLevel.openWords.Count == dataGame.activeLevel.countWords
      ? new()
      : managerHiddenWords.HiddenWords.Keys.ToList();
    dataGame.activeLevel.countOpenChars = managerHiddenWords.OpenWords.Select(t => t.Key.Length).Sum();

    _gameManager.DataManager.Save();
    OnChangeState.Invoke(dataGame, statePerk);
  }

  // public void RefreshData()
  // {
  //   ActuallityData();
  //   Save
  // }

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
    if (statePerk.countCharForBonus >= _gameManager.PlayerSetting.bonusCount.charBonus)
    {
      statePerk.countCharForBonus -= _gameManager.PlayerSetting.bonusCount.charBonus;
      dataGame.activeLevel.index++;
    }

    // Add hint.
    if (statePerk.countCharForAddHint >= _gameManager.PlayerSetting.bonusCount.charHint)
    {
      statePerk.countCharForAddHint -= _gameManager.PlayerSetting.bonusCount.charHint;
      dataGame.activeLevel.hint++;
    }

    // Check add star to grid.
    if (statePerk.countCharForAddStar >= _gameManager.PlayerSetting.bonusCount.charStar)
    {
      statePerk.countCharForAddStar -= _gameManager.PlayerSetting.bonusCount.charStar;
      dataGame.activeLevel.star++;
    }

    RefreshData();
  }

  public void OpenCharHiddenWord(char _char)
  {
    statePerk.countCharForAddCoin += 1;

    // Check add coin to grid.
    if (statePerk.countCharForAddCoin >= _gameManager.PlayerSetting.bonusCount.charCoin)
    {
      statePerk.countCharForAddCoin -= _gameManager.PlayerSetting.bonusCount.charCoin;
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

    if (statePerk.countErrorForNullBonus == _gameManager.PlayerSetting.bonusCount.errorClear)
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


  public void SetActiveLevel(GameLevelWord wordConfig)
  {
    if (wordConfig == null)
    {
      //TODO Not found new level - GAME OVER
      return;
    }

    var allLevels = _gameManager.GameSettings.GameLevels;

    if (dataGame.levels.Count == 0)
    {
      if (dataGame.completeWords.Contains(wordConfig.word))
      {
        // Find not completed word.
        var notCompletedWords = allLevels.levelWords.Where(t => !dataGame.completeWords.Contains(t.idLevelWord));
        if (notCompletedWords.Count() > 0)
        {
          wordConfig = notCompletedWords.ElementAt(0);
          // Clear all words prev level.
          // dataGame.completeWords.Clear();
        }
      }
      // else
      // // Check complete word.
      // {
      //   var allowWords = wordConfig.words.Where(t => !dataGame.completeWords.Contains(t));
      //   if (allowWords.Count() > 0)
      //   {
      //     word = allowWords.ElementAt(0);
      //     // Debug.Log($"Change word [{wordConfig.idLevelWord}-{wordConfig.title}]");
      //   }
      // }
    }

    ActiveWordConfig = wordConfig;
    // ActiveWordConfig = word;
    // dataGame.lastLevelWord = wordConfig.idLevel;
    dataGame.lastLevelWord = wordConfig.idLevelWord;

    if (dataGame.levels.Find(t => t.id == wordConfig.idLevelWord) == null)
    {
      dataGame.levels.Add(new DataLevel()
      {
        id = wordConfig.idLevelWord,
        // word = word,
        hint = wordConfig.countHint,
        star = wordConfig.countStar
      });

    }
    // var indexActiveLevel = dataGame.levels.FindIndex(t => t.id == wordConfig.idLevel && t.word == word);
    dataGame.activeLevel = dataGame.levels.Find((t) => t.id == wordConfig.idLevelWord);
    // Debug.Log($"Set active level ={indexActiveLevel}| {dataGame.activeLevel.openChars.Count}");

    SetDefaultPerk();
  }

  public GameLevelWord GetConfigNextLevel()
  {
    // check completed level.
    if (dataGame.activeLevel.openWords.Count >= dataGame.activeLevel.countWords)
    {
      if (!dataGame.completeWords.Contains(dataGame.activeLevel.id))
      {
        dataGame.completeWords.Add(dataGame.activeLevel.id);
      }
    }
    GameLevelWord result = null;

    dataGame.levels.Remove(dataGame.activeLevel);

    // Find not completed word.
    var notCompletedWords = _gameManager.GameSettings.GameLevels.levelWords.Where(t => !dataGame.completeWords.Contains(t.idLevelWord));

    if (notCompletedWords.Count() > 0)
    {
      result = notCompletedWords.ElementAt(0);
    }

    return result;
  }

  public DataGame GetData()
  {

    return dataGame;
  }

  public void UseHint()
  {
    dataGame.activeLevel.hint--;
    RefreshData();
  }

  public void UseStar()
  {
    dataGame.activeLevel.star--;
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
  public int countCharForAddCoin;
  public int countCharForAddStar;
  public int countErrorForNullBonus;
  public int needCreateCoin;
}