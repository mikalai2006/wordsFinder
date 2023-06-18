using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataGame
{
  public int rate;
  public int coins;
  public List<DataLevel> Levels;
  [System.NonSerialized] public DataLevel activeLevel;
  public string lastActiveLevelId;
  public string lastActiveWordId;

  public DataGame()
  {
    Levels = new();
  }
}


[System.Serializable]
public class DataLevel
{
  public string id;
  public string idWord;
  public int hint;
  public float index;
  public SerializableDictionary<Vector2, string> openChars;
  public SerializeEntity ent;
  public List<string> openWords;
  public List<string> hiddenWords;
  public string wordForChars;
  public int countWords;
  public int countOpenChars;

  public DataLevel()
  {
    openWords = new();
    hiddenWords = new();
    openChars = new();
    ent = new();
  }
}