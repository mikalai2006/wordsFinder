using System.Linq;
using UnityEngine;

[System.Serializable]
public class ParseWords : MonoBehaviour
{
  [SerializeField] public TextAsset jsonFile;
  void Start()
  {
    GameManager.Instance.Words = JsonUtility.FromJson<Words>(jsonFile.text);
    GameManager.Instance.Words.data.OrderBy(t => Random.value);
    // foreach (string str in employeesInJson.data)
    // {
    //   if (str.Length <= 3) Debug.Log("Str: " + str);
    // }

    Debug.LogWarning($"Load {GameManager.Instance.Words.data.Length} words!");

    // DataManager.Instance.CreateNeedWords();
    // DataManager.Instance.CreateSymbols();
  }
}

[System.Serializable]
public class Words
{
  public string[] data;

}
