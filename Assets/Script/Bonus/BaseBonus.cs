using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

public abstract class BaseBonus : MonoBehaviour, IPointerDownHandler
{
  protected LevelManager _levelManager => GameManager.Instance.LevelManager;
  protected GameSetting _gameSetting => GameManager.Instance.GameSettings;
  protected StateManager _stateManager => GameManager.Instance.StateManager;
  protected GameManager _gameManager => GameManager.Instance;

  protected Vector3 initScale;
  protected Vector3 initPosition;
  protected Vector3 initScaleCounterObject;
  protected GameBonus configBonus;
  [SerializeField] protected GameObject spritesObject;
  [SerializeField] protected SpriteRenderer spriteBg;
  [SerializeField] protected SpriteMask spriteMask;
  [SerializeField] protected SpriteRenderer spriteProgress;
  [SerializeField] protected TMPro.TextMeshProUGUI counterText;
  protected float progressBasePositionY = -1.4f;
  protected bool statusShowCounter = false;
  protected int valueCounter;
  protected int value;
  private UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> asset;

  #region UnityMethods
  protected virtual void Awake()
  {
    initScale = spritesObject.transform.localScale;
    initPosition = spritesObject.transform.position;

    spriteBg.color = _gameSetting.Theme.colorPrimary;

    initScaleCounterObject = counterText.gameObject.transform.localScale;
    counterText.text = "";

    gameObject.SetActive(false);

    // ResetProgressBar();
    // spriteProgress.color = _gameSetting.Theme.entityActiveColor;

    if (configBonus != null)
    {
      spriteBg.sprite = configBonus.sprite;
      spriteMask.sprite = configBonus.sprite;
    }
  }

  protected virtual void OnDestroy()
  {
  }
  #endregion


  public virtual void Init(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> asset)
  {
    this.asset = asset;
  }


  public virtual void SetValue(DataGame data)
  {
    // Debug.Log($"star={data.activeLevel.star}/status={_statusShowCounter}");
    if (value > 0 && valueCounter != value)
    {
      Show();
    }
    else if (value == 0)
    {

      Hide();
    }

    valueCounter = value;
  }

  private void Show()
  {
    gameObject.SetActive(true);

    _gameManager.audioManager.PlayClipEffect(_gameSetting.Audio.addBonus);
    spritesObject.transform
      .DOPunchScale(new Vector3(1f, 1f, 0), _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack);
  }

  public void Hide()
  {
    // Debug.Log($"Run hide");
    counterText.transform
      .DOScale(0f, _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack)
      .OnComplete(() =>
      {
        // Debug.Log($"complete hide");
        Destroy(gameObject);
      });
  }

  public virtual void SetValueProgressBar(DataGame data)
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
    _gameManager.audioManager.Click();

    transform
        .DOPunchScale(new Vector3(.2f, .2f, 0), _gameSetting.timeGeneralAnimation)
        .SetEase(Ease.OutBack)
        .OnComplete(() =>
        {
          transform.localScale = initScale;
        });

    if (value != 0)
    {
      // RunHint();
    }
  }

  protected void SetDefault()
  {
    spritesObject.transform.localScale = initScale;
    spritesObject.transform.position = initPosition;
  }

}
