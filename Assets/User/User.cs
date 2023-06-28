namespace User
{
  [System.Serializable]
  public class AppInfoContainer
  {
    public string uid;
    public UserInfo UserInfo;
    public UserSettings setting;

    public AppInfoContainer()
    {
      UserInfo = new UserInfo();
      setting = new();
    }

    public override string ToString()
    {
      return string.Format(
        "name={0}\r\nDeviceId={1}",
        UserInfo.name,
        uid
        );
    }
  }

  [System.Serializable]
  public class UserInfo
  {
    // public string Id { get; set; }
    // public string Name { get; set; }
    // public string Lang { get; set; }
    public string name;
    public string photo;
    // public string userId { get; set; }
    public UserInfoAuth UserInfoAuth;
    public UserInfo()
    {

      // UserInfoAuth = new UserInfoAuth();
    }

  }

  [System.Serializable]
  public class UserInfoAuth
  {
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
  }


  [System.Serializable]
  public class UserSettings
  {
    public string lang;
    public float muv;
    public float auv;
    public int td;
    public string theme;
  }
}