using Assets;

using Cysharp.Threading.Tasks;


public struct DataResultUIDialog
{
  public bool isOk;
}

public class UIShopOperation : LocalAssetLoader
{
  public UIShopOperation()
  {
  }

  public async UniTask<DataResultUIDialog> ShowAndHide()
  {
    var window = await Load();
    var result = await window.ProcessAction();
    Unload();
    return result;
  }

  public UniTask<UIShop> Load()
  {
    return LoadInternal<UIShop>(Constants.UILabels.UI_SHOP);
  }

  public void Unload()
  {
    UnloadInternal();
  }
}