using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Coin : BaseEntity
{
  public override void Init(GridNode node)
  {
    base.Init(node);

    node.SetOccupiedEntity(this);

    _spriteRenderer.sprite = _gameSetting.spriteCoin;
    _spriteBg.color = _gameSetting.Theme.bgColor;
    SetColor(_gameSetting.Theme.entityColor);
  }

  public override void InitStandalone()
  {
    base.InitStandalone();

    _spriteRenderer.sprite = _gameSetting.spriteCoin;
    _spriteBg.color = _gameSetting.Theme.bgColor;
    SetColor(_gameSetting.Theme.entityColor);
  }

  public override void SetColor(Color color)
  {
    // base.SetColor(color);
    _spriteRenderer.color = color;
    _spriteRenderer.sprite = _gameSetting.spriteCoin;
  }

  public void RunEffect()
  {
    SetColor(_gameSetting.Theme.entityActiveColor);
    Vector3 positionTo = _levelManager.topSide.spriteCoinPosition;
    Vector3 positionFrom = _levelManager.ManagerHiddenWords.tilemap.CellToWorld(new Vector3Int(OccupiedNode.arrKey.x, OccupiedNode.arrKey.y));
    Vector3[] waypoints = {
      positionFrom,
      positionFrom + new Vector3(1, 1),
      positionTo - new Vector3(1.5f, 1.5f),
      positionTo - new Vector3(0.5f, 0),
    };
    gameObject.transform
      .DOPath(waypoints, 1f, PathType.Linear)
      .SetEase(Ease.OutCubic)
      .OnComplete(() => AddCoins());
  }

  public override void SetPosition(Vector3 pos)
  {
    pos = pos + new Vector3(_spriteRenderer.bounds.size.x / 2, _spriteRenderer.bounds.size.y / 2);
    gameObject.transform
      .DOMove(pos, 1f)
      .SetEase(Ease.OutCubic);

    //SetDefault();
  }

  public override void AddCoins(int count = 1)
  {
    _stateManager.IncrementCoin(count);
    _levelManager.topSide.AddCoin().Forget();
    Destroy(gameObject);
  }

  public override void Run()
  {
    base.Run();

    RunEffect();

    Debug.Log("TODO Run coin");
  }

  public override void OnPointerDown(PointerEventData eventData)
  {
    Debug.Log("TODO Show dialog about coin.");
  }
}
