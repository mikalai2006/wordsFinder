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
    base.Awake();

    spriteBg.sprite = _gameSetting.spriteStar;
    spriteMask.sprite = _gameSetting.spriteStar;

    StateManager.OnChangeState += SetValue;
  }
  protected override void OnDestroy()
  {
    base.OnDestroy();

    StateManager.OnChangeState -= SetValue;
  }
  #endregion

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

  public override void RunHint()
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


}
