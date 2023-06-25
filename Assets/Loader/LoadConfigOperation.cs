using System;

using Cysharp.Threading.Tasks;
using UnityEngine.Localization.Settings;

namespace Loader
{
  public class LoadConfigOperation : ILoadingOperation
  {
    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
      onProgress?.Invoke(0.1f);

      GameManager.Instance.ResourceSystem = ResourceSystem.Instance;
      await ResourceSystem.Instance.LoadCollectionsAsset<GameEntity>(Constants.Labels.LABEL_ENTITY);
      onProgress?.Invoke(0.2f);
      GameManager.Instance.ResourceSystem = ResourceSystem.Instance;
      await ResourceSystem.Instance.LoadCollectionsAsset<GameBonus>(Constants.Labels.LABEL_BONUS);

      GameManager.Instance.DataManager.Init();
      var dataGame = await GameManager.Instance.DataManager.Load();
      GameManager.Instance.StateManager.Init(dataGame);

      // Change locale
      var userSetting = GameManager.Instance.StateManager.dataGame.setting;
      if (userSetting != null)
      {
        int indexLocale = LocalizationSettings.AvailableLocales.Locales.FindIndex(t => t.name == userSetting.lang);
        if (userSetting.lang != LocalizationSettings.SelectedLocale.name && indexLocale != -1)
        {
          LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[indexLocale];
        }
      }

      var t = await Helpers.GetLocaledString("loadconfig");
      onSetNotify?.Invoke(t);

      onProgress?.Invoke(.9f);
    }
  }
}