using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
  private GameManager _gameManager => GameManager.Instance;

  private bool _statusShowCounter = false;
  private int _valueCounter;

  private void Awake()
  {
    _transform = gameObject.transform;
    _scale = _transform.localScale;
    _position = _transform.position;

    _countHintText.text = "0";
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
    if (data.activeLevel.hint > 0)
    {
      if (!_statusShowCounter)
      {
        Show();
      }
      else if (data.activeLevel.hint != _valueCounter)
      {
        // HelpersAnimation.Pulse(_countHintObject, new Vector3(.5f, .5f, 0), .5f);
        _countHintObject.transform
          .DOPunchScale(new Vector3(.5f, .5f, 0), _gameSetting.timeGeneralAnimation)
          .SetEase(Ease.OutBack)
          .OnComplete(() =>
          {
            _statusShowCounter = true;
          });
      }
    }
    else if (_statusShowCounter)
    {
      HideCounter();
    }
    _valueCounter = data.activeLevel.hint;
    _countHintText.text = _valueCounter.ToString();

    // TODO animation get hit.

    SetValueProgressBar(data, statePerk);
  }

  private void Show()
  {
    _countHintObject.transform
      .DOScale(new Vector3(1, 1, 0), _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack)
      .From(0, true)
      .OnComplete(() =>
      {
        _statusShowCounter = true;
      });
    _countHintObject.transform
      .DOPunchScale(new Vector3(0.5f, 0.5f, 0), _gameSetting.timeGeneralAnimation)
      .SetDelay(_gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack);
  }

  public void HideCounter()
  {
    _countHintObject.transform
      .DOScale(new Vector3(0, 0, 0), _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack)
       .OnComplete(() =>
      {
        _statusShowCounter = false;
      });
  }

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
    var newPosition = (progressBasePositionY + 1.2f) + progressBasePositionY - progressBasePositionY * (float)statePerk.countCharForAddHint / _gameManager.PlayerSetting.bonusCount.charHint;
    _spriteProgress.transform.localPosition
      = new Vector3(_spriteProgress.transform.localPosition.x, newPosition);
  }

  public void ResetProgressBar()
  {
    var pos = new Vector3(_spriteProgress.transform.localPosition.x, progressBasePositionY);
    _spriteProgress.transform
      .DOMove(pos, _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack);
  }
}
