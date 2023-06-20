using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameLevelWord : ScriptableObject
{
  public string idLevelWord;
  public string title;
  public string word;
  public int maxCountHiddenChar;

  public List<string> hiddenWords;
}
