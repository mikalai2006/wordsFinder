using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class Star : BaseEntity
{
  public override void Init(GridNode node)
  {
    base.Init(node);

    node.SetOccupiedEntity(this);

    _spriteRenderer.sprite = _gameSetting.spriteStar;
    _spriteRenderer.color = _gameSetting.Theme.entityColor;
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

  public override void Run()
  {
    base.Run();

    RunOpenEffect();

    Debug.Log("TODO Run star");

    var nodes = _levelManager.ManagerHiddenWords.GridHelper.GetEqualsHiddenNeighbours();
    foreach (var node in nodes)
    {
      if (node != null)
      {
        node.OccupiedChar.ShowCharAsNei(true).Forget();
        _levelManager.ManagerHiddenWords.AddOpenChar(node.OccupiedChar);
        node.SetHint();
      }
    }

    Destroy(gameObject);
  }


  public override void SetPosition(Vector3 pos)
  {
    pos = pos + new Vector3(_spriteRenderer.bounds.size.x / 2, _spriteRenderer.bounds.size.y / 2);
    iTween.MoveFrom(gameObject, iTween.Hash(
      "position", pos,
      "islocal", true,
      "time", 2,
      "easetype", iTween.EaseType.easeOutCubic
    ));

    //SetDefault();
  }


  public override void OnPointerDown(PointerEventData eventData)
  {
    Debug.Log("TODO Show dialog about star.");
  }
}
