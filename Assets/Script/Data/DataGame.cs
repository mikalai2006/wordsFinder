using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataGame
{
  public int rate;
  public int coins;
  public SerializableDictionary<TypeEntity, int> hints;
  public SerializableDictionary<TypeBonus, int> bonus;
  public List<DataLevel> levels;
  public List<string> completed;
  [System.NonSerialized] public DataLevel activeLevel;
  public string lastWord;
  public string rank;

  public DataGame()
  {
    levels = new();
    completed = new();
    hints = new();
    bonus = new();
  }
}


[System.Serializable]
public class DataLevel
{
  public string id;
  public float index;
  public int coins;
  public BonusCount bonusCount;
  public SerializableDictionary<Vector2, string> openChars;
  public SerializableDictionary<TypeEntity, int> hints; // [System.NonSerialized]
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
    hints = new();
    bonusCount = new();
  }
}