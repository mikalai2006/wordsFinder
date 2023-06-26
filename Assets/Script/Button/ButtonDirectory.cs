using System;
using UnityEngine.EventSystems;

public class ButtonDirectory : BaseButton
{
  public static event Action<string> OnShuffleWord;

  #region UnityMethods
  protected override void Awake()
  {
    base.Awake();

    spriteBg.sprite = _gameSetting.spriteDirectory;
    spriteMask.sprite = _gameSetting.spriteDirectory;

    interactible = false;
  }
  #endregion

  public override void ChangeTheme()
  {
    spriteBg.color = _gameManager.Theme.colorPrimary;
  }

  public override async void OnPointerDown(PointerEventData eventData)
  {
    base.OnPointerDown(eventData);

    _gameManager.InputManager.Disable();
    var dialogWindow = new UIDirectoryOperation();
    var result = await dialogWindow.ShowAndHide();
    _gameManager.InputManager.Enable();

  }
}
