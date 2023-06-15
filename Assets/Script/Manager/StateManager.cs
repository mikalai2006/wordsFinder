using System;
using System.Linq;
using UnityEngine;

public class StateManager : MonoBehaviour
{
  // public DataState dataState;
  public DataGame dataGame;
  public GameLevel ActiveLevelConfig;
  public GameLevelWord ActiveWordConfig;
  public static event Action<DataGame> OnChangeState;

  public LevelManager _levelManager => GameManager.Instance.LevelManager;

  private void Awake()
  {
    // ManagerHiddenWords.OnChangeData += RefreshData;

  }

  private void OnDestroy()
  {
    // ManagerHiddenWords.OnChangeData -= RefreshData;
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
  }

  public void LoadState(DataGame data)
  {
    if (data == null)
    {
      data = new DataGame();
    }
    dataGame = data;
  }
}
