using System.Collections.Generic;

[System.Serializable]
public class DataPlay
{
  public int Level;
  public List<string> potentialWords;
  public List<string> openPotentialWords;
  public List<string> openHiddenWords;
  public List<string> hiddenWords;
  public string wordSymbol;

  public DataPlay()
  {
    potentialWords = new();
    openHiddenWords = new();
    openPotentialWords = new();
    hiddenWords = new();
  }
}
