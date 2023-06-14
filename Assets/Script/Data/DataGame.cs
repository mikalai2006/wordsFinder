using System.Collections.Generic;

[System.Serializable]
public class DataGame
{
  // public int Level;
  // public List<string> potentialWords;
  // public List<string> openPotentialWords;
  // public List<string> openHiddenWords;
  // public List<string> hiddenWords;
  // public string wordForChars;
  public int rate;
  public List<DataLevel> Levels;
  public string lastActiveLevelId;
  [System.NonSerialized] public DataLevel activeLevel;
  // public DataState dataState;

  public DataGame()
  {
    // potentialWords = new();
    // openHiddenWords = new();
    // openPotentialWords = new();
    // hiddenWords = new();
    Levels = new();
    // dataState = new();
  }
}


// [System.Serializable]
// public struct DataState
// {
//   public int countOpenChars;
//   public int countOpenWords;
//   public int countAllowWords;
//   public int rate;
// }


[System.Serializable]
public class DataLevel
{
  public string id;
  public string idWord;
  // public List<string> potentialWords;
  public List<string> openWords;
  public List<string> hiddenWords;
  // public List<string> openHiddenWords;
  public string wordForChars;
  public int countWords;
  public int countOpenChars;

  public DataLevel()
  {
    openWords = new();
    hiddenWords = new();
    // openHiddenWords = new();
  }
}