using System.Collections.Generic;
using UnityEngine;
using User;
// using Login;
using Loader;

public class StartApp : MonoBehaviour
{
  private LoadingScreenProvider loadingProvider => GameManager.Instance.LoadingScreenProvider;

  private async void Start()
  {
    GameManager.Instance.Init();

    var appInfo = new AppInfoContainer();

    var loadingOperations = new Queue<ILoadingOperation>();
    loadingOperations.Enqueue(GameManager.Instance.AssetProvider);
    loadingOperations.Enqueue(new LoginOperation(appInfo));
    loadingOperations.Enqueue(new UIAppOperation());
    GameManager.Instance.AppInfo = appInfo;
    await loadingProvider.LoadAndDestroy(loadingOperations);
    GameManager.Instance.DataManager.Init();
  }
}
