using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct DataState
{
  public int countOpenChars;
  public int countOpenWords;
  public int countAllowWords;
  public int rate;

}
public class StateManager : MonoBehaviour
{
  public DataState dataState;
  public static event Action<DataState> OnChangeState;

  public LevelManager _levelManager => GameManager.Instance.LevelManager;

  private void Awake()
  {
    // ManagerHiddenWords.OnChangeData += RefreshData;
    // DataManager.OnLoadData += LoadState;

  }

  private void OnDestroy()
  {
    // DataManager.OnLoadData -= LoadState;
    // ManagerHiddenWords.OnChangeData -= RefreshData;
  }

  private void RefreshData()
  {
    dataState.countOpenWords = _levelManager.ManagerHiddenWords.OpenPotentialWords.Count;
    dataState.countOpenChars = _levelManager.ManagerHiddenWords.OpenPotentialWords.Keys.Select(t => t.Length).Sum();
    dataState.countAllowWords = _levelManager.ManagerHiddenWords.PotentialWords.Count;

    OnChangeState.Invoke(dataState);
  }

  public void AddWord()
  {
    dataState.rate++;
    RefreshData();
  }

  public void LoadState(DataGame data)
  {
    dataState = data.dataState;
    OnChangeState.Invoke(dataState);
  }
}
