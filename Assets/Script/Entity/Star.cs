using Cysharp.Threading.Tasks;
using UnityEngine;

public class Star : BaseEntity
{
  #region Unity methods
  protected override void Awake()
  {
    configEntity = _gameManager.ResourceSystem.GetAllEntity().Find(t => t.typeEntity == TypeEntity.Star);

    base.Awake();
  }
  #endregion


  public override void Init(GridNode node, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> asset)
  {
    base.Init(node, asset);

    node.SetOccupiedEntity(this);
  }

  public async override UniTask Run()
  {
    await base.Run();

    RunOpenEffect();

    // var nodes = _levelManager.ManagerHiddenWords.GridHelper.GetEqualsHiddenNeighbours();
    // foreach (var node in nodes)
    // {
    //   if (node != null)
    //   {
    //     node.OccupiedChar.ShowCharAsHint(true).Forget();
    //     _levelManager.ManagerHiddenWords.AddOpenChar(node.OccupiedChar);
    //     node.SetHint();
    //   }
    // }

    Destroy(gameObject);
  }
}
