using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ManagerHiddenWords : MonoBehaviour
{
  [SerializeField] private LineManager _lineManager;
  public Colba colba;
  [SerializeField] private WordMB _wordMB;
  [SerializeField] private HiddenWordMB _hiddenWordMB;
  // public Dictionary<HiddenWordMB, bool> hiddenWordsMB;
  public SerializableDictionary<string, HiddenWordMB> hiddenWords = new SerializableDictionary<string, HiddenWordMB>();
  //   [SerializeField] private int maxCountChars = 8;
  public string wordSymbol;
  public int countHiddenWords = 4;
  public Dictionary<string, int> PotentialWords;
  public Dictionary<string, int> OpenPotentialWords;
  public Dictionary<string, int> OpenHiddenWords;

  public List<SymbolMB> listChoosedGameObjects;
  public string choosedWord => string.Join("", listChoosedGameObjects.Select(t => t.GetComponent<SymbolMB>().charTextValue).ToList());


  private void Awake()
  {

    listChoosedGameObjects = new List<SymbolMB>();

    // hiddenWordsMB = new Dictionary<HiddenWordMB, bool>();

    PotentialWords = new Dictionary<string, int>();

    OpenPotentialWords = new Dictionary<string, int>();

    OpenHiddenWords = new Dictionary<string, int>();
  }

  public void LoadWords(DataPlay data)
  {
    // get word by max length.
    wordSymbol = data.wordSymbol;

    hiddenWords.Clear();

    OpenPotentialWords = data.openPotentialWords.ToDictionary(t => t, t => 0);

    OpenHiddenWords = data.openHiddenWords.ToDictionary(t => t, t => 0);

    PotentialWords = data.potentialWords.ToDictionary(t => t, t => 0);

    foreach (var word in data.hiddenWords)
    {
      var wordGameObject = CreateWord(word);
      hiddenWords.Add(word, wordGameObject);
      if (OpenHiddenWords.ContainsKey(word))
      {
        CheckWord(word);
      }
    }
  }
  public HiddenWordMB CreateWord(string word)
  {
    var newObj = GameObject.Instantiate(
          _hiddenWordMB,
          transform.position,
          Quaternion.identity,
          transform
        );
    // hiddenWordsMB.Add(newObj, false);
    newObj.Init(this);
    newObj.DrawWord(word);

    return newObj;
  }

  public void CreateWords(List<string> potentialWords)
  {
    // get word by max length.
    wordSymbol = potentialWords
    //   .GetRange(0, countWords)
      .OrderByDescending(s => s.Length)
      .First();

    hiddenWords.Clear();

    int countAddWords = 0;
    var listWords = new List<string>();
    foreach (var word in potentialWords)
    {
      if (
        word.ToArray().Intersect(wordSymbol.ToArray()).Count() == word.Length
        &&
        !hiddenWords.ContainsKey(word)
        )
      {
        if (countAddWords < countHiddenWords)
        {
          listWords.Add(word);
        }
        countAddWords++;
        PotentialWords.Add(word, 0);
      }
      // if (countAddWords >= countHiddenWords)
      // {
      //   break;
      // }
    }

    listWords = listWords.OrderBy(t => -t.Length).ToList();
    foreach (var word in listWords)
    {
      var wordGameObject = CreateWord(word);
      hiddenWords.Add(word, wordGameObject);
    }
  }

  public void CheckWord(string word)
  {
    if (hiddenWords.ContainsKey(word))
    {
      hiddenWords[word].ShowWord();
    }
  }


  public async UniTask CheckChoosedWord()
  {
    if (hiddenWords.ContainsKey(choosedWord))
    {
      if (OpenHiddenWords.ContainsKey(choosedWord))
      {
        // Debug.Log($"++++++ReFind potential word {choosedWord}");
        await _wordMB.YesPotentialWord(colba);
      }
      else
      {
        // Debug.Log($"++++++ReFind word {choosedWord}");
        await _wordMB.YesWord(hiddenWords[choosedWord]);
        OpenHiddenWords.Add(choosedWord, 1);
      }
    }
    else if (PotentialWords.ContainsKey(choosedWord))
    {
      if (OpenPotentialWords.ContainsKey(choosedWord))
      {
        // Debug.Log($"++++++ReFind potential word {choosedWord}");
        await _wordMB.YesPotentialWord(colba);
      }
      else
      {
        // Debug.Log($"++++++Find potential word {choosedWord}");
        await _wordMB.YesPotentialWord(colba);
        OpenPotentialWords.Add(choosedWord, 1);
      }
    }
    else
    {
      // Debug.Log($"------Not found {choosedWord}");
      await _wordMB.NoWord();
    }
    foreach (var obj in listChoosedGameObjects)
    {
      obj.GetComponent<SymbolMB>().ResetObject();
    }
    CheckWord(choosedWord);
    _lineManager.ResetLine();
    listChoosedGameObjects.Clear();
    GameManager.Instance.DataManager.Save();
    if (hiddenWords.Count == OpenHiddenWords.Count)
    {
      await NextLevel();
    }
  }

  private async UniTask NextLevel()
  {
    PotentialWords.Clear();
    OpenHiddenWords.Clear();
    OpenPotentialWords.Clear();

    foreach (var wordItem in hiddenWords)
    {
      GameObject.Destroy(wordItem.Value.gameObject);
    }
    hiddenWords.Clear();

    await GameManager.Instance.LevelManager.NextLevel();
  }

  public void AddChoosedChar(SymbolMB charGameObject)
  {
    if (!listChoosedGameObjects.Contains(charGameObject))
    {
      AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.addChar);
      _lineManager.AddPoint(charGameObject.transform.position);
      listChoosedGameObjects.Add(charGameObject);
      _lineManager.DrawLine();
    }
    else
    {
      var lastSymbol = listChoosedGameObjects.ElementAt(listChoosedGameObjects.Count - 1);
      var preLastIndex = listChoosedGameObjects.Count == 1 ? 0 : listChoosedGameObjects.Count - 2;
      var preLastSymbol = listChoosedGameObjects.ElementAt(preLastIndex);
      if (preLastSymbol == charGameObject)
      {
        AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.removeChar);
        listChoosedGameObjects.Remove(lastSymbol);
        _lineManager.RemovePoint(lastSymbol.transform.position);
        lastSymbol.GetComponent<SymbolMB>().ResetObject();
        _lineManager.DrawLine();
      }
    }
    _wordMB.DrawWord(choosedWord);
  }
}
