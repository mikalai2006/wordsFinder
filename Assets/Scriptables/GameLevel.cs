using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameLevel : ScriptableObject
{
  public string id;
  public string title;
  public int minRate;
  public int countHint;
  public int countStar;

  public List<GameLevelWord> words;
}
