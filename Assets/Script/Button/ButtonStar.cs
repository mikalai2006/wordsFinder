using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ButtonStar : BaseButton
{
  [SerializeField] private TMPro.TextMeshProUGUI _countChars;

  #region UnityMethods
  protected override void Awake()
  {
    configEntity = _gameManager.ResourceSystem.GetAllEntity().Find(t => t.typeEntity == TypeEntity.Star);

    base.Awake();

    StateManager.OnChangeState += SetValue;
  }
  protected override void OnDestroy()
  {
    base.OnDestroy();

    StateManager.OnChangeState -= SetValue;
  }
  #endregion

  // public void CreateStar(DataGame data, StatePerk statePerk)
  // {
  //   // if (data.activeLevel.star <= 0) return;

  //   var potentialGroup = _levelManager.ManagerHiddenWords.GridHelper
  //     .GetGroupNodeChars()
  //     // .OrderBy(t => UnityEngine.Random.value)
  //     .FirstOrDefault();
  //   if (potentialGroup.Value != null && potentialGroup.Value.Count > 0)
  //   {
  //     var node = potentialGroup.Value.First();
  //     if (node != null)
  //     {
  //       // data.activeLevel.star -= 1;
  //       var starEntity = _levelManager.ManagerHiddenWords.AddEntity(node.arrKey, TypeEntity.Star);
  //       if (gameObject != null)
  //       {
  //         // node.StateNode |= StateNode.Entity;
  //         starEntity.RunEffect(_levelManager.ManagerHiddenWords.tilemap.WorldToCell(gameObject.transform.position));
  //       }
  //       else
  //       {
  //         Debug.LogWarning($"Not found {name}");
  //       }
  //     }
  //   }
  // }

  #region Effects
  public virtual void RunEffect()
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
  #endregion


  public async UniTask AddChar()
  {
    Vector3 initialScale = initScale;
    Vector3 initialPosition = initPosition;
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
    RunEffect();

    SetDefault();
  }

  public override void SetValue(DataGame data, StatePerk statePerk)
  {
    value = data.star;

    base.SetValue(data, statePerk);
    _countChars.text = string.Format(
      "{0}--{1}",
      data.activeLevel.countOpenChars,
      statePerk.countCharForAddStar
    );
  }

  public override void SetValueProgressBar(DataGame data, StatePerk statePerk)
  {
    var newPosition = (progressBasePositionY + 1.2f) + progressBasePositionY - progressBasePositionY * (float)statePerk.countCharForAddStar / _gameManager.PlayerSetting.bonusCount.charStar;
    // spriteProgress.transform.localPosition
    //   = new Vector3(spriteProgress.transform.localPosition.x, newPosition);
    spriteProgress.transform
      .DOLocalMoveY(newPosition, _gameSetting.timeGeneralAnimation * 2)
      .SetEase(Ease.OutBounce);
  }

  public async override void RunHint()
  {
    var node = _levelManager.ManagerHiddenWords.GridHelper.GetRandomNodeWithHiddenChar();

    if (node == null)
    {
      // Show dialog - not found node for entity
      var message = await Helpers.GetLocaledString("notfoundnodehiddenchar");
      var dialog = new DialogProvider(new DataDialog()
      {
        messageText = message,
        showCancelButton = false
      });

      _gameManager.InputManager.Disable();
      await dialog.ShowAndHide();
      _gameManager.InputManager.Enable();
      return;
    }

    var newEntity = await _levelManager.AddEntity(node.arrKey, TypeEntity.Star);

    node.SetHint();

    newEntity.Move(_levelManager.ManagerHiddenWords.tilemap.WorldToCell(gameObject.transform.position), new() { node }, true);

    _stateManager.UseStar();
  }


}
