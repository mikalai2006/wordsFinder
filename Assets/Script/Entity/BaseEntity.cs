using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class BaseEntity : MonoBehaviour, IPointerDownHandler
{
  // public static event Action<string> OnShuffleWord;
  protected StateManager _stateManager => GameManager.Instance.StateManager;
  protected LevelManager _levelManager => GameManager.Instance.LevelManager;
  protected GameSetting _gameSetting => GameManager.Instance.GameSettings;
  protected GameManager _gameManager => GameManager.Instance;
  public GameEntity configEntity { get; protected set; }
  protected Vector3 initScale;
  protected Vector3 initPosition;
  [SerializeField] protected SpriteRenderer spriteRenderer;
  [SerializeField] protected SpriteRenderer spriteBg;
  [SerializeField] private SortingGroup order;
  [SerializeField] public GameObject counterObject;
  [SerializeField] public TMPro.TextMeshProUGUI counterText;
  public GridNode OccupiedNode;
  protected List<GridNode> nodesForCascade = new();
  private UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> asset;
  // [SerializeField] private TMPro.TextMeshProUGUI _countHintText;

  #region Unity methods
  protected virtual void Awake()
  {
    spriteRenderer.sprite = configEntity.sprite;
    spriteRenderer.color = _gameSetting.Theme.entityColor;
    spriteBg.color = Color.clear;

    counterObject.transform.localScale = Vector3.zero;
  }

  private void OnDestroy()
  {
    Addressables.Release(asset);
  }
  #endregion

  public virtual void Init(GridNode node, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> asset)
  {
    this.asset = asset;

    OccupiedNode = node;
    transform.localPosition = initPosition = node.arrKey + new Vector2(.5f, .5f); //  = _position
    initScale = transform.localScale;
    gameObject.SetActive(false);
  }

  public virtual void InitStandalone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> asset)
  {
    this.asset = asset;

    initScale = transform.localScale;
    order.sortingOrder = 51;
  }

  public virtual void SetColor(Color color)
  {
  }

  #region Effects
  public virtual void AddCoins(int count = 1)
  {

  }


  public void RunOpenEffect()
  {
    var _CachedSystem = GameObject.Instantiate(
      _gameSetting.BoomLarge,
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
    Destroy(_CachedSystem.gameObject, 2f);
  }


  public void RunMoveEffect()
  {
    var _CachedSystem = GameObject.Instantiate(
      configEntity.MoveEffect,
      transform.position,
      Quaternion.identity,
      transform
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
    // Destroy(_CachedSystem.gameObject, 2f);
  }
  #endregion

  public virtual void Move(Vector3 positionFrom, List<GridNode> nodesForEffect, bool isImmediatelyRun)
  {
    positionFrom = positionFrom + new Vector3(spriteRenderer.bounds.size.x / 2, spriteRenderer.bounds.size.y / 2);
    transform.localPosition = positionFrom;
    gameObject.SetActive(true);

    RunMoveEffect();

    Vector3[] waypoints = {
          positionFrom,
          positionFrom + new Vector3(Random.Range(0,1), Random.Range(0,1)),
          initPosition - new Vector3(Random.Range(0.5f,1.5f), Random.Range(0.5f,2.5f)),
          initPosition,
        };
    transform
      .DOLocalPath(waypoints, 1f, PathType.CatmullRom)
      .From(true)
      .SetEase(Ease.OutCubic)
      .OnComplete(async () =>
      {
        if (isImmediatelyRun)
        {
          _gameManager.audioManager.PlayClipEffect(_gameSetting.Audio.runEffect);
          this.nodesForCascade = nodesForEffect;
          await OccupiedNode.OccupiedChar.ShowCharAsHint(true);
        }
      });

    //SetDefault();
  }

  public virtual void RunCascadeEffect(Vector3 positionFrom, float duration = .5f)
  {
    gameObject.SetActive(true);
    // spriteRenderer.gameObject.SetActive(false);
    RunMoveEffect();

    transform
      .DOMove(transform.position, duration)
      .From(positionFrom, true)
      .SetEase(Ease.Linear)
      .OnComplete(async () =>
      {
        OccupiedNode.SetHint();

        await OccupiedNode.OccupiedChar.ShowCharAsHint(true);
      });
  }

  public virtual void SetDefault()
  {
    transform.localScale = initScale;
    transform.localPosition = initPosition;
  }

  public async virtual UniTask Run()
  {
    _levelManager.ManagerHiddenWords.RemoveEntity(this);

    await UniTask.Yield();
  }

  public async virtual void OnPointerDown(PointerEventData eventData)
  {
    var title = await Helpers.GetLocaledString(configEntity.text.title);
    var message = await Helpers.GetLocaledString(configEntity.text.description);
    var dialog = new DialogProvider(new DataDialog()
    {
      headerText = title,
      messageText = message,
      showCancelButton = false
    });

    _gameManager.InputManager.Disable();
    await dialog.ShowAndHide();
    _gameManager.InputManager.Enable();
  }

  #region LoadAsset
  private async void CreateEntity()
  {
    AssetReferenceGameObject gameObj = null;
    if (configEntity.prefab.RuntimeKeyIsValid())
    {
      gameObj = configEntity.prefab;
    }

    if (gameObj == null)
    {
      Debug.LogWarning($"Not found mapPrefab {configEntity.name}!");
      return;
    }
    var asset = Addressables.InstantiateAsync(
               gameObj,
               OccupiedNode.position,
               Quaternion.identity,
               _levelManager.ManagerHiddenWords.tilemapEntities.transform
               );
    await asset.Task;
  }
  #endregion

}