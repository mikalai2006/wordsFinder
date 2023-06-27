using System;

using Cysharp.Threading.Tasks;

namespace Loader
{
  public class InitConfigOperation : ILoadingOperation
  {
    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
      onSetNotify?.Invoke("...");

      onProgress?.Invoke(0.1f);
      GameManager.Instance.ResourceSystem = ResourceSystem.Instance;
      await ResourceSystem.Instance.LoadCollectionsAsset<GameEntity>(Constants.Labels.LABEL_ENTITY);

      onProgress?.Invoke(0.2f);
      GameManager.Instance.ResourceSystem = ResourceSystem.Instance;
      await ResourceSystem.Instance.LoadCollectionsAsset<GameBonus>(Constants.Labels.LABEL_BONUS);

      onProgress?.Invoke(0.3f);
      GameManager.Instance.ResourceSystem = ResourceSystem.Instance;
      await ResourceSystem.Instance.LoadCollectionsAsset<GameTheme>(Constants.Labels.LABEL_THEME);

      UnityEngine.Debug.Log("InitConfigOperation completed!");
    }
  }
}