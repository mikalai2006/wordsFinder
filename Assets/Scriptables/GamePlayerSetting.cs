using UnityEngine;

[CreateAssetMenu]
public class GamePlayerSetting : ScriptableObject
{
  [Space(5)]
  [Header("Player")]
  [Range(10, 100)] public int countCharForAddHint;
  [Range(10, 100)] public int countCharForBonus;
  [Tooltip("Сколько ошибок в угадывании слова - обнуляют бонусный прогресс")]
  [Range(1, 20)] public int countNotFoundForClearCharBonus;
}
