using System.Collections.Generic;

public class BonusIndex : BaseBonus
{
  #region UnityMethods
  protected override void Awake()
  {
    configBonus = _gameManager.ResourceSystem.GetAllBonus().Find(t => t.typeBonus == TypeBonus.Index);

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
    value = data.activeLevel.bonus.GetValueOrDefault(TypeBonus.Index);
    counterText.text = value.ToString();

    base.SetValue(data);

  }

}
