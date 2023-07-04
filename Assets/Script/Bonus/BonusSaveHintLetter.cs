using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class BonusSaveHintLetter : BaseBonus
{
  #region UnityMethods
  protected override void Awake()
  {
    configBonus = _gameManager.ResourceSystem.GetAllBonus().Find(t => t.typeBonus == TypeBonus.SaveHintLetter);
    // order.sortingOrder = 25;

    base.Awake();
  }
  #endregion

  public override void SetValue(StateGame state)
  {
    value = state.activeDataGame.bonus.GetValueOrDefault(TypeBonus.SaveHintLetter);

    // counterText.text = string.Format("{0}", value);

    base.SetValue(state);

    if (value == 0) return;

    SetValueProgressBar(state);
  }

  public override void SetValueProgressBar(StateGame state)
  {
    var valueBonus = state.activeDataGame.bonus.Where(t => t.Key == configBonus.typeBonus);
    if (valueBonus != null)
    {

      var val = valueBonus.FirstOrDefault().Value;
      var newPosition = (progressBasePositionY + 1.2f) + progressBasePositionY - progressBasePositionY * (float)val / configBonus.value;

      spriteProgress.transform
        .DOLocalMoveY(newPosition, _gameSetting.timeGeneralAnimation * 2)
        .SetEase(Ease.OutBounce);
    }
  }

}
