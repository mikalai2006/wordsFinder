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
    StateManager.OnChangeState += SetValue;
  }

  private void OnDestroy()
  {
    StateManager.OnChangeState -= SetValue;
  }

  public async void SetValue(DataGame data, StatePerk statePerk)
  {
    // View new data.
    var dataPlural = new Dictionary<string, int> {
      {"count",  _levelManager.ManagerHiddenWords.OpenNeedWords.Count},
      {"count2", data.activeLevel.countWords},
      {"count3", _levelManager.ManagerHiddenWords.AllowPotentialWords.Count}, //  data.activeLevelWord.openWords.Count
    };
    var arguments = new[] { dataPlural };
    var textCountWords = await Helpers.GetLocalizedPluralString(
        new UnityEngine.Localization.LocalizedString(Constants.LanguageTable.LANG_TABLE_LOCALIZE, "foundcountword"),
        arguments,
        dataPlural
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
