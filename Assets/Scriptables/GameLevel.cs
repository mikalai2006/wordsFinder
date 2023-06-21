using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu]
public class GameLevel : ScriptableObject
{
  public string idLevel;
  public TextLocalize text;

  public List<GameLevelWord> levelWords;
}

[System.Serializable]
public struct TextLocalize
{
  public LocalizedString title;
  public LocalizedString description;
}
