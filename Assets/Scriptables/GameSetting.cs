using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSetting : ScriptableObject
{
  public GameAudio Audio;
  public CharMB PrefabSymbol;
  public List<GameLevel> GameLevels;

  [Space(5)]
  [Header("Hidden Word")]
  public Color bgHiddenWord;
  public Color bgOpentHiddenWord;
  public Color textOpentHiddenWord;
  public Sprite bgImageHiddenWord;


  [Space(5)]
  [Header("Choosed Word")]
  public Color bgChoosedWord;
  public Color textChoosedWord;
  public Color bgFindAllowWord;
  public Color textFindAllowWord;
  public Color bgFindHiddenWord;
  public Color textFindHiddenWord;
  public Color bgNotFoundWord;
  public Color textNotFoundWord;


  [Space(5)]
  [Header("Chars")]
  public Color colorTextChar;
  public Color bgColorChar;
  public Color colorTextChooseChar;
  public Color bgColorChooseChar;

  [Space(5)]
  [Header("Game")]
  public Color bgColor;
  [Range(0.1f, 1f)] public float lineWidth;
  [Range(0.5f, 3f)] public float radius;

  [Space(5)]
  [Header("Events")]
  [Range(10, 1000)] public int timeDelayOverChar;

}
