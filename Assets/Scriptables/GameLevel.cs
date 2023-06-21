using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu]
public class GameLevel : ScriptableObject
{
  public string idLevel;
  public TextLocalize text;
  [Space(10)]
  [Header("---Difficulty settings")]

  [Tooltip("Коэффициент начисления начального количества подсказок: коэф. * количество слов на табло = кол-во подск.")]
  [Range(0f, .3f)] public float coefHint;
  [Tooltip("Коэффициент начисления начального количества подсказок: коэф. * количество слов на табло = кол-во подск.")]
  [Range(0f, .3f)] public float coefStar;

  [Space(10)]
  [Header("Word settings")]
  public List<GameLevelWord> levelWords;
}

[System.Serializable]
public struct TextLocalize
{
  public LocalizedString title;
  public LocalizedString description;
}
