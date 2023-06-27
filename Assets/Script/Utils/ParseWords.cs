using System.Linq;
using UnityEngine;

[System.Serializable]
public class ParseWords : MonoBehaviour
{
  [SerializeField] public TextAsset jsonFile;
  void Start()
  {
    var words = JsonUtility.FromJson<Words>(jsonFile.text);

    GameManager.Instance.InitWords(words);

    GameManager.Instance.Words.data.OrderBy(t => Random.value);

    Debug.Log($"Load {GameManager.Instance.Words.data.Length} words!");
  }
}

[System.Serializable]
public class Words
{
  public string[] data;
}
