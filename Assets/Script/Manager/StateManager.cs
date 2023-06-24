using System;
using System.Collections.Generic;
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
  public string ActiveWordConfig;
  // public string ActiveWordConfig;

  public LevelManager _levelManager => GameManager.Instance.LevelManager;
  public GameManager _gameManager => GameManager.Instance;
  public GameSetting _gameSetting => GameManager.Instance.GameSettings;

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
    if (data == null || string.IsNullOrEmpty(data.rank))
    {
      var idPlayerSetting = _gameSetting.PlayerSetting.ElementAt(0).idPlayerSetting;
      data = new DataGame()
      {
        rank = idPlayerSetting,
        // hint = 10,
        // star = 10,
        // bomb = 10,
        // lighting = 10
      };
    }

    dataGame = data;
    _gameManager.PlayerSetting = _gameSetting.PlayerSetting.Find(t => t.idPlayerSetting == dataGame.rank);

    // Load setting user.
    _gameManager.AppInfo.userSettings = dataGame.setting;
  }

  public void RefreshData()
  {
    var managerHiddenWords = _levelManager.ManagerHiddenWords;
    // Save setting user.
    dataGame.setting = _gameManager.AppInfo.userSettings;

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
    dataGame.activeLevel.needWords = managerHiddenWords.NeedWords.Keys.ToList();
    dataGame.activeLevel.countNeedWords = managerHiddenWords.NeedWords.Count;
    dataGame.activeLevel.hiddenWords = dataGame.activeLevel.openWords.Count == dataGame.activeLevel.countNeedWords
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
    AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.addCoin);

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
      dataGame.hints[TypeEntity.Hint]++;
    }

    // Check add star to grid.
    if (statePerk.countCharForAddStar >= _gameManager.PlayerSetting.bonusCount.charStar)
    {
      statePerk.countCharForAddStar -= _gameManager.PlayerSetting.bonusCount.charStar;
      dataGame.hints[TypeEntity.Star]++;
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


  public List<string> GetAllowNotCompleteWords()
  {
    var allAllowLevelWords = _gameManager.GameSettings.GameLevels
      .Where(t => t.minRate <= dataGame.rate)
      .ToList();
    List<string> allAllowWords = new();
    foreach (var el in allAllowLevelWords)
    {
      allAllowWords.AddRange(el.levelWords);
    };

    return allAllowWords.ToList();
  }

  public void SetActiveLevel(string wordConfig)
  {
    if (wordConfig == null)
    {
      //TODO Not found new level - GAME OVER
      return;
    }

    if (dataGame.levels.Count == 0)
    {
      if (dataGame.completed.Contains(wordConfig))
      {
        var allAllowWords = GetAllowNotCompleteWords();

        // Find not completed word.
        var notCompletedWords = allAllowWords.Where(t => !dataGame.completed.Contains(t));

        if (notCompletedWords.Count() > 0)
        {
          wordConfig = notCompletedWords.ElementAt(0);
        }
      }
    }

    ActiveWordConfig = wordConfig;
    // ActiveWordConfig = word;
    // dataGame.lastLevelWord = wordConfig.idLevel;
    dataGame.lastWord = wordConfig;

    if (dataGame.levels.Find(t => t.id == wordConfig) == null)
    {
      dataGame.levels.Add(new DataLevel()
      {
        id = wordConfig,
        // word = word,
        // hint = (int)Mathf.Round(wordConfig.word.Length * _gameSetting.GameLevels.coefHint),
        // star = (int)Mathf.Round(wordConfig.word.Length * _gameSetting.GameLevels.coefStar)
      });

    }
    // var indexActiveLevel = dataGame.levels.FindIndex(t => t.id == wordConfig.idLevel && t.word == word);
    dataGame.activeLevel = dataGame.levels.Find((t) => t.id == wordConfig);
    // Debug.Log($"Set active level ={indexActiveLevel}| {dataGame.activeLevel.openChars.Count}");

    SetDefaultPerk();
  }

  public GamePlayerSetting SetNewLevelPlayer()
  {
    GamePlayerSetting result = null;

    int countFindWordsForUp = _gameManager.PlayerSetting.countFindWordsForUp;

    if (dataGame.rate < countFindWordsForUp) return null;


    var allPlayerSettings = _gameSetting.PlayerSetting.OrderBy(t => t.countFindWordsForUp).ToList();

    int indexPlayerSetting = allPlayerSettings.FindIndex(t => t.idPlayerSetting == _gameManager.PlayerSetting.idPlayerSetting);

    if (indexPlayerSetting + 1 < allPlayerSettings.Count)
    {
      indexPlayerSetting++;
    }

    dataGame.rank = allPlayerSettings[indexPlayerSetting].idPlayerSetting;

    _gameManager.PlayerSetting = allPlayerSettings[indexPlayerSetting];

    return result;
  }

  public string GetNextWord()
  {
    // check completed level.
    if (dataGame.activeLevel.openWords.Count >= dataGame.activeLevel.countNeedWords)
    {
      if (!dataGame.completed.Contains(dataGame.activeLevel.id))
      {
        dataGame.completed.Add(dataGame.activeLevel.id);
      }
    }
    string result = null;

    dataGame.levels.Remove(dataGame.activeLevel);

    // Find not completed word.
    var allAllowWords = GetAllowNotCompleteWords();

    var notCompletedWords = allAllowWords.Where(t => !dataGame.completed.Contains(t));

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

  public void UseHint(int count, TypeEntity typeEntity)
  {
    int currentCount;

    dataGame.hints.TryGetValue(typeEntity, out currentCount);

    dataGame.hints[typeEntity] = currentCount + count;

    RefreshData();
  }
  // public void UseBomb(int count)
  // {
  //   dataGame.bomb -= count;
  //   RefreshData();
  // }
  // public void UseLighting(int count)
  // {
  //   dataGame.lighting -= count;
  //   RefreshData();
  // }

  // public void UseStar(int count)
  // {
  //   dataGame.star -= count;
  //   RefreshData();
  // }

  public void BuyHint(ShopItem item)
  {
    Debug.Log($"Buy {item.entity.typeEntity}");
    dataGame.hints[item.entity.typeEntity] += item.count;
    // switch (item.entity.typeEntity)
    // {
    //   case TypeEntity.Hint:
    //     dataGame.hint += item.count;
    //     break;
    //   case TypeEntity.Star:
    //     dataGame.star += item.count;
    //     break;
    //   case TypeEntity.Bomb:
    //     dataGame.bomb += item.count;
    //     break;
    //   case TypeEntity.Lighting:
    //     dataGame.lighting += item.count;
    //     break;
    //   case TypeEntity.OpenWord:
    //     dataGame.word += item.count;
    //     break;
    // }

    dataGame.coins -= item.cost;
    if (_levelManager != null) RefreshData();
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