using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Stat : MonoBehaviour
{
  [SerializeField] private TMPro.TextMeshProUGUI _countWords;
  private LevelManager _levelManager => GameManager.Instance.LevelManager;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  private StateManager _stateManager => GameManager.Instance.StateManager;
  private GameManager _gameManager => GameManager.Instance;
  private float maxWidthProgress = 10f;
  [SerializeField] private RectTransform spriteProgress;

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
    _countWords.color = _gameManager.Theme.colorPrimary;
  }

  public void SetValue(StateGame state)
  {
    Localize();

    SetProgressValue(state);
  }

  private void SetProgressValue(StateGame state)
  {
    float width = 0;
    if (state.activeDataGame.activeLevel.countNeedWords > 0)
    {
      width = ((state.activeDataGame.activeLevel.openWords.Count - state.activeDataGame.activeLevel.countDopWords) * 100f / state.activeDataGame.activeLevel.countNeedWords) * (maxWidthProgress / 100f);
    }

    spriteProgress.DOSizeDelta(new Vector3(width, 1f), _gameSetting.timeGeneralAnimation);
    //.sizeDelta = new Vector3(width, 1f);
  }

  private async void Localize()
  {

    // View new data.
    var textCountWords = await Helpers.GetLocalizedPluralString(
          "foundcountword",
           new Dictionary<string, object> {
            {"count",  _levelManager.ManagerHiddenWords.OpenNeedWords.Count},
            {"count2", _gameManager.StateManager.dataGame.activeLevel.countNeedWords},
            // {"count3", _levelManager.ManagerHiddenWords.AllowPotentialWords.Count},
            // {"count4", _levelManager.ManagerHiddenWords.OpenWords.Count},
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
