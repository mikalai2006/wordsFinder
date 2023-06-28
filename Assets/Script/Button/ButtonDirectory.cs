using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonDirectory : BaseButton
{
  #region UnityMethods
  protected override void Awake()
  {
    base.Awake();

    spriteBg.sprite = _gameSetting.spriteDirectory;
    spriteMask.sprite = _gameSetting.spriteDirectory;

    interactible = false;
    isShowCounter = false;

    StateManager.OnChangeState += SetValue;
  }
  protected override void OnDestroy()
  {
    base.OnDestroy();

    StateManager.OnChangeState -= SetValue;
  }
  #endregion

  public override void ChangeTheme()
  {
    spriteBg.color = _gameManager.Theme.colorPrimary;
  }


  public override void SetValue(DataGame data)
  {
    value = data.activeLevel.openWords.Count;

    base.SetValue(data);
  }


  public override async void OnPointerDown(PointerEventData eventData)
  {
    base.OnPointerDown(eventData);

    _gameManager.InputManager.Disable();
    var dialogWindow = new UIDirectoryOperation();
    var result = await dialogWindow.ShowAndHide();
    _gameManager.InputManager.Enable();
  }


  public override void SetValueProgressBar(DataGame data)
  {
    var newPosition = (progressBasePositionY + 1.2f) + progressBasePositionY - progressBasePositionY * (float)data.activeLevel.openWords.Count / (float)_levelManager.ManagerHiddenWords.AllowPotentialWords.Count;
    spriteProgress.transform
      .DOLocalMoveY(newPosition, _gameSetting.timeGeneralAnimation * 2)
      .SetEase(Ease.OutBounce);
  }

}
