using System;

using Cysharp.Threading.Tasks;

namespace Loader
{
  public class LoadConfigOperation : ILoadingOperation
  {
    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
      onProgress?.Invoke(0.5f);

      var t = await Helpers.GetLocaleString("loadconfig");
      onSetNotify?.Invoke(t);


      GameManager.Instance.DataManager.Init();
      var dataGame = GameManager.Instance.DataManager.Load();
      GameManager.Instance.StateManager.LoadState(dataGame);

      onProgress?.Invoke(.9f);
    }
  }
}