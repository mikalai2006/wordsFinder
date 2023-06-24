using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Loader;

namespace User
{
  public class LoginOperation : ILoadingOperation
  {
    private readonly AppInfoContainer _appInfoContainer;

    private Action<float> _onProgress;
    private Action<string> _onSetNotify;

    public LoginOperation(AppInfoContainer appInfoContainer)
    {
      _appInfoContainer = appInfoContainer;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
      _onProgress = onProgress;
      _onSetNotify = onSetNotify;

      // var t = await Helpers.GetLocaledString("loading");
      _onSetNotify?.Invoke("...");
      _onProgress?.Invoke(0.1f);

      var res = await GetUserInfo(DeviceInfo.GetDeviceId());
      _appInfoContainer.UserInfo = res.UserInfo;
      _appInfoContainer.DeviceId = res.DeviceId;

      _onProgress?.Invoke(.2f);
    }

    private async UniTask<AppInfoContainer> GetUserInfo(string deviceId)
    {
      AppInfoContainer result = null;

      //Fake login
      if (PlayerPrefs.HasKey(deviceId))
      {
        result = JsonUtility.FromJson<AppInfoContainer>(PlayerPrefs.GetString(deviceId));
      }
      // await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
      _onProgress?.Invoke(0.3f);
      //Fake login

      if (result == null || result.UserInfo == null || result.UserInfo.Id == null)
      {
        // result = await GameManager.Instance.LoginWindowProvider.ShowAndHide();
        result = await LoginAsDeviceId();
      }

      // _appInfoContainer.UserInfo = result.UserInfo;
      // _appInfoContainer.DeviceId = deviceId;

      PlayerPrefs.SetString(deviceId, JsonUtility.ToJson(result));

      return result;
    }

    private async UniTask<AppInfoContainer> LoginAsDeviceId()
    {
      string deviceId = DeviceInfo.GetDeviceId();
      var result = new AppInfoContainer()
      {
        DeviceId = deviceId
      };
      await UniTask.Yield();
      return result;
    }

  }
}