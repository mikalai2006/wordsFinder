using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ButtonHint : BaseButton
{
  #region UnityMethods
  protected override void Awake()
  {
    configEntity = _gameManager.ResourceSystem.GetAllEntity().Find(t => t.typeEntity == TypeEntity.Hint);

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
    value = data.hint;

    base.SetValue(data, statePerk);
  }

  public async override void RunHint()
  {
    var nodeWithHiddenChar = _levelManager.ManagerHiddenWords.GridHelper.GetGroupNodeChars();
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

    var nodesForEffect = nodeWithHiddenChar.OrderBy(t => -t.Value.Count).First().Value.ToList();

    GridNode nodeStartEffect = nodesForEffect.ElementAt(Random.Range(0, nodesForEffect.Count - 1));
    nodesForEffect.Remove(nodeStartEffect);

    var newEntity = await _levelManager.AddEntity(nodeStartEffect.arrKey, TypeEntity.Hint);

    nodeStartEffect.SetHint();

    newEntity.Move(_levelManager.ManagerHiddenWords.tilemap.WorldToCell(gameObject.transform.position), nodesForEffect, true);

    _stateManager.UseHint();
  }

  public override void SetValueProgressBar(DataGame data, StatePerk statePerk)
  {
    var newPosition = (progressBasePositionY + 1.2f) + progressBasePositionY - progressBasePositionY * (float)statePerk.countCharForAddHint / _gameManager.PlayerSetting.bonusCount.charHint;
    // spriteProgress.transform.localPosition
    //   = new Vector3(spriteProgress.transform.localPosition.x, newPosition);
    spriteProgress.transform
      .DOLocalMoveY(newPosition, _gameSetting.timeGeneralAnimation * 2)
      .SetEase(Ease.OutBounce);
  }
}
