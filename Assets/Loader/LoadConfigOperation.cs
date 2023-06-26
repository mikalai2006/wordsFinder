using System;

using Cysharp.Threading.Tasks;
using UnityEngine.Localization.Settings;

namespace Loader
{
  public class LoadConfigOperation : ILoadingOperation
  {
    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
      UnityEngine.Debug.Log($"LoadConfigOperation");
      onProgress?.Invoke(0.1f);

      GameManager.Instance.ResourceSystem = ResourceSystem.Instance;
      await ResourceSystem.Instance.LoadCollectionsAsset<GameEntity>(Constants.Labels.LABEL_ENTITY);
      onProgress?.Invoke(0.2f);
      GameManager.Instance.ResourceSystem = ResourceSystem.Instance;
      await ResourceSystem.Instance.LoadCollectionsAsset<GameBonus>(Constants.Labels.LABEL_BONUS);
      onProgress?.Invoke(0.3f);
      GameManager.Instance.ResourceSystem = ResourceSystem.Instance;
      await ResourceSystem.Instance.LoadCollectionsAsset<GameTheme>(Constants.Labels.LABEL_THEME);

      UnityEngine.Debug.Log($"LoadConfigOperation Ok1");
      GameManager.Instance.DataManager.Init();
      UnityEngine.Debug.Log($"LoadConfigOperation Ok2");
      var dataGame = await GameManager.Instance.DataManager.Load();
      GameManager.Instance.StateManager.Init(dataGame);

      UnityEngine.Debug.Log($"LoadConfigOperation Ok3");

      // Change locale
      await LocalizationSettings.InitializationOperation.Task;

      UnityEngine.Debug.Log($"LoadConfigOperation Ok4");
      var userSetting = GameManager.Instance.StateManager.dataGame.setting;
      if (userSetting != null)
      {
        int indexLocale = LocalizationSettings.AvailableLocales.Locales.FindIndex(t => t.name == userSetting.lang);
        if (userSetting.lang != LocalizationSettings.SelectedLocale.name && indexLocale != -1)
        {
          LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[indexLocale];
        }
      }
      UnityEngine.Debug.Log($"LoadConfigOperation Ok5");

      var t = await Helpers.GetLocaledString("loadconfig");
      onSetNotify?.Invoke(t);

      onProgress?.Invoke(.9f);
    }
  }
}