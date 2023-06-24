using System.Collections.Generic;
using System.Linq;

public class ButtonBomb : BaseButton
{
  #region UnityMethods
  protected override void Awake()
  {
    configEntity = _gameManager.ResourceSystem.GetAllEntity().Find(t => t.typeEntity == TypeEntity.Bomb);

    base.Awake();

    StateManager.OnChangeState += SetValue;
  }
  protected override void OnDestroy()
  {
    base.OnDestroy();

    StateManager.OnChangeState -= SetValue;
  }
  #endregion

  public override void SetValue(DataGame data, StatePerk statePerk)
  {
    value = data.hints.GetValueOrDefault(TypeEntity.Bomb);
    base.SetValue(data, statePerk);
  }

  public async override void RunHint()
  {
    GridNode nodeStartEffect = null;
    var helper = _levelManager.ManagerHiddenWords.GridHelper;
    var nodeWithHiddenChar = helper.GetAllNodeWithHiddenChar();

    if (nodeWithHiddenChar.Count == 0)
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
    };

    Dictionary<GridNode, List<GridNode>> potentialNodes = new();
    foreach (var node in nodeWithHiddenChar)
    {
      potentialNodes[node] = helper.GetAllNeighboursWithChar(node)
        .Where(t => !t.StateNode.HasFlag(StateNode.Open))
        .ToList();
    }
    nodeStartEffect = potentialNodes.OrderBy(t => -t.Value.Count).First().Key;

    List<GridNode> nodesForEffect = potentialNodes[nodeStartEffect];
    // nodesForEffect.Add(nodeStartEffect);

    var newEntity = await _levelManager.AddEntity(nodeStartEffect.arrKey, TypeEntity.Bomb);

    nodeStartEffect.SetHint();

    newEntity.Move(_levelManager.ManagerHiddenWords.tilemap.WorldToCell(gameObject.transform.position), nodesForEffect, true);

    _stateManager.UseHint(-1, configEntity.typeEntity);
  }
}
