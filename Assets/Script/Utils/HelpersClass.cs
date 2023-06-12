using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

/// <summary>
/// A static class for general helpful methods
/// </summary>
public static class Helpers
{
  /// <summary>
  /// Destroy all child objects of this transform (Unintentionally evil sounding).
  /// Use it like so:
  /// <code>
  /// transform.DestroyChildren();
  /// </code>
  /// </summary>
  public static void DestroyChildren(this Transform t)
  {
    foreach (Transform child in t) UnityEngine.Object.Destroy(child.gameObject);
  }

  public static int GenerateChance(int start = 0, int end = 100)
  {
    System.Random random = new System.Random();
    return random.Next(0, 100);
  }

  public static Dictionary<Transform, dynamic> GetChildrenHierarchy(this GameObject gameobject)
  {
    Dictionary<Transform, dynamic> children = new Dictionary<Transform, dynamic>();

    foreach (Transform child in gameobject.transform)
    {
      children.Add(child, GetChildrenHierarchy(child.gameObject));
    }

    return children;
  }

  /// <summary>
  /// Clone Dictionary
  /// </summary>
  /// <param name="original"></param>
  /// <typeparam name="TKey"></typeparam>
  /// <typeparam name="TValue"></typeparam>
  /// <returns></returns>
  public static Dictionary<TKey, TValue> CloneDictionaryCloningValues<TKey, TValue>
 (Dictionary<TKey, TValue> original) where TValue : ICloneable
  {
    Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count,
                                                            original.Comparer);
    foreach (KeyValuePair<TKey, TValue> entry in original)
    {
      ret.Add(entry.Key, (TValue)entry.Value.Clone());
    }
    return ret;
  }

  // public static string GetLocalizedPluralString<T>(
  //     LocalizedString localizedString,
  //     Dictionary<string, T>[] args,
  //     Dictionary<string, T> dictionary
  //     )
  // {
  //     if (localizedString.IsEmpty) return "NO_LANG";

  //     localizedString.Arguments = args;
  //     return localizedString.GetLocalizedString(dictionary);
  // }

  // public static string GetNameByValue(LocalizedString localizedString, int value)
  // {
  //     var data = new Dictionary<string, int> {
  //         { "value", value },
  //     };
  //     var arguments = new[] { data };
  //     return Helpers.GetLocalizedPluralString(
  //         localizedString,
  //         arguments,
  //         data
  //         );
  // }
  public static void LineConnection(LineRenderer lr, GameObject first, GameObject second, float linewidth)
  {
    // var lr = first.GetComponent<LineRenderer>();
    lr.SetPosition(0, first.gameObject.transform.position);
    lr.SetPosition(1, second.gameObject.transform.position);
    lr.startWidth = linewidth;
    lr.endWidth = linewidth;
  }
  public static string GetColorString(string str)
  {
    return " <color=#FFFFAB>" + str + "</color>";
  }
  public async static UniTask<string> GetLocaleString(string key)
  {
    var t = new LocalizedString(Constants.LanguageTable.LANG_TABLE_LOCALIZE, key).GetLocalizedStringAsync();
    await t.Task;
    return t.Result;
  }
}

// [System.Serializable]
// public struct ItemProbabiliti<T>
// {
//     public T Item;
//     [Range(0, 1)] public double probability;
// }

// [System.Serializable]
// public struct ResultProbabiliti<T>
// {
//     public T Item;
//     public int index;
// }