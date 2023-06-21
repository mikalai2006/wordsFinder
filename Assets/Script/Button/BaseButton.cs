using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseButton : MonoBehaviour, IPointerDownHandler
{
  protected LevelManager _levelManager => GameManager.Instance.LevelManager;
  protected GameSetting _gameSetting => GameManager.Instance.GameSettings;
  protected StateManager _stateManager => GameManager.Instance.StateManager;
  protected GameManager _gameManager => GameManager.Instance;

  protected Vector3 initScale;
  protected Vector3 initPosition;
  [SerializeField] protected GameObject spritesObject;
  [SerializeField] protected SpriteRenderer spriteBg;
  [SerializeField] protected SpriteMask spriteMask;
  [SerializeField] protected SpriteRenderer spriteProgress;
  [SerializeField] protected GameObject counterObject;
  [SerializeField] protected TMPro.TextMeshProUGUI counterText;
  protected float progressBasePositionY = -1.4f;
  protected bool statusShowCounter = false;
  protected int valueCounter;
  protected int value;

  #region UnityMethods
  protected virtual void Awake()
  {
    initScale = spritesObject.transform.localScale;
    initPosition = spritesObject.transform.position;

    counterText.text = "0";
    counterObject.transform.localScale = new Vector3(0, 0, 0);

    ResetProgressBar();

    spriteBg.color = _gameSetting.Theme.colorPrimary;
  }

  protected virtual void OnDestroy()
  {
  }
  #endregion

  public virtual void SetValue(DataGame data, StatePerk statePerk)
  {
    // Debug.Log($"star={data.activeLevel.star}/status={_statusShowCounter}");
    if (value > 0)
    {
      if (!statusShowCounter)
      {
        ShowCounter();
      }
      else if (value != valueCounter)
      {
        counterObject.transform
          .DOPunchScale(new Vector3(.5f, .5f, 0), _gameSetting.timeGeneralAnimation)
          .SetEase(Ease.OutBack)
          .OnComplete(() =>
          {
            statusShowCounter = true;
          });
      }
    }
    else // if (_statusShowCounter)
    {

      HideCounter();
    }
    valueCounter = value;
    counterText.text = value.ToString();

    // TODO animation get hit.

    SetValueProgressBar(data, statePerk);
  }

  private void ShowCounter()
  {
    // Debug.Log($"Run Show");
    counterObject.transform
      .DOScale(1f, _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack)
      .From(0f, true)
      .OnComplete(() =>
      {
        // Debug.Log($"complete show");
        statusShowCounter = true;
      });
    counterObject.transform
      .DOPunchScale(new Vector3(0.5f, 0.5f, 0), _gameSetting.timeGeneralAnimation)
      .SetDelay(_gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack);
    spriteBg.color = _gameSetting.Theme.colorPrimary;
  }

  public void HideCounter()
  {
    // Debug.Log($"Run hide");
    counterObject.transform
      .DOScale(0f, _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack)
      .OnComplete(() =>
      {
        // Debug.Log($"complete hide");
        statusShowCounter = false;
      });
    spriteBg.color = _gameSetting.Theme.colorDisable;
    //gameObject.SetActive(false);
  }

  public virtual void SetValueProgressBar(DataGame data, StatePerk statePerk)
  {
  }

  public virtual void ResetProgressBar()
  {
    spriteProgress.transform
      .DOMoveY(progressBasePositionY, _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack);
  }

  public virtual void OnPointerDown(PointerEventData eventData)
  {
    transform
      .DOPunchScale(new Vector3(.2f, .2f, 0), _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack);

    if (value != 0)
    {
      RunHint();
    }
  }

  public virtual void RunHint()
  {
    // var node = _levelManager.ManagerHiddenWords.GridHelper.GetRandomNodeWithChar();
    // if (node != null)
    // {
    //   node.OccupiedChar.ShowCharAsNei(true).Forget();
    //   _levelManager.ManagerHiddenWords.AddOpenChar(node.OccupiedChar);
    //   _stateManager.UseStar();
    //   node.SetHint();
    // }
  }


  protected void SetDefault()
  {
    spritesObject.transform.localScale = initScale;
    spritesObject.transform.position = initPosition;
  }

  #region Effects

  public virtual void RunOpenEffect()
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

  //   public virtual void CreateStar(DataGame data, StatePerk statePerk)
  //   {
  //     // if (data.activeLevel.star <= 0) return;

  //     var potentialGroup = _levelManager.ManagerHiddenWords.GridHelper
  //       .GetGroupNodeChars()
  //       // .OrderBy(t => UnityEngine.Random.value)
  //       .FirstOrDefault();
  //     if (potentialGroup.Value != null && potentialGroup.Value.Count > 0)
  //     {
  //       var node = potentialGroup.Value.First();
  //       if (node != null)
  //       {
  //         // data.activeLevel.star -= 1;
  //         var starEntity = _levelManager.ManagerHiddenWords.AddEntity(node.arrKey, TypeEntity.Star);
  //         if (gameObject != null)
  //         {
  //           // node.StateNode |= StateNode.Entity;
  //           starEntity.SetPosition(_levelManager.ManagerHiddenWords.tilemap.WorldToCell(gameObject.transform.position));
  //         }
  //         else
  //         {
  //           Debug.LogWarning($"Not found {name}");
  //         }
  //       }
  //     }
  //   }

  public void Hide()
  {
    gameObject.SetActive(false);
  }
  public void Show()
  {
    gameObject.SetActive(true);
  }

  #endregion

}
