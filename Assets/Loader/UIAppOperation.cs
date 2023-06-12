using System;

using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Loader
{
  public class UIAppOperation : ILoadingOperation
  {
    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {

      await LocalizationSettings.InitializationOperation;

      // var t = new LocalizedString(Constants.LanguageTable.LANG_TABLE_LOCALIZE, "loading").GetLocalizedStringAsync();
      // await t.Task;
      var t = await Helpers.GetLocaleString("loading");
      onSetNotify?.Invoke(t);

      onProgress?.Invoke(0.1f);

      var environment = await GameManager.Instance.AssetProvider.LoadAsset("UIMenuApp");

      if (environment.TryGetComponent(out UIApp component) == false)
        throw new NullReferenceException("Object of type UIApp is null");

      component.Init(environment);

      onProgress?.Invoke(.9f);

    }
  }
}