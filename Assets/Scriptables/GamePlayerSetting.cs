using UnityEngine;

[CreateAssetMenu]
public class GamePlayerSetting : ScriptableObject
{
  public string idPlayerSetting;
  public TextLocalize text;

  [Space(10)]
  [Header("---Difficulty settings")]
  [Space(3)]
  [Tooltip("Коэффициент сложности")]
  [Range(0f, 1f)] public float coefDifficulty;
  [Tooltip("Количество слов для перехода на новый уровень сложности")]
  public int countFindWordsForUp;
  [Tooltip("Максимальная длина слов, которые будут использоваться для табло")]
  public int maxLengthWord;
  // [Tooltip("Максимальная длина слова для набора символов")]
  // public int maxLengthWordForChars;
  // [Tooltip("Минимальная длина слова для набора символов")]
  // public int minLengthWordForChars;
  [Range(5, 200)] public int maxFindWords;
  [Tooltip("Коэффициент начисления начального количества подсказок - частая буква")]
  [Range(0f, .3f)] public float coefHint;
  [Tooltip("Коэффициент начисления начального количества подсказок - случайная буква")]
  [Range(0f, .3f)] public float coefStar;

  [Space(5)]
  [Header("---Bonuses")]
  [Space(3)]
  public BonusCount bonusCount;
}


[System.Serializable]
public struct BonusCount
{
  [Tooltip("Количество найденных символов для получения бонуса")]
  [Range(1, 100)] public int charBonus;
  [Tooltip("Сколько ошибок в угадывании слова - обнуляют бонусный прогресс")]
  [Range(1, 20)] public int errorClear;
  [Tooltip("Количество букв последовательно открытых для добавления доп. коина на поле слов")]
  [Range(1, 100)] public int charCoin;
  [Tooltip("Количество букв последовательно открытых для добавления доп. звезды на поле слов")]
  [Range(1, 100)] public int charStar;
  [Tooltip("Количество букв последовательно открытых для добавления подсказки")]
  [Range(1, 100)] public int charHint;
}