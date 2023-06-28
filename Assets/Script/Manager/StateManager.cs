using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StateManager : MonoBehaviour
{
  public static event Action<DataGame> OnChangeState;
  // public static event Action<GameTheme> OnChangeUserSetting;
  public DataGame dataGame;
  public string ActiveWordConfig;

  private LevelManager _levelManager => GameManager.Instance.LevelManager;
  private GameManager _gameManager => GameManager.Instance;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;

  public void Init(DataGame data)
  {
    dataGame = data;
  }

  public void RefreshData()
  {
    var managerHiddenWords = _levelManager.ManagerHiddenWords;

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
    OnChangeState.Invoke(dataGame);
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

    dataGame.activeLevel.coins += quantity;
    // if (_levelManager.ManagerHiddenWords.OpenWords.Keys.Count > 10)
    // {
    //   dataGame.activeLevel.hint++;
    // }
    RefreshData();
  }
  public void IncrementTotalCoin(int quantity)
  {
    AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.addCoin);

    dataGame.coins += quantity;

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

    dataGame.activeLevel.bonusCount.wordInOrder += 1;
    dataGame.activeLevel.bonusCount.errorNullBonus = 0;

    RefreshData();
  }


  public void OpenAllowWord(string word)
  {
    dataGame.rate += 1;

    dataGame.activeLevel.bonusCount.wordInOrder += 1;
    dataGame.activeLevel.bonusCount.errorNullBonus = 0;

    // statePerk.countWordInOrder += 1;
    // statePerk.countErrorForNullBonus = 0;

    RefreshData();
  }


  public void OpenCharAllowWord(char textChar)
  {
    dataGame.activeLevel.bonusCount.charInOrder += 1;
    dataGame.activeLevel.bonusCount.charBonus += 1;
    dataGame.activeLevel.bonusCount.charHint += 1;
    dataGame.activeLevel.bonusCount.charStar += 1;
    dataGame.activeLevel.bonusCount.charBomb += 1;
    dataGame.activeLevel.bonusCount.charLighting += 1;

    // Add bonus index.
    if (dataGame.activeLevel.bonusCount.charBonus >= _gameManager.PlayerSetting.bonusCount.charBonus)
    {
      dataGame.activeLevel.bonusCount.charBonus -= _gameManager.PlayerSetting.bonusCount.charBonus;
      dataGame.activeLevel.index++;
    }

    // Add hint.
    if (dataGame.activeLevel.bonusCount.charHint >= _gameManager.PlayerSetting.bonusCount.charHint)
    {
      dataGame.activeLevel.bonusCount.charHint -= _gameManager.PlayerSetting.bonusCount.charHint;
      // dataGame.hints[TypeEntity.Hint]++;
      UseHint(1, TypeEntity.Frequency);
    }

    // Check add star to grid.
    if (dataGame.activeLevel.bonusCount.charStar >= _gameManager.PlayerSetting.bonusCount.charStar)
    {
      dataGame.activeLevel.bonusCount.charStar -= _gameManager.PlayerSetting.bonusCount.charStar;
      // dataGame.hints[TypeEntity.Star]++;
      UseHint(1, TypeEntity.Star);
    }

    // Add bomb.
    if (dataGame.activeLevel.bonusCount.charBomb >= _gameManager.PlayerSetting.bonusCount.charBomb)
    {
      dataGame.activeLevel.bonusCount.charBomb -= _gameManager.PlayerSetting.bonusCount.charBomb;
      // dataGame.hints[TypeEntity.Bomb]++;
      UseHint(1, TypeEntity.Bomb);
    }

    // Add Lighting.
    if (dataGame.activeLevel.bonusCount.charLighting >= _gameManager.PlayerSetting.bonusCount.charLighting)
    {
      dataGame.activeLevel.bonusCount.charLighting -= _gameManager.PlayerSetting.bonusCount.charLighting;
      UseHint(1, TypeEntity.Lighting);
    }

    RefreshData();
  }

  public void OpenCharHiddenWord(char _char)
  {
    dataGame.activeLevel.bonusCount.charCoin += 1;

    // Check add coin to grid.
    if (dataGame.activeLevel.bonusCount.charCoin >= _gameManager.PlayerSetting.bonusCount.charCoin)
    {
      dataGame.activeLevel.bonusCount.charCoin -= _gameManager.PlayerSetting.bonusCount.charCoin;
      // dataGame.activeLevel.bonusCount.needCreateCoin++;
    }

    OpenCharAllowWord(_char);
  }

  public void DeRunPerk(string word)
  {
    if (word.Length > 1)
    {
      dataGame.activeLevel.bonusCount.errorNullBonus++;
    }

    if (dataGame.activeLevel.bonusCount.errorNullBonus == _gameManager.PlayerSetting.bonusCount.errorNullBonus)
    {
      dataGame.activeLevel.bonusCount.wordInOrder = 0;
      dataGame.activeLevel.bonusCount.charInOrder = 0;
      dataGame.activeLevel.bonusCount.charBonus = 0;
      dataGame.activeLevel.bonusCount.charCoin = 0;
      dataGame.activeLevel.bonusCount.charHint = 0;
      dataGame.activeLevel.bonusCount.charStar = 0;
      dataGame.activeLevel.bonusCount.charBomb = 0;
      dataGame.activeLevel.bonusCount.charLighting = 0;
      dataGame.activeLevel.bonusCount.errorNullBonus = 0;
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

    // SetDefaultPerk();
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

    // _gameManager.PlayerSetting = allPlayerSettings[indexPlayerSetting];

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
    dataGame.levels.Remove(dataGame.activeLevel);

    // Find not completed word.
    string result = null;

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

  public void UseBonus(int count, TypeBonus typeBonus)
  {
    int currentCount;

    dataGame.bonus.TryGetValue(typeBonus, out currentCount);

    dataGame.bonus[typeBonus] = currentCount + count;

    // await _levelManager.topSide.AddBonus(typeBonus);

    // RefreshData();
    OnChangeState?.Invoke(dataGame);
  }

  public void UseHint(int count, TypeEntity typeEntity)
  {
    int currentCount;

    dataGame.hints.TryGetValue(typeEntity, out currentCount);

    dataGame.hints[typeEntity] = currentCount + count;

    // RefreshData();
    OnChangeState?.Invoke(dataGame);
  }


  public void BuyHint(ShopItem<GameEntity> item)
  {
    Debug.Log($"Buy {item.entity.typeEntity}");

    int currentCount;

    dataGame.hints.TryGetValue(item.entity.typeEntity, out currentCount);

    dataGame.hints[item.entity.typeEntity] = item.count + currentCount;

    dataGame.coins -= item.cost;
    // if (_levelManager != null)
    _gameManager.DataManager.Save();
    OnChangeState?.Invoke(dataGame);
  }


  public void BuyBonus(ShopItem<GameBonus> item)
  {
    Debug.Log($"Buy bonus {item.entity.typeBonus}");

    // int currentCount;

    // dataGame.bonus.TryGetValue(item.entity.typeBonus, out currentCount);

    // dataGame.bonus[item.entity.typeBonus] = item.count + currentCount;

    // dataGame.coins -= item.cost;
    // if (_levelManager != null)
    UseBonus(item.count, item.entity.typeBonus);
    _gameManager.DataManager.Save();
    OnChangeState?.Invoke(dataGame);
  }

}
