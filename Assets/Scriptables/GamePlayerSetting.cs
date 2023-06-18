using UnityEngine;

[CreateAssetMenu]
public class GamePlayerSetting : ScriptableObject
{
  [Space(5)]
  [Header("Player")]
  public BonusCount bonusCount;
}


[System.Serializable]
public struct BonusCount
{
  [Range(10, 100)] public int charHint;
  [Range(10, 100)] public int charBonus;
  [Tooltip("Сколько ошибок в угадывании слова - обнуляют бонусный прогресс")]
  [Range(1, 20)] public int errorClear;
  [Tooltip("Количество букв последовательно открытых для добавления доп. коина на поле слов")]
  [Range(1, 100)] public int charCoin;
  [Tooltip("Количество букв последовательно открытых для добавления доп. звезды на поле слов")]
  [Range(1, 100)] public int charStar;
}