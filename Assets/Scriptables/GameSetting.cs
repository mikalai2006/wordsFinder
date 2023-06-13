using UnityEngine;

[CreateAssetMenu]
public class GameSetting : ScriptableObject
{
  public GameAudio Audio;
  public SymbolMB PrefabSymbol;
  public Color colorSymbol;
  public Color colorChooseSymbol;
  public Color colorWordSymbol;
  public Color colorWordSymbolNo;
  public Color colorWordSymbolYes;

  public Sprite bgChar;

  public GameLevels GameLevels;

  [Space(5)]
  [Header("Visual")]
  public Color bgColor;
  [Range(0.1f, 1f)] public float lineWidth;
  [Range(0.5f, 3f)] public float radius;

  [Space(5)]
  [Header("Events")]
  [Range(10, 1000)] public int timeDelayOverChar;

}
