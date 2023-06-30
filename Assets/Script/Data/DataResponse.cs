
using System.Collections.Generic;

public struct LeaderBoard
{
  public LeaderBoardInfo leaderboard;
  public int userRank;
  public List<LeaderUser> entries;
}

public struct LeaderBoardInfo
{
  public LeaderBoardInfoTitle title;
}
public struct LeaderUser
{
  public int rank;
  public int score;
  public string lang;
  public string name;
  public string photo;
}

public struct LeaderBoardInfoTitle
{
  public string ru;
  public string en;
}