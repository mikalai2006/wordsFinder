using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameLevelWord : ScriptableObject
{
  public string id;
  public string title;
  public string word;

  public List<string> hiddenWords;
}
