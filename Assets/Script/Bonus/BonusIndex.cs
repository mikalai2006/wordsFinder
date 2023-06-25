using System.Collections.Generic;
using DG.Tweening;

public class BonusIndex : BaseBonus
{
  #region UnityMethods
  protected override void Awake()
  {
    configBonus = _gameManager.ResourceSystem.GetAllBonus().Find(t => t.typeBonus == TypeBonus.Index);

    base.Awake();
  }
  #endregion

  public override void SetValue(DataGame data)
  {
    value = data.bonus.GetValueOrDefault(TypeBonus.Index);

    counterText.text = string.Format("x{0}", value + 1);

    base.SetValue(data);

    if (value == 0) return;

    SetValueProgressBar(data);
  }

  public override void SetValueProgressBar(DataGame data)
  {
    var newPosition = (progressBasePositionY + 1.2f) + progressBasePositionY - progressBasePositionY;

    spriteProgress.transform
      .DOLocalMoveY(newPosition, _gameSetting.timeGeneralAnimation * 2)
      .SetEase(Ease.OutBounce);
  }

}
