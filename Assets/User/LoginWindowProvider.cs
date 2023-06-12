using Assets;
using Cysharp.Threading.Tasks;

namespace User
{
  public class LoginWindowProvider : LocalAssetLoader
  {
    public async UniTask<UserInfoContainer> ShowAndHide()
    {
      var loginWindow = await Load();
      var result = await loginWindow.ProcessLogin();
      Unload();
      return result;
    }

    public UniTask<UILoginWindow> Load()
    {
      return LoadInternal<UILoginWindow>("UILogin");
    }

    public void Unload()
    {
      UnloadInternal();
    }
  }
}