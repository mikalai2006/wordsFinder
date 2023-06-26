using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using User;

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
  public UserSettings setting;

  public DataGame()
  {
    levels = new();
    completed = new();
    setting = new();
    hints = new();
    bonus = new();
  }

  public async UniTask SetDefaultSettings()
  {
    var gameSettings = GameManager.Instance.GameSettings;

    await LocalizationSettings.InitializationOperation.Task;

    setting = new()
    {
      auv = gameSettings.Audio.volumeEffect,
      lang = LocalizationSettings.SelectedLocale.name,
      muv = gameSettings.Audio.volumeMusic,
      theme = gameSettings.ThemeDefault.name,
      td = 50 // time delay
    };
  }
}


[System.Serializable]
public class DataLevel
{
  public string id;
  public float index;
  public int coins;
  public StatePerk statePerk;
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
    statePerk = new();
  }
}