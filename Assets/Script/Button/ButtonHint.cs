using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ButtonHint : BaseButton
{
  #region UnityMethods
  protected override void Awake()
  {
    base.Awake();

    spriteBg.sprite = _gameSetting.spriteHint;
    spriteMask.sprite = _gameSetting.spriteHint;

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
    value = data.activeLevel.hint;

    base.SetValue(data, statePerk);
  }

  public override void RunHint()
  {
    var nodes = _levelManager.ManagerHiddenWords.GridHelper.GetGroupNodeChars();
    if (nodes.Count <= 0)
    {
      // TODO Show dialog - no hidden char
      return;
    };


    var nodesForShow = nodes.OrderBy(t => -t.Value.Count).First().Value;

    int countRunHit = 0;
    foreach (var node in nodesForShow)
    {
      if (node != null)
      {
        node.OccupiedChar.ShowCharAsNei(true).Forget();
        _levelManager.ManagerHiddenWords.AddOpenChar(node.OccupiedChar);
        node.SetHint();
        countRunHit++;
      }
    }
    if (countRunHit > 0) _stateManager.UseHint();
  }

  public override void SetValueProgressBar(DataGame data, StatePerk statePerk)
  {
    var newPosition = (progressBasePositionY + 1.2f) + progressBasePositionY - progressBasePositionY * (float)statePerk.countCharForAddHint / _gameManager.PlayerSetting.bonusCount.charHint;
    spriteProgress.transform.localPosition
      = new Vector3(spriteProgress.transform.localPosition.x, newPosition);
  }
}
