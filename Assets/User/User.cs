namespace User
{
  [System.Serializable]
  public class AppInfoContainer
  {
    public string DeviceId { get; set; }
    public UserInfo UserInfo { get; set; }
    public UserSettings userSettings { get; set; }
    //TODO config etc
    public AppInfoContainer()
    {
      UserInfo = new UserInfo();
      userSettings = new();
    }

    public override string ToString()
    {
      return string.Format(
        "name={0}\r\nDeviceId={1}",
        UserInfo.Name,
        DeviceId
        );
    }
  }
  // [System.Serializable]
  // public class UserInfoContainer
  // {
  //   public string Id { get; set; }
  //   public string DeviceId { get; set; }
  //   public UserInfoAuth UserInfoAuth;
  //   public UserInfo UserInfo;

  //   public UserInfoContainer()
  //   {
  //     UserInfo = new UserInfo();

  //     UserInfoAuth = new UserInfoAuth();
  //   }
  // }

  [System.Serializable]
  public class UserInfo
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Lang { get; set; }
    public string Login { get; set; }
    public string userId { get; set; }
    public UserInfoAuth UserInfoAuth;
    public UserInfo()
    {

      UserInfoAuth = new UserInfoAuth();
    }

    // public bool IsFacebook => string.IsNullOrWhiteSpace(FacebookId) == false;
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