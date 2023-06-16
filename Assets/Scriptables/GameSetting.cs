using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSetting : ScriptableObject
{
  public GameAudio Audio;
  public CharMB PrefabSymbol;
  public List<GameLevel> GameLevels;

  public GameTheme Theme;

  public GamePlayerSetting PlayerSetting;

  [Space(5)]
  [Header("Game")]
  [Range(0.1f, 1f)] public float lineWidth;
  [Range(0.5f, 3f)] public float radius;

  [Space(5)]
  [Header("Events")]
  [Range(10, 1000)] public int timeDelayOverChar;

}
