using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

public abstract class BaseButton : MonoBehaviour, IPointerDownHandler
{
  protected LevelManager _levelManager => GameManager.Instance.LevelManager;
  protected GameSetting _gameSetting => GameManager.Instance.GameSettings;
  protected StateManager _stateManager => GameManager.Instance.StateManager;
  protected GameManager _gameManager => GameManager.Instance;

  protected MonoBehaviour pointer;
  protected Vector3 initScale;
  protected Vector3 initPosition;
  protected Vector3 initScaleCounterObject;
  protected GameEntity configEntity;
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
    pointer = GetComponent<IPointerDownHandler>() as MonoBehaviour;

    initScale = spritesObject.transform.localScale;
    initPosition = spritesObject.transform.position;
    initScaleCounterObject = counterObject.transform.localScale;

    counterText.text = "0";
    counterObject.transform.localScale = new Vector3(0, 0, 0);

    ResetProgressBar();

    spriteBg.color = _gameSetting.Theme.colorPrimary;

    if (configEntity != null)
    {
      spriteBg.sprite = configEntity.sprite;
      spriteMask.sprite = configEntity.sprite;
    }
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
            counterObject.transform.localScale = initScaleCounterObject;
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
    if (!pointer.enabled) return;

    _gameManager.audioManager.Click();

    pointer.enabled = false;
    transform
        .DOPunchScale(new Vector3(.2f, .2f, 0), _gameSetting.timeGeneralAnimation)
        .SetEase(Ease.OutBack)
        .OnComplete(() =>
        {
          statusShowCounter = true;
          transform.localScale = initScale;
          pointer.enabled = true;
        });

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

  public void Hide()
  {
    gameObject.SetActive(false);
  }

  public void Show()
  {
    gameObject.SetActive(true);
  }

  public virtual async UniTask<BaseEntity> CreateEntity(Vector3 positionForSpawn, GameObject target = null)
  {
    var asset = Addressables.InstantiateAsync(
      configEntity.prefab,
      positionForSpawn,
      Quaternion.identity,
      target == null ? _levelManager.ManagerHiddenWords.tilemapEntities.transform : target.transform
      );
    var newObj = await asset.Task;

    var newEntity = newObj.GetComponent<BaseEntity>();
    newEntity.InitStandalone(asset);

    // _stateManager.RefreshData();

    return newEntity;
  }

}
