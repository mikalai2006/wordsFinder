using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu]
public class GameLevel : ScriptableObject
{
  public string idLevel;
  public TextLevel text;

  public List<GameLevelWord> levelWords;
}

[System.Serializable]
public struct TextLevel
{
  public LocalizedString title;
  public LocalizedString description;
}
