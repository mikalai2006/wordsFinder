using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GamePlayerSetting : ScriptableObject
{
  [Space(5)]
  [Header("Player")]
  [Range(10, 100)] public int countCharForAddHint;
  [Range(10, 100)] public int countCharForBonus;
}
