using UnityEngine;

[CreateAssetMenu]
public class GameSetting : ScriptableObject
{
  public GameAudio Audio;
  public GameObject PrefabSymbol;
  public Color colorSymbol;
  public Color colorChooseSymbol;
  public Color colorWordSymbol;
  public Color colorWordSymbolNo;
  public Color colorWordSymbolYes;

  public Sprite bgChar;

  public GameLevels GameLevels;

  [Space(5)]
  [Header("Visual")]
  [Range(0.1f, 1f)] public float lineWidth;
  [Range(0.5f, 3f)] public float radius;

}
