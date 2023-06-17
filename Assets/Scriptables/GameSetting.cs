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

  [Space(5)]
  [Header("Particle System")]
  public ParticleSystem prefabParticleSystem;

  [Space(5)]
  [Header("Assets")]
  public Sprite coin;
  public Sprite star;
  public Sprite bomb;
  public Sprite shuffle;
  public Sprite spriteCog;
  public Sprite spriteLevels;
  public Sprite spriteCheck;
  public Sprite hint;
  public Sprite box;
  public Sprite lighting;
  public Sprite spriteLock;
}
