using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Bomb : BaseEntity
{
  #region Unity methods
  protected override void Awake()
  {
    configEntity = _gameManager.ResourceSystem.GetAllEntity().Find(t => t.typeEntity == TypeEntity.Bomb);

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

    List<UniTask> tasks = new();
    foreach (var node in nodesForCascade)
    {
      var newEntity = await _levelManager.AddEntity(node.arrKey, configEntity.typeEntity);

      tasks.Add(newEntity.RunCascadeEffect(gameObject.transform.position, .1f));
    }

    await UniTask.WhenAll(tasks);

    nodesForCascade.Clear();

    _gameManager.ChangeState(GameState.StopEffect);

    Destroy(gameObject);
  }
}
