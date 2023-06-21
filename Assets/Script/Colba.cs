using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Colba : MonoBehaviour, IPointerDownHandler
{
  private Vector3 _initScale;
  private Vector3 _initPosition;
  [SerializeField] private GameObject spritesObject;
  [SerializeField] private SpriteRenderer _sprite;
  [SerializeField] private SpriteRenderer _spriteProgress;
  [SerializeField] private TMPro.TextMeshProUGUI _countChars;
  [SerializeField] private GameObject _countStarObject;
  [SerializeField] private TMPro.TextMeshProUGUI _countStarText;
  [SerializeField] private Image _countStarImage;
  private LevelManager _levelManager => GameManager.Instance.LevelManager;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  private StateManager _stateManager => GameManager.Instance.StateManager;
  private GameManager _gameManager => GameManager.Instance;
  private float progressBasePositionY = -1.4f;
  private bool _statusShowCounter = false;
  private int _valueCounter;

  private void Awake()
  {
    _initScale = spritesObject.transform.localScale;
    _initPosition = spritesObject.transform.position;
    _sprite.sprite = _gameSetting.spriteStar;

    _countStarText.text = "0";
    _countStarObject.transform.localScale = new Vector3(0, 0, 0);

    StateManager.OnChangeState += SetValue;
  }

  private void OnDestroy()
  {
    StateManager.OnChangeState -= SetValue;
  }


  public void RunOpenEffect()
  {
    var _CachedSystem = GameObject.Instantiate(
      _gameSetting.Boom,
      transform.position,
      Quaternion.identity
    );

    var main = _CachedSystem.main;
    main.startSize = new ParticleSystem.MinMaxCurve(0.05f, _levelManager.ManagerHiddenWords.scaleGrid / 2);

    var col = _CachedSystem.colorOverLifetime;
    col.enabled = true;

    Gradient grad = new Gradient();
    grad.SetKeys(new GradientColorKey[] {
      new GradientColorKey(_gameSetting.Theme.bgFindAllowWord, 1.0f),
      new GradientColorKey(_gameSetting.Theme.bgHiddenWord, 0.0f)
      }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f)
    });

    col.color = grad;
    _CachedSystem.Play();
    if (_CachedSystem.isPlaying || _CachedSystem.isStopped) Destroy(_CachedSystem.gameObject, 2f);
  }


  public void CreateStar(DataGame data, StatePerk statePerk)
  {
    // if (data.activeLevel.star <= 0) return;

    var potentialGroup = _levelManager.ManagerHiddenWords.GridHelper
      .GetGroupNodeChars()
      // .OrderBy(t => UnityEngine.Random.value)
      .FirstOrDefault();
    if (potentialGroup.Value != null && potentialGroup.Value.Count > 0)
    {
      var node = potentialGroup.Value.First();
      if (node != null)
      {
        // data.activeLevel.star -= 1;
        var starEntity = _levelManager.ManagerHiddenWords.AddEntity(node.arrKey, TypeEntity.Star);
        if (gameObject != null)
        {
          // node.StateNode |= StateNode.Entity;
          starEntity.SetPosition(_levelManager.ManagerHiddenWords.tilemap.WorldToCell(gameObject.transform.position));
        }
        else
        {
          Debug.LogWarning($"Not found {name}");
        }
      }
    }
  }


  public async UniTask AddChar()
  {
    Vector3 initialScale = _initScale;
    Vector3 initialPosition = _initPosition;
    Vector3 upScale = new Vector3(1.5f, 1.5f, 0);

    float elapsedTime = 0f;
    float duration = .2f;
    float startTime = Time.time;

    AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.addToColba);
    while (elapsedTime < duration)
    {
      float progress = (Time.time - startTime) / duration;
      spritesObject.transform.localScale = Vector3.Lerp(initialScale, upScale, progress);
      await UniTask.Yield();
      elapsedTime += Time.deltaTime;
    }
    RunOpenEffect();

    SetDefault();
  }

  public void CreateCoin()
  {
    var newObj = GameObject.Instantiate(
       _gameSetting.PrefabCoin,
       transform.position,
       Quaternion.identity
     );
    var newEntity = newObj.GetComponent<BaseEntity>();
    newEntity.InitStandalone();
    newEntity.SetColor(_gameSetting.Theme.entityActiveColor);
    var positionFrom = transform.position;
    var positionTo = _levelManager.topSide.spriteCoinPosition;
    Vector3[] waypoints = {
          positionFrom,
          positionFrom + new Vector3(1.5f, 0f),
          positionTo - new Vector3(1.5f, 0.5f),
          positionTo,
        };

    newObj.gameObject.transform
      .DOPath(waypoints, 1f, PathType.CatmullRom)
      .SetEase(Ease.OutCubic)
      .OnComplete(() => newEntity.AddCoins(1));
  }

  public void SetValue(DataGame data, StatePerk statePerk)
  {
    // if (data.activeLevel.star > 0)
    // {
    //   for (int i = 0; i < data.activeLevel.star; i++)
    //   {
    //     CreateStar(data, statePerk);
    //   }
    // }

    Debug.Log($"star={data.activeLevel.star}/status={_statusShowCounter}");
    if (data.activeLevel.star > 0)
    {
      if (!_statusShowCounter)
      {
        Show();
      }
      else if (data.activeLevel.star != _valueCounter)
      {
        _countStarObject.transform
          .DOPunchScale(new Vector3(.5f, .5f, 0), _gameSetting.timeGeneralAnimation)
          .SetEase(Ease.OutBack)
          .OnComplete(() =>
          {
            _statusShowCounter = true;
          });
      }
    }
    else // if (_statusShowCounter)
    {

      HideCounter();
    }
    _valueCounter = data.activeLevel.star;
    _countStarText.text = _valueCounter.ToString();
    _countChars.text = string.Format(
      "{0}--{1}",
      data.activeLevel.countOpenChars,
      statePerk.countCharForAddStar
    );

    // TODO animation get hit.

    SetValueProgressBar(data, statePerk);
  }

  private void Show()
  {
    // Debug.Log($"Run Show");
    _countStarObject.transform
      .DOScale(1f, _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack)
      .From(0f, true)
      .OnComplete(() =>
      {
        // Debug.Log($"complete show");
        _statusShowCounter = true;
      });
    _countStarObject.transform
      .DOPunchScale(new Vector3(0.5f, 0.5f, 0), _gameSetting.timeGeneralAnimation)
      .SetDelay(_gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack);
  }

  public void HideCounter()
  {
    // Debug.Log($"Run hide");
    _countStarObject.transform
      .DOScale(0f, _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack)
      .OnComplete(() =>
      {
        // Debug.Log($"complete hide");
        _statusShowCounter = false;
      });
    //gameObject.SetActive(false);
  }

  private void SetValueProgressBar(DataGame data, StatePerk statePerk)
  {
    var newPosition = (progressBasePositionY + 1.2f) + progressBasePositionY - progressBasePositionY * (float)statePerk.countCharForAddStar / _gameManager.PlayerSetting.bonusCount.charStar;
    _spriteProgress.transform.localPosition
      = new Vector3(_spriteProgress.transform.localPosition.x, newPosition);
  }

  public void ResetProgressBar()
  {
    _spriteProgress.transform
      .DOMoveY(progressBasePositionY, _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack);
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    transform
      .DOPunchScale(new Vector3(.2f, .2f, 0), _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack);

    if (_stateManager.dataGame.activeLevel.star == 0)
    {
      // TODO Show dialog with info get hint by adsense.
      return;
    }
    RunHint();
  }

  public void RunHint()
  {
    var node = _levelManager.ManagerHiddenWords.GridHelper.GetRandomNodeWithChar();
    if (node != null)
    {
      node.OccupiedChar.ShowCharAsNei(true).Forget();
      _levelManager.ManagerHiddenWords.AddOpenChar(node.OccupiedChar);
      _stateManager.UseStar();
      node.SetHint();
    }
  }


  private void SetDefault()
  {
    spritesObject.transform.localScale = _initScale;
    spritesObject.transform.position = _initPosition;
  }
}
