using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameLevel : ScriptableObject
{
  public string id;
  public string title;

  public List<GameLevelWord> words;
}
