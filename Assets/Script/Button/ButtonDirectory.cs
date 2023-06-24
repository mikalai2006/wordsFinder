using System;
using Cysharp.Threading.Tasks;
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

    spriteBg.color = _gameSetting.Theme.colorPrimary;
  }
  #endregion

  public override async void OnPointerDown(PointerEventData eventData)
  {
    base.OnPointerDown(eventData);

    _gameManager.InputManager.Disable();
    var dialogWindow = new UIDirectoryOperation();
    var result = await dialogWindow.ShowAndHide();
    _gameManager.InputManager.Enable();

  }
}
