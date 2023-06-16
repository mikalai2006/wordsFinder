using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameTheme : ScriptableObject
{
  [Space(5)]
  [Header("Hidden Word")]
  public Color bgHiddenWord;
  public Color bgOpentHiddenWord;
  public Color textOpentHiddenWord;
  public Sprite bgImageHiddenWord;
  public Color bgOpenNeiHiddenWord;
  public Color textOpenNeiHiddenWord;


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
  public Sprite bgImageChar;
  public Sprite bgImageCharChoose;

  [Space(5)]
  [Header("Game")]
  public Color bgColor;
  public Color colorLine;

}
