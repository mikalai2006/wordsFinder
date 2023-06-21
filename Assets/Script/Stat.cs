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
    _countWords.color = _gameSetting.Theme.colorSecondary;

    StateManager.OnChangeState += SetValue;
    UISettings.OnChangeLocale += Localize;
  }

  private void OnDestroy()
  {
    StateManager.OnChangeState -= SetValue;
    UISettings.OnChangeLocale -= Localize;
  }

  public void SetValue(DataGame data, StatePerk statePerk)
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
            {"count2", _gameManager.StateManager.dataGame.activeLevel.countWords},
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
