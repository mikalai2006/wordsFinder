using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hint : MonoBehaviour, IPointerDownHandler
{
  // public static event Action<string> OnShuffleWord;
  [SerializeField] private Transform _transform;
  private Vector3 _scale;
  private Vector3 _position;
  [SerializeField] private Image _image;
  [SerializeField] private SpriteRenderer _spriteProgress;
  private float progressBasePositionY = -1.4f;
  [SerializeField] private TMPro.TextMeshProUGUI _countHintText;
  [SerializeField] private Image _countHintImage;
  [SerializeField] private GameObject _countHintObject;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  private StateManager _stateManager => GameManager.Instance.StateManager;
  private LevelManager _levelManager => GameManager.Instance.LevelManager;

  private void Awake()
  {
    _transform = gameObject.transform;
    _scale = _transform.localScale;
    _position = _transform.position;

    _countHintObject.gameObject.SetActive(false);
    _countHintObject.transform.localScale = new Vector3(0, 0, 0);

    StateManager.OnChangeState += SetValue;
  }

  private void OnDestroy()
  {
    StateManager.OnChangeState -= SetValue;
  }

  public void SetValue(DataGame data, StatePerk statePerk)
  {
    // Set new value
    int oldValue = int.Parse(_countHintText.text);
    if (data.activeLevel.hint > 0)
    {
      if (!_countHintObject.activeInHierarchy)
      {
        _countHintObject.gameObject.SetActive(true);
        Show();
      }
      else if (data.activeLevel.hint != oldValue)
      {
        HelpersAnimation.Pulse(_countHintObject, new Vector3(.5f, .5f, 0), 1f);
      }
    }
    else if (oldValue != 0)
    {
      // Hide();
      _countHintObject.gameObject.SetActive(false);
    }
    _countHintText.text = data.activeLevel.hint.ToString();

    // TODO animation get hit.

    SetValueProgressBar(data, statePerk);
  }

  private void Show()
  {
    iTween.ScaleTo(_countHintObject, iTween.Hash(
        "scale", new Vector3(1, 1, 1),
        "time", .5f,
        "easetype", iTween.EaseType.easeOutBack
        // "oncomplete", "CompetedShow",
        // "oncompletetarget", gameObject
        ));
  }

  // private void Hide()
  // {
  //   iTween.ScaleTo(_countHintObject, iTween.Hash(
  //       "scale", new Vector3(0, 0, 0),
  //       "time", .1f,
  //       "easetype", iTween.EaseType.linear,
  //       "oncomplete", "CompetedHide",
  //       "oncompletetarget", gameObject
  //       ));

  // }

  // private void CompetedHide()
  // {
  //   _countHintObject.gameObject.SetActive(false);
  // }

  private void SetDefault()
  {
    transform.localScale = _scale;
    transform.position = _position;
  }

  public void RunHintFrequentChar()
  {
    var nodes = _levelManager.ManagerHiddenWords.GridHelper.GetGroupNodeChars();
    if (nodes.Count <= 0)
    {
      // TODO Show dialog - no hidden char
      return;
    };

    var nodesForShow = nodes.OrderBy(t => -t.Value.Count).First().Value;

    int countRunHit = 0;
    foreach (var node in nodesForShow)
    {
      if (node != null)
      {
        node.OccupiedChar.ShowCharAsNei(true).Forget();
        _levelManager.ManagerHiddenWords.AddOpenChar(node.OccupiedChar);
        node.SetHint();
        countRunHit++;
      }
    }
    if (countRunHit > 0) _stateManager.UseHint();
  }


  public void OnPointerDown(PointerEventData eventData)
  {
    if (_stateManager.dataGame.activeLevel.hint == 0)
    {
      // TODO Show dialog with info get hint by adsense.
      return;
    }
    RunHintFrequentChar();
  }


  private void SetValueProgressBar(DataGame data, StatePerk statePerk)
  {
    var newPosition = (progressBasePositionY + 1.2f) + progressBasePositionY - progressBasePositionY * (float)statePerk.countCharForAddHint / _gameSetting.PlayerSetting.bonusCount.charHint;
    _spriteProgress.transform.localPosition
      = new Vector3(_spriteProgress.transform.localPosition.x, newPosition);
  }

  public async UniTask Destroy()
  {
    var countNotUseHint = _stateManager.dataGame.activeLevel.hint;

    for (int i = 0; i < countNotUseHint; i++)
    {
      var newObj = GameObject.Instantiate(
        _gameSetting.PrefabCoin,
        transform.position,
        Quaternion.identity
      );
      newObj.GetComponent<BaseEntity>().SetColor(_gameSetting.Theme.entityActiveColor);
      var positionFrom = transform.position;
      var positionTo = _levelManager.topSide.spriteCoinPosition;
      Vector3[] waypoints = {
          positionFrom,
          positionFrom + new Vector3(1, 1),
          positionTo - new Vector3(1.5f, 2.5f),
          positionTo - new Vector3(0.5f, 0),
        };
      await UniTask.Delay(150);
      iTween.MoveTo(newObj, iTween.Hash(
        "path", waypoints,
        "time", 2,
        "easetype", iTween.EaseType.easeOutCubic,
        "oncomplete", "OnCompleteEffect"
        ));
    }

    gameObject.SetActive(false);
  }
}
