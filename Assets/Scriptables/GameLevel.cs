using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameLevel : ScriptableObject
{
  public string id;
  public string title;
  public int countWord;
  public int maxCountChar;

  public List<GameLevelWord> words;
}
