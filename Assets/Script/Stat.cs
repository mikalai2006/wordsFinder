using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Stat : MonoBehaviour
{
  [SerializeField] private TMPro.TextMeshProUGUI _countWords;
  private LevelManager _levelManager => GameManager.Instance.LevelManager;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  private StateManager _stateManager => GameManager.Instance.StateManager;
  private GameManager _gameManager => GameManager.Instance;

  private void Awake()
  {
    ChangeTheme();

    StateManager.OnChangeState += SetValue;
    UISettings.OnChangeLocale += Localize;
    GameManager.OnChangeTheme += ChangeTheme;
  }

  private void OnDestroy()
  {
    StateManager.OnChangeState -= SetValue;
    UISettings.OnChangeLocale -= Localize;
    GameManager.OnChangeTheme -= ChangeTheme;
  }

  private void ChangeTheme()
  {
    _countWords.color = _gameManager.Theme.colorSecondary;
  }

  public void SetValue(DataGame data)
  {
    Localize();
  }

  private async void Localize()
  {

    // View new data.
    var textCountWords = await Helpers.GetLocalizedPluralString(
          "foundcountword",
           new Dictionary<string, object> {
            {"count",  _levelManager.ManagerHiddenWords.OpenNeedWords.Count},
            {"count2", _gameManager.StateManager.dataGame.activeLevel.countNeedWords},
            {"count3", _levelManager.ManagerHiddenWords.AllowPotentialWords.Count},
            {"count4", _levelManager.ManagerHiddenWords.OpenWords.Count},
          }
        );
    _countWords.text = textCountWords;
  }

  public void Hide()
  {
    gameObject.SetActive(false);
  }

  public void Show()
  {
    gameObject.SetActive(true);
  }
}
