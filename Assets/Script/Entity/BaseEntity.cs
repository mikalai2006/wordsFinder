using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseEntity : MonoBehaviour, IPointerDownHandler
{
  // public static event Action<string> OnShuffleWord;
  [SerializeField] protected Transform _transform;
  [SerializeField] protected Vector3 _scale;
  [SerializeField] protected Vector3 _position;
  [SerializeField] protected SpriteRenderer _spriteRenderer;
  // [SerializeField] private TMPro.TextMeshProUGUI _countHintText;
  // [SerializeField] private Image _countHintImage;
  // [SerializeField] private GameObject _countHintObject;
  protected StateManager _stateManager => GameManager.Instance.StateManager;
  protected LevelManager _levelManager => GameManager.Instance.LevelManager;
  protected GameSetting _gameSetting => GameManager.Instance.GameSettings;
  protected GameManager _gameManager => GameManager.Instance;

  public GridNode OccupiedNode;

  protected virtual void Awake()
  {
    _transform = gameObject.transform;

    // StateManager.OnChangeState += SetValue;
  }

  // protected virtual void OnDestroy()
  // {
  //   StateManager.OnChangeState -= SetValue;
  // }

  public virtual void Init(GridNode node)
  {
    transform.localPosition = _position = new Vector3(node.x, node.y) + new Vector3(.5f, .5f);
    OccupiedNode = node;
    _scale = _transform.localScale;
  }

  public virtual void SetColor(Color color)
  {
  }

  // public virtual void SetValue(DataGame data, StatePerk statePerk)
  // {
  //   // _countHintText.text = data.activeLevel.hint.ToString();
  //   // if (data.activeLevel.hint > 0)
  //   // {
  //   //   _countHintObject.gameObject.SetActive(true);
  //   // }
  //   // else
  //   // {
  //   //   _countHintObject.gameObject.SetActive(false);
  //   // }
  //   // TODO animation get hit.
  // }

  public virtual void SetPosition(Vector3 fromPos)
  {
  }

  public virtual void SetDefault()
  {
    transform.localScale = _scale;
    transform.localPosition = _position;
  }

  public virtual void Run()
  {
    _levelManager.ManagerHiddenWords.RemoveEntity(this);

  }

  public virtual void OnPointerDown(PointerEventData eventData)
  {
    // TODO Show dialog about bomb.
  }
}


[System.Serializable]
public enum TypeEntity
{
  None = 0,
  Bomb = 1,
  Lighting = 2,
  Star = 3,
  Coin = 4,
}