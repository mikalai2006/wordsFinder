
using System.Collections.Generic;

[System.Serializable]
public struct LeaderBoard
{
  public LeaderBoardInfo leaderboard;
  public int userRank;
  public List<LeaderUser> entries;
}

[System.Serializable]
public struct LeaderBoardInfo
{
  public List<LeaderBoardInfoTitle> title;
}

[System.Serializable]
public struct LeaderUser
{
  public int rank;
  public int score;
  public string lang;
  public string name;
  public string photo;
}

[System.Serializable]
public struct LeaderBoardInfoTitle
{
  public string lang;
  public string value;
}