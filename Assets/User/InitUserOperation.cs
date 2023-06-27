using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Loader;
using UnityEngine.Localization.Settings;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace User
{
  public class InitUserOperation : ILoadingOperation
  {
    private AppInfoContainer _result;
    private TaskCompletionSource<string> _getNameCompletionSource;
    private TaskCompletionSource<string> _getPhotoCompletionSource;
    private TaskCompletionSource<AppInfoContainer> _getUserDataCompletionSource;
    private TaskCompletionSource<UserSettings> _getUserSettingCompletionSource;
    private Action<float> _onProgress;
    private Action<string> _onSetNotify;

    // public InitUserOperation(AppInfoContainer appInfoContainer)
    // {
    //   _appInfoContainer = appInfoContainer;
    // }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
      _result = new();

      _onProgress = onProgress;
      _onSetNotify = onSetNotify;

      // var t = await Helpers.GetLocaledString("loading");
      _onSetNotify?.Invoke("...");
      _onProgress?.Invoke(0.3f);

      _result.DeviceId = DeviceInfo.GetDeviceId();

      GameManager.Instance.DataManager.Init(_result.DeviceId);

      // Get user name.
      DataManager.OnLoadName += SetName;
      _onSetNotify?.Invoke("name ...");
      _result.UserInfo.Name = await GetName();
      DataManager.OnLoadName -= SetName;

      // Get user name.
      DataManager.OnLoadPhoto += SetPhoto;
      _onSetNotify?.Invoke("photo ...");
      _result.UserInfo.Photo = await GetPhoto();
      DataManager.OnLoadPhoto -= SetPhoto;

      DataManager.OnLoadData += InitData;
      _onSetNotify?.Invoke("data ...");
      var result = await GetUserData();
      _result.userSettings = result.userSettings;
      DataManager.OnLoadData -= InitData;

      GameManager.Instance.SetAppInfo(_result);

      // _appInfoContainer.UserInfo = res.UserInfo;
      // _appInfoContainer.DeviceId = res.DeviceId;

      // var t = await Helpers.GetLocaledString("loadconfig");
      // onSetNotify?.Invoke(t);

      _onProgress?.Invoke(.5f);
    }

    private void SetPhoto(string photo)
    {
      _result.UserInfo.Photo = photo;
      _getPhotoCompletionSource.SetResult(photo);
    }


    private void SetName(string name)
    {
      _result.UserInfo.Name = name;
      _getNameCompletionSource.SetResult(name);
    }


    public async Task<string> GetName()
    {
      _getNameCompletionSource = new TaskCompletionSource<string>();

#if android
      string name;
      if (PlayerPrefs.HasKey(_result.DeviceId))
      {
        var playerInfo = JsonUtility.FromJson<AppInfoContainer>(PlayerPrefs.GetString(_result.DeviceId));
        name = playerInfo.UserInfo.Name;
      }
      else
      {
        // Load form for input name.
        var result = await GameManager.Instance.InitUserProvider.ShowAndHide();
        name = result.UserInfo.Name;
      }
      SetName(name);
#endif

#if ysdk && !UNITY_EDITOR
      GameManager.Instance.DataManager.LoadNameAsYsdk();
#endif

      return await _getNameCompletionSource.Task;
    }

    public async Task<string> GetPhoto()
    {
      _getPhotoCompletionSource = new TaskCompletionSource<string>();

#if android
      string photo = "";
      if (PlayerPrefs.HasKey(_result.DeviceId))
      {
        var playerInfo = JsonUtility.FromJson<AppInfoContainer>(PlayerPrefs.GetString(_result.DeviceId));
        photo = playerInfo.UserInfo.Photo;
      }
      SetPhoto(photo);
#endif

#if ysdk && !UNITY_EDITOR
      GameManager.Instance.DataManager.LoadPhotoAsYsdk();
#endif

      return await _getPhotoCompletionSource.Task;
    }



    private async UniTask<AppInfoContainer> GetUserData()
    {
      _getUserDataCompletionSource = new();

      //AppInfoContainer result = null;

#if android
      DataGame dataGame;
      dataGame = await GameManager.Instance.DataManager.Load();
      // InitData(dataGame);
#endif

#if ysdk && !UNITY_EDITOR
    GameManager.Instance.DataManager.LoadAsYsdk();
#endif

      return await _getUserDataCompletionSource.Task;
    }

    private void InitData(DataGame dataGame)
    {
      var _gameManager = GameManager.Instance;

      GameManager.Instance.StateManager.Init(dataGame);

      GamePlayerSetting playerSetting;
      if (string.IsNullOrEmpty(dataGame.rank))
      {
        playerSetting = _gameManager.GameSettings.PlayerSetting
          .OrderBy(t => t.countFindWordsForUp)
          .First();
      }
      else
      {
        playerSetting = _gameManager.GameSettings.PlayerSetting.Find(t => t.idPlayerSetting == dataGame.rank);
      }

      // Set player setting.
      _gameManager.PlayerSetting = playerSetting;

      _result.userSettings = dataGame.setting;

      List<GameTheme> allThemes = _gameManager.ResourceSystem.GetAllTheme();

      // Set theme.
      if (!string.IsNullOrEmpty(dataGame.setting.theme))
      {
        GameTheme userTheme = allThemes.Where(t => t.name == dataGame.setting.theme).FirstOrDefault();

        _gameManager.SetTheme(userTheme);
      }

      // Set user setting to PlayPref.
      string jsonData = JsonUtility.ToJson(_result);

      PlayerPrefs.SetString(_result.DeviceId, jsonData);

      Debug.Log($"result=[{_result.ToString()}], jsonData={jsonData}");

      _getUserDataCompletionSource.SetResult(new AppInfoContainer()
      {
        userSettings = dataGame.setting
      });
    }



    //   private async UniTask<AppInfoContainer> LoginAsDeviceId()
    //   {
    //     string deviceId = DeviceInfo.GetDeviceId();
    //     var result = new AppInfoContainer()
    //     {
    //       DeviceId = deviceId
    //     };
    //     await UniTask.Yield();
    //     return result;
    //   }

  }
}