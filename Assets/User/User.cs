namespace User
{
  [System.Serializable]
  public class AppInfoContainer
  {
    public UserInfoContainer UserInfo { get; set; }
    //TODO config etc
    public override string ToString()
    {
      return string.Format(
        "name={0}\r\nDeviceId={1}",
        UserInfo.UserInfo.Name,
        UserInfo.DeviceId
        );
    }
  }
  [System.Serializable]
  public class UserInfoContainer
  {
    public string Id { get; set; }
    public string DeviceId { get; set; }
    public UserInfoAuth UserInfoAuth;
    public UserInfo UserInfo;

    public UserInfoContainer()
    {
      UserInfo = new UserInfo();

      UserInfoAuth = new UserInfoAuth();
    }
  }

  [System.Serializable]
  public class UserInfo
  {
    public string Name { get; set; }
    public string Lang { get; set; }
    public string Login { get; set; }
    public string userId { get; set; }

    // public bool IsFacebook => string.IsNullOrWhiteSpace(FacebookId) == false;
  }

  [System.Serializable]
  public class UserInfoAuth
  {
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
  }
}