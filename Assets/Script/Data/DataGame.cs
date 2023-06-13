using System.Collections.Generic;

[System.Serializable]
public class DataGame
{
  public int Level;
  public List<string> potentialWords;
  public List<string> openPotentialWords;
  public List<string> openHiddenWords;
  public List<string> hiddenWords;
  public string wordForChars;
  public DataState dataState;

  public DataGame()
  {
    potentialWords = new();
    openHiddenWords = new();
    openPotentialWords = new();
    hiddenWords = new();
    dataState = new();
  }
}