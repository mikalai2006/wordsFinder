using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParseWords : MonoBehaviour
{
  [SerializeField] public TextAsset jsonFile;
  void Start()
  {
    GameManager.Instance.Words = JsonUtility.FromJson<Words>(jsonFile.text);
    // foreach (string str in employeesInJson.data)
    // {
    //   if (str.Length <= 3) Debug.Log("Str: " + str);
    // }

    // Debug.Log($"maxLengthWord={DataManager.Instance.maxLengthWord}");

    // DataManager.Instance.CreateNeedWords();
    // DataManager.Instance.CreateSymbols();
  }
}
