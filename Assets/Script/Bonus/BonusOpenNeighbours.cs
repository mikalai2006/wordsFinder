using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class BonusOpenNeighbours : BaseBonus
{
  #region UnityMethods
  protected override void Awake()
  {
    configBonus = _gameManager.ResourceSystem.GetAllBonus().Find(t => t.typeBonus == TypeBonus.OpenNeighbours);

    base.Awake();

    StateManager.OnChangeState += SetValue;
  }
  protected override void OnDestroy()
  {
    base.OnDestroy();

    StateManager.OnChangeState -= SetValue;
  }
  #endregion

  public override void SetValue(DataGame data)
  {
    value = data.activeLevel.bonus.GetValueOrDefault(TypeBonus.OpenNeighbours);
    base.SetValue(data);

    SetValueProgressBar(data);
  }

  public override void SetValueProgressBar(DataGame data)
  {
    var valueBonus = data.activeLevel.bonus.Where(t => t.Key == configBonus.typeBonus).First().Value;
    var newPosition = (progressBasePositionY + 1.2f) + progressBasePositionY - progressBasePositionY * (float)valueBonus / configBonus.value;

    spriteProgress.transform
      .DOLocalMoveY(newPosition, _gameSetting.timeGeneralAnimation * 2)
      .SetEase(Ease.OutBounce);
  }

}
