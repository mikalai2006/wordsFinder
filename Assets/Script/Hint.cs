using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hint : MonoBehaviour, IPointerDownHandler
{
  // public static event Action<string> OnShuffleWord;
  [SerializeField] private Transform _transform;
  [SerializeField] private Vector3 _scale;
  [SerializeField] private Vector3 _position;
  [SerializeField] private Image _image;
  [SerializeField] private TMPro.TextMeshProUGUI _countHintText;
  [SerializeField] private Image _countHintImage;
  [SerializeField] private GameObject _countHintObject;
  private StateManager _stateManager => GameManager.Instance.StateManager;
  private LevelManager _levelManager => GameManager.Instance.LevelManager;

  private void Awake()
  {
    _transform = gameObject.transform;
    _scale = _transform.localScale;
    _position = _transform.position;

    StateManager.OnChangeState += SetValue;
  }

  private void OnDestroy()
  {
    StateManager.OnChangeState -= SetValue;
  }

  public void SetValue(DataGame data, StatePerk statePerk)
  {
    _countHintText.text = data.activeLevel.hint.ToString();
    if (data.activeLevel.hint > 0)
    {
      _countHintObject.gameObject.SetActive(true);
    }
    else
    {
      _countHintObject.gameObject.SetActive(false);
    }
    // TODO animation get hit.
  }

  private void SetDefault()
  {
    transform.localScale = _scale;
    transform.position = _position;
  }

  public void RunHint()
  {
    var node = _levelManager.ManagerHiddenWords.GridHelper.GetRandomNodeWithChar();
    if (node != null)
    {
      node.OccupiedChar.ShowCharAsNei(true).Forget();
      _levelManager.ManagerHiddenWords.AddOpenChar(node.OccupiedChar);
      _stateManager.UseHint();
    }
  }

  private void RunHintFrequentChar()
  {
    var nodes = _levelManager.ManagerHiddenWords.GridHelper.GetGroupNodeChars(); //.GetRandomNodeWithChar();
    var nodesForShow = nodes.OrderBy(t => t.Value.Count).First().Value;
    foreach (var node in nodesForShow)
    {
      node.OccupiedChar.ShowCharAsNei(true).Forget();
      _levelManager.ManagerHiddenWords.AddOpenChar(node.OccupiedChar);
    }

    _stateManager.UseHint();
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    if (_stateManager.dataGame.activeLevel.hint == 0)
    {
      // TODO Show dialog with info get hint by adsense.
      return;
    }
    RunHint();
  }
}
