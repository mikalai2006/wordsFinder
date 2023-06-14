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
  [SerializeField] private ChoosedWordMB _choosedWordMB;
  [SerializeField] private HiddenWordMB _hiddenWordMB;
  public SerializableDictionary<string, HiddenWordMB> hiddenWords = new SerializableDictionary<string, HiddenWordMB>();
  private string _wordForChars;
  public string WordForChars => _wordForChars;
  public Dictionary<string, int> PotentialWords;
  public Dictionary<string, int> OpenPotentialWords;
  // public Dictionary<string, int> OpenHiddenWords;
  public List<CharMB> listChoosedGameObjects;
  public string choosedWord => string.Join("", listChoosedGameObjects.Select(t => t.GetComponent<CharMB>().charTextValue).ToList());

  private void Awake()
  {

    listChoosedGameObjects = new List<CharMB>();

    PotentialWords = new Dictionary<string, int>();

    OpenPotentialWords = new Dictionary<string, int>();

    // OpenHiddenWords = new Dictionary<string, int>();

    Shuffle.OnShuffleWord += SetWordForChars;
  }

  private void OnDestroy()
  {
    Shuffle.OnShuffleWord -= SetWordForChars;
  }

  public void Init(GameLevel levelConfig, GameLevelWord wordConfig)
  {
    hiddenWords.Clear();

    var data = GameManager.Instance.StateManager.dataGame.activeLevel;

    if (!string.IsNullOrEmpty(data.wordForChars))
    {
      SetWordForChars(data.wordForChars);

      OpenPotentialWords = data.openWords.ToDictionary(t => t, t => 0);

      // OpenHiddenWords = data.openHiddenWords.ToDictionary(t => t, t => 0);

      // foreach (var word in data.hiddenWords)
      // {
      //   var wordGameObject = CreateWord(word);
      //   hiddenWords.Add(word, wordGameObject);
      //   if (OpenHiddenWords.ContainsKey(word))
      //   {
      //     CheckWord(word);
      //   }
      // }
    }
    else
    {
      SetWordForChars(wordConfig.word);
    }

    CreateAllowWords();

    CreateHiddenWords(); //data.potentialWords.ToDictionary(t => t, t => 0);

    // OnChangeData?.Invoke();
  }

  // private Dictionary<string, int> CreatePotentialWords(string word)
  // {
  //   Dictionary<string, int> allowWords = new();
  //   // get word by max length.
  //   // find all allow words.
  //   var potentialWords = GameManager.Instance.Words.data
  //     .Where(t => t.Length <= _gameSetting.GameLevels.Levels[0].maxCountChar)
  //     .OrderBy(t => UnityEngine.Random.value)
  //     .ToList();

  //   // var wordSymbol = potentialWords
  //   //   .OrderByDescending(s => s.Length)
  //   //   .First();

  //   // SetWordForChars(word);

  //   // HashSet<string> arr2Set = new HashSet<string>(_wordForChars.Split(""));

  //   // hiddenWords.Clear();

  //   int countAddWords = 0;
  //   var listWords = new List<string>();
  //   foreach (var word in words)
  //   {
  //     var res = Helpers.IntersectWithRepetitons(_wordForChars, word);
  //     if (res.Count() == word.Length)
  //     {
  //       {
  //         if (countAddWords < _gameSetting.GameLevels.Levels[0].countWord)
  //         {
  //           listWords.Add(word);
  //         }
  //         countAddWords++;
  //         PotentialWords.Add(word, 0);
  //       }
  //     }
  //   }
  //   return allowWords;
  // }

  public void CreateHiddenWords()
  {
    hiddenWords.Clear();
    var listWords = GameManager.Instance.StateManager.dataGame.activeLevel.hiddenWords.Count > 0
      ? GameManager.Instance.StateManager.dataGame.activeLevel.hiddenWords
      : PotentialWords.Keys.ToList().GetRange(0, GameManager.Instance.StateManager.ActiveLevelConfig.countWord);

    listWords = listWords.OrderBy(t => -t.Length).ToList();
    foreach (var word in listWords)
    {
      var wordGameObject = CreateWord(word);
      hiddenWords.Add(word, wordGameObject);
      if (OpenPotentialWords.ContainsKey(word))
      {
        CheckWord(word);
      }
    }
  }

  public void CheckWord(string word)
  {
    if (hiddenWords.ContainsKey(word))
    {
      hiddenWords[word].ShowWord();
    }
  }

  public void CreateAllowWords()
  {
    var potentialWords = GameManager.Instance.Words.data
      .Where(t => t.Length <= WordForChars.Length)
      .OrderBy(t => UnityEngine.Random.value)
      .ToList();

    // HashSet<string> arr2Set = new HashSet<string>(_wordForChars.Split(""));

    foreach (var word in potentialWords)
    {
      var res = Helpers.IntersectWithRepetitons(WordForChars, word);
      if (res.Count() == word.Length)
      {
        PotentialWords.Add(word, 0);
      }
    }
    Debug.Log($"Add {PotentialWords.Count} potential words");
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

  public async UniTask CheckChoosedWord()
  {
    if (hiddenWords.ContainsKey(choosedWord))
    {
      if (OpenPotentialWords.ContainsKey(choosedWord))
      {
        // already open hidden word.
        // await _choosedWordMB.ExistHiddenWord(hiddenWords[choosedWord]);
        await _choosedWordMB.OpenHiddenWord(hiddenWords[choosedWord]);
        // CheckWord(choosedWord);
      }
      else
      {
        // open new hidden word.
        await _choosedWordMB.OpenHiddenWord(hiddenWords[choosedWord]);
        OpenPotentialWords.Add(choosedWord, 1);
        GameManager.Instance.StateManager.AddWord();
      }
    }
    else if (PotentialWords.ContainsKey(choosedWord))
    {
      if (OpenPotentialWords.ContainsKey(choosedWord))
      {
        // already open allow word.
        await _choosedWordMB.OpenPotentialWord(colba);
      }
      else
      {
        // open new allow word.
        await _choosedWordMB.OpenPotentialWord(colba);
        OpenPotentialWords.Add(choosedWord, 1);
        GameManager.Instance.StateManager.AddWord();
      }
    }
    else
    {
      // Debug.Log($"------Not found {choosedWord}");
      await _choosedWordMB.NoWord();
    }

    foreach (var obj in listChoosedGameObjects)
    {
      obj.GetComponent<CharMB>().ResetObject();
    }
    _lineManager.ResetLine();
    listChoosedGameObjects.Clear();
    GameManager.Instance.DataManager.Save();
    if (OpenPotentialWords.Keys.Intersect(hiddenWords.Keys).Count() == hiddenWords.Count())
    {
      await NextLevel();
    }
    // OnChangeData?.Invoke();
  }

  private async UniTask NextLevel()
  {
    PotentialWords.Clear();
    // OpenHiddenWords.Clear();
    OpenPotentialWords.Clear();

    foreach (var wordItem in hiddenWords)
    {
      GameObject.Destroy(wordItem.Value.gameObject);
    }
    hiddenWords.Clear();

    await GameManager.Instance.LevelManager.NextLevel();
  }

  public void AddChoosedChar(CharMB charGameObject)
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
        lastSymbol.GetComponent<CharMB>().ResetObject();
        _lineManager.DrawLine();
      }
    }
    _choosedWordMB.DrawWord(choosedWord);
  }
}
