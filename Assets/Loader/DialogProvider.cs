using Assets;

using Cysharp.Threading.Tasks;

using UnityEngine;


public struct DataDialogResult
{
  public bool isOk;
}

public struct DataDialog
{
  public string headerText;
  public string messageText;
  public Sprite sprite;
  public bool showCancelButton;
}

public class DialogProvider : LocalAssetLoader
{
  private DataDialog _dataDialog;

  public DialogProvider(DataDialog dataDialog)
  {
    _dataDialog = dataDialog;
  }

  public async UniTask<DataDialogResult> ShowAndHide()
  {
    var loginWindow = await Load();
    var result = await loginWindow.ProcessAction(_dataDialog);
    Unload();
    return result;
  }

  public UniTask<UIDialog> Load()
  {
    return LoadInternal<UIDialog>(Constants.UILabels.UI_DIALOG);
  }

  public void Unload()
  {
    UnloadInternal();
  }
}