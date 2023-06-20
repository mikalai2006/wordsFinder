using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSetting : ScriptableObject
{
  public GameAudio Audio;
  public CharMB PrefabSymbol;

  public GameTheme Theme;

  public List<GamePlayerSetting> PlayerSetting;

  [Space(5)]
  [Header("Game")]
  [Range(0.1f, 1f)] public float lineWidth;
  [Range(0.5f, 3f)] public float radius;

  [Space(5)]
  [Header("Animations")]
  [Range(0.1f, 2f)] public float timeGeneralAnimation;

  [Space(5)]
  [Header("Particle System")]
  public ParticleSystem Boom;
  public ParticleSystem BoomLarge;

  [Space(5)]
  [Header("Assets")]
  public Sprite spriteCoin;
  public Sprite spriteStar;
  public Sprite spriteBomb;
  public Sprite spriteShuffle;
  public Sprite spriteCog;
  public Sprite spriteLevels;
  public Sprite spriteCheck;
  public Sprite spriteHint;
  public Sprite spriteBox;
  public Sprite spriteLighting;
  public Sprite spriteLock;
  public GameObject PrefabCoin;
  public GameObject PrefabBomb;
  public GameObject PrefabStar;
  public GameObject PrefabLighting;
  public Sprite spriteRate;
  public Sprite spriteShop;
  public Sprite spriteCart;

  [Space(5)]
  [Header("System")]
  public int debounceTime;
  [Range(10, 1000)] public int timeDelayOverChar;
  public GameLevel GameLevels;
}
