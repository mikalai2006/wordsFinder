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
  }
  #endregion

  public override void SetValue(DataGame data)
  {
    value = data.bonus.GetValueOrDefault(TypeBonus.OpenNeighbours);

    // counterText.text = string.Format("{0}", value);

    base.SetValue(data);

    if (value == 0) return;

    SetValueProgressBar(data);
  }

  public override void SetValueProgressBar(DataGame data)
  {
    var valueBonus = data.bonus.Where(t => t.Key == configBonus.typeBonus);
    if (valueBonus != null)
    {

      var val = valueBonus.First().Value;
      var newPosition = (progressBasePositionY + 1.2f) + progressBasePositionY - progressBasePositionY * (float)val / configBonus.value;

      spriteProgress.transform
        .DOLocalMoveY(newPosition, _gameSetting.timeGeneralAnimation * 2)
        .SetEase(Ease.OutBounce);
    }
  }

}
