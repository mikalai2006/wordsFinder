using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

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
  [SerializeField] protected Canvas canvasObject;
  [SerializeField] protected SpriteRenderer spriteBg;
  [SerializeField] protected SpriteMask spriteMask;
  [SerializeField] protected SpriteRenderer spriteProgress;
  [SerializeField] protected TMPro.TextMeshProUGUI counterText;
  [SerializeField] protected SortingGroup order;
  protected float progressBasePositionY = -1.4f;
  protected bool statusShowCounter = false;
  protected int valueCounter;
  protected int value;
  private UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> asset;

  #region UnityMethods
  protected virtual void Awake()
  {
    initScale = transform.localScale;
    initPosition = transform.localPosition;

    ChangeTheme();

    initScaleCounterObject = counterText.gameObject.transform.localScale;
    counterText.text = "";

    gameObject.SetActive(false);

    // ResetProgressBar();

    if (configBonus != null)
    {
      spriteBg.sprite = configBonus.sprite;
      spriteMask.sprite = configBonus.sprite;
    }

    StateManager.OnChangeState += SetValue;
    GameManager.OnChangeTheme += ChangeTheme;
  }
  protected virtual void OnDestroy()
  {
    StateManager.OnChangeState -= SetValue;
    GameManager.OnChangeTheme -= ChangeTheme;
  }
  #endregion

  private void ChangeTheme()
  {
    spriteBg.color = _gameManager.Theme.entityColor;
    spriteProgress.color = _gameManager.Theme.colorAccent;
    counterText.color = _gameManager.Theme.bgColor;
  }


  public virtual void Init(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> asset)
  {
    this.asset = asset;
  }


  public virtual void SetValue(StateGame state)
  {
    if (valueCounter == value) return;

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

  public virtual void Show()
  {
    gameObject.SetActive(true);

    _gameManager.audioManager.PlayClipEffect(_gameSetting.Audio.addBonus);
    transform
      .DOPunchScale(new Vector3(1f, 1f, 0), _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack)
      .OnComplete(() =>
      {
        SetDefault();
      });
  }

  public void Hide()
  {
    transform
      .DOScale(0f, _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack)
      .OnComplete(() =>
      {
        // Destroy(gameObject);
        gameObject.SetActive(false);
      });
  }

  public virtual void SetValueProgressBar(StateGame state)
  {
  }

  public virtual void ResetProgressBar()
  {
    spriteProgress.transform
      .DOMoveY(progressBasePositionY, _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutBack);
  }

  public async virtual void OnPointerDown(PointerEventData eventData)
  {
    _gameManager.audioManager.Click();

    transform
        .DOPunchScale(new Vector3(.2f, .2f, 0), _gameSetting.timeGeneralAnimation)
        .SetEase(Ease.OutBack);
    var title = await Helpers.GetLocaledString(configBonus.text.title);
    var message = await Helpers.GetLocaledString(configBonus.text.description);
    var dialog = new DialogProvider(new DataDialog()
    {
      sprite = configBonus.sprite,
      title = title,
      message = message,
      showCancelButton = false
    });

    _gameManager.InputManager.Disable();
    await dialog.ShowAndHide();
    _gameManager.InputManager.Enable();

  }

  public void SetDefault()
  {
    transform.localScale = initScale;
    order.sortingOrder = 1;
    canvasObject.sortingOrder = 1;
    // transform.localPosition = Vector3.zero;
  }

  public void SetSortOrder(int value)
  {
    order.sortingOrder = value;
    canvasObject.sortingOrder = value;
  }
}
