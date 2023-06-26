using Cysharp.Threading.Tasks;
using UnityEngine;

public class Coin : BaseEntity
{
  #region Unity methods
  protected override void Awake()
  {
    configEntity = _gameManager.ResourceSystem.GetAllEntity().Find(t => t.typeEntity == TypeEntity.Coin);

    base.Awake();
  }
  #endregion

  public override void Init(GridNode node, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> asset)
  {
    base.Init(node, asset);

    node.SetOccupiedEntity(this);

    SetColor(_gameManager.Theme.entityColor);
  }

  public override void InitStandalone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> asset)
  {
    base.InitStandalone(asset);

    spriteBg.color = _gameManager.Theme.bgColor;
    SetColor(_gameManager.Theme.entityColor);
  }

  public override void SetColor(Color color)
  {
    // base.SetColor(color);
    spriteRenderer.color = color;
  }

  public override void AddCoins(int count = 1)
  {
    _stateManager.IncrementCoin(count);

    _levelManager.topSide.AddCoin().Forget();

    Destroy(gameObject);
  }
  public override void AddTotalCoins(int count = 1)
  {
    _stateManager.IncrementTotalCoin(count);

    // _levelManager.topSide.AddCoin().Forget();

    Destroy(gameObject);
  }
  public async override UniTask Run()
  {
    await base.Run();

    // RunEffect();

    Debug.Log("TODO Run coin");
  }
}
