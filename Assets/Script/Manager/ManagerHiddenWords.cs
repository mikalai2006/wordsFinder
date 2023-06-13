using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ManagerHiddenWords : MonoBehaviour
{
  // public static event Action OnChangeData;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  [SerializeField] private LineManager _lineManager;
  public Colba colba;
  [SerializeField] private WordMB _wordMB;
  [SerializeField] private HiddenWordMB _hiddenWordMB;
  public SerializableDictionary<string, HiddenWordMB> hiddenWords = new SerializableDictionary<string, HiddenWordMB>();
  private string _wordForChars;
  public string WordForChars => _wordForChars;
  public Dictionary<string, int> PotentialWords;
  public Dictionary<string, int> OpenPotentialWords;
  public Dictionary<string, int> OpenHiddenWords;
  public List<SymbolMB> listChoosedGameObjects;
  public string choosedWord => string.Join("", listChoosedGameObjects.Select(t => t.GetComponent<SymbolMB>().charTextValue).ToList());

  private void Awake()
  {

    listChoosedGameObjects = new List<SymbolMB>();

    PotentialWords = new Dictionary<string, int>();

    OpenPotentialWords = new Dictionary<string, int>();

    OpenHiddenWords = new Dictionary<string, int>();

    Shuffle.OnShuffleWord += SetWordForChars;
  }

  private void OnDestroy()
  {
    Shuffle.OnShuffleWord -= SetWordForChars;
  }

  public void LoadWords(DataGame data)
  {
    SetWordForChars(data.wordForChars);

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
    // OnChangeData?.Invoke();
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

  /// <summary>
  /// Set word for radial word.
  /// </summary>
  /// <param name="word">Word</param>
  public void SetWordForChars(string word)
  {
    // get word by max length.
    _wordForChars = word;
  }

  public void CreateWords(List<string> potentialWords)
  {
#if UNITY_EDITOR
    System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
    stopWatch.Start();
#endif
    // get word by max length.
    var wordSymbol = potentialWords
      .OrderByDescending(s => s.Length)
      .First();
    SetWordForChars(wordSymbol);

    HashSet<string> arr2Set = new HashSet<string>(_wordForChars.Split(""));

    hiddenWords.Clear();

    int countAddWords = 0;
    var listWords = new List<string>();
    foreach (var word in potentialWords)
    {
      var res = Helpers.IntersectWithRepetitons(_wordForChars, word);
      if (res.Count() == word.Length)
      {
        {
          if (countAddWords < _gameSetting.GameLevels.Levels[0].countWord)
          {
            listWords.Add(word);
          }
          countAddWords++;
          PotentialWords.Add(word, 0);
        }
      }
    }
    listWords = listWords.OrderBy(t => -t.Length).ToList();
    foreach (var word in listWords)
    {
      var wordGameObject = CreateWord(word);
      hiddenWords.Add(word, wordGameObject);
    }
#if UNITY_EDITOR
    stopWatch.Stop();
    System.TimeSpan timeTaken = stopWatch.Elapsed;
    Debug.LogWarning($"Time Generation Step::: {timeTaken.ToString(@"m\:ss\.ffff")}");
#endif
    // OnChangeData?.Invoke();
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
        // already open hidden word.
        await _wordMB.ExistHiddenWord(hiddenWords[choosedWord]);
      }
      else
      {
        // open new hidden word.
        await _wordMB.openWord(hiddenWords[choosedWord]);
        OpenHiddenWords.Add(choosedWord, 1);
        GameManager.Instance.StateManager.AddWord();
      }
    }
    else if (PotentialWords.ContainsKey(choosedWord))
    {
      if (OpenPotentialWords.ContainsKey(choosedWord))
      {
        // already open allow word.
        await _wordMB.OpenPotentialWord(colba);
      }
      else
      {
        // open new allow word.
        await _wordMB.OpenPotentialWord(colba);
        OpenPotentialWords.Add(choosedWord, 1);
        GameManager.Instance.StateManager.AddWord();
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
    // OnChangeData?.Invoke();
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
