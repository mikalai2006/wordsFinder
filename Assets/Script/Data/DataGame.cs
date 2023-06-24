using System.Collections.Generic;
using UnityEngine;
using User;

[System.Serializable]
public class DataGame
{
  public int rate;
  public int coins;
  public int hint;
  public int star;
  public int bomb;
  public int lighting;
  public int word;
  public List<DataLevel> levels;
  public List<string> completeWords;
  [System.NonSerialized] public DataLevel activeLevel;
  public string lastLevelWord;
  public string idPlayerSetting;
  public UserSettings userSettings;

  // public string lastWord;

  public DataGame()
  {
    levels = new();
    completeWords = new();
    userSettings = new();
  }
}


[System.Serializable]
public class DataLevel
{
  public string id;
  public float index;
  public int hintLevel;
  public int starLevel;
  public SerializableDictionary<Vector2, string> openChars;
  public SerializeEntity ent;
  public List<string> openWords;
  public List<string> hiddenWords;
  public string word;
  public int countNeedWords;
  public List<string> needWords;
  public int countOpenChars;
  public int countDopWords;

  public DataLevel()
  {
    openWords = new();
    hiddenWords = new();
    openChars = new();
    ent = new();
  }
}