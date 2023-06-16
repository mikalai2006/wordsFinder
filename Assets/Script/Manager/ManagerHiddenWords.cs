using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ManagerHiddenWords : MonoBehaviour
{
  // public static event Action OnChangeData;
  [SerializeField] private Tilemap _tilemap;
  [SerializeField] private Grid _GridObject;
  public GridHelper GridHelper { get; private set; }
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  [SerializeField] private LineManager _lineManager;
  public Colba colba;
  [SerializeField] private ChoosedWordMB _choosedWordMB;
  [SerializeField] private HiddenWordMB _hiddenWordMB;
  public SerializableDictionary<string, HiddenWordMB> hiddenWords = new SerializableDictionary<string, HiddenWordMB>();
  private string _wordForChars;
  public string WordForChars => _wordForChars;
  public Dictionary<string, int> AllowWords;
  public Dictionary<string, int> OpenWords;
  public List<CharMB> listChoosedGameObjects;
  public string choosedWord => string.Join("", listChoosedGameObjects.Select(t => t.GetComponent<CharMB>().charTextValue).ToList());


  private void Awake()
  {

    listChoosedGameObjects = new List<CharMB>();

    AllowWords = new Dictionary<string, int>();

    OpenWords = new Dictionary<string, int>();

    Shuffle.OnShuffleWord += SetWordForChars;
  }

  private void OnDestroy()
  {
    Shuffle.OnShuffleWord -= SetWordForChars;
  }

  /// <summary>
  /// Init level
  /// </summary>
  /// <param name="levelConfig">Config level</param>
  /// <param name="wordConfig">Config word</param>
  public void Init(GameLevel levelConfig, GameLevelWord wordConfig)
  {
    hiddenWords.Clear();

    var data = GameManager.Instance.StateManager.dataGame.activeLevel;

    if (!string.IsNullOrEmpty(data.wordForChars))
    {
      SetWordForChars(data.wordForChars);

      OpenWords = data.openWords.ToDictionary(t => t, t => 0);
    }
    else
    {
      SetWordForChars(wordConfig.word);
    }

    CreateAllowWords();

    List<string> _hiddenWords = new();
    if (!string.IsNullOrEmpty(data.wordForChars))
    {
      _hiddenWords = data.hiddenWords;
    }
    else
    {
      _hiddenWords = CreateHiddenWords();
    }

    SetScaleChars(_hiddenWords);

    CreateGameObjectHiddenWords(_hiddenWords);
    // OnChangeData?.Invoke();

    OpenNeighbours().Forget();
  }

  public void CreateGameObjectHiddenWords(List<string> words)
  {
    hiddenWords.Clear();

    // var stateManager = GameManager.Instance.StateManager;
    // var listWords = stateManager.ActiveWordConfig.hiddenWords;

    var listWords = words; //.OrderBy(t => -t.Length).ToList();
    for (int i = 0; i < listWords.Count; i++)
    {
      var wordGameObject = CreateWord(listWords[i], i);
      // hiddenWords.Add(listWords[i], wordGameObject);
      hiddenWords[listWords[i]] = wordGameObject;
      if (OpenWords.ContainsKey(listWords[i]))
      {
        CheckWord(listWords[i]);
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

  public List<string> CreateHiddenWords()
  {
    List<string> hiddenWords = new();
    var stateManager = GameManager.Instance.StateManager;

    int countChar = 0;
    foreach (var word in AllowWords)
    {
      if (countChar > stateManager.ActiveWordConfig.maxCountHiddenChar) break;
      if (OpenWords.ContainsKey(word.Key)) continue;

      var newCountChar = countChar + word.Key.Length;
      if (newCountChar < stateManager.ActiveWordConfig.maxCountHiddenChar)
      {
        hiddenWords.Add(word.Key);
        countChar += word.Key.Length;
      }
    }

    // Debug.Log($"Add {AllowWords.Count} potential words");
    return hiddenWords;
  }

  public void CreateAllowWords()
  {
    var potentialWords = GameManager.Instance.Words.data
      .Where(t => t.Length <= WordForChars.Length)
      .OrderBy(t => UnityEngine.Random.value)
      .ToList();

    foreach (var word in potentialWords)
    {
      var res = Helpers.IntersectWithRepetitons(WordForChars, word);
      if (res.Count() == word.Length)
      {
        AllowWords.Add(word, 0);
      }
    }
    // Debug.Log($"Add {AllowWords.Count} potential words");
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

  public HiddenWordMB CreateWord(string word, int index)
  {
    // find node for spawn word.
    var nodes = GridHelper.FindNodeForSpawnWord(word, index);

    var newObj = GameObject.Instantiate(
          _hiddenWordMB,
          nodes[0].position,
          Quaternion.identity,
          _tilemap.transform
        );
    newObj.transform.localPosition = new Vector3(nodes[0].x, nodes[0].y);
    // hiddenWordsMB.Add(newObj, false);
    newObj.Init(this);
    newObj.DrawWord(word, nodes);
    // node.SetOccupiedChar();

    return newObj;
  }

  public async UniTask CheckChoosedWord()
  {
    if (hiddenWords.ContainsKey(choosedWord))
    {
      if (OpenWords.ContainsKey(choosedWord))
      {
        // already open hidden word.
        await _choosedWordMB.ExistHiddenWord(hiddenWords[choosedWord]);
        // CheckWord(choosedWord);
      }
      else
      {
        // open new hidden word.
        await _choosedWordMB.OpenHiddenWord(hiddenWords[choosedWord]);
        OpenWords.Add(choosedWord, 1);
        GameManager.Instance.StateManager.AddWord();
      }
    }
    else if (AllowWords.ContainsKey(choosedWord))
    {
      if (OpenWords.ContainsKey(choosedWord))
      {
        // already open allow word.
        // await _choosedWordMB.OpenAllowWord(colba);
        await _choosedWordMB.ExistAllowWord();
      }
      else
      {
        // open new allow word.
        await _choosedWordMB.OpenAllowWord(colba);
        OpenWords.Add(choosedWord, 1);
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

    bool isEndLevel = OpenWords.Count == AllowWords.Count;
    bool isOpenAllHiddenWords = OpenWords.Keys.Intersect(hiddenWords.Keys).Count() == hiddenWords.Count();
    if (isEndLevel)
    {
      Debug.Log("Next level");
      await NextLevel();
    }
    else if (isOpenAllHiddenWords)
    {
      Debug.Log("Refresh hiddenWords");
      RefreshHiddenWords();
    }
    // OnChangeData?.Invoke();
  }


  private void RefreshHiddenWords()
  {
    foreach (var wordItem in hiddenWords)
    {
      GameObject.Destroy(wordItem.Value.gameObject);
    }

    hiddenWords.Clear();

    var _hiddenWords = CreateHiddenWords();

    SetScaleChars(_hiddenWords);

    CreateGameObjectHiddenWords(_hiddenWords);
  }


  private async UniTask NextLevel()
  {
    AllowWords.Clear();
    // OpenHiddenWords.Clear();
    OpenWords.Clear();

    await GameManager.Instance.LevelManager.NextLevel();
  }


  public async UniTask OpenNeighbours()
  {
    // open equals chars.
    List<GridNode> equalsCharNodes = GameManager.Instance.LevelManager.ManagerHiddenWords.GridHelper.FindEqualsHiddenNeighbours();
    Debug.Log($"equalsCharNodes count={equalsCharNodes.Count}");
    foreach (var equalCharNode in equalsCharNodes)
    {
      await equalCharNode.OccupiedChar.ShowChar();
    }
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

  private void SetScaleChars(List<string> _hiddenWords)
  {
    var defaultGridSize = 9;
    var defaultCountChars = System.Math.Pow(defaultGridSize, 2);
    var stateManager = GameManager.Instance.StateManager;

    // calculate count char of by words.
    // var sortedListHiddenWords = _hiddenWords.OrderBy(t => t.Length);
    // int countChars = sortedListHiddenWords.OrderBy(t => t.Length).Select(t => t.Length).Sum();
    // countChars += sortedListHiddenWords.Count();
    // string wordWithMaxLength = sortedListHiddenWords.Last();
    // int maxLengthWord = wordWithMaxLength.Length;
    // if (maxLengthWord < defaultGridSize) maxLengthWord = defaultGridSize;
    // int minNeedRows = (int)System.Math.Ceiling((double)countChars / maxLengthWord);

    var sizeGridXY = defaultGridSize;

    if (stateManager.ActiveWordConfig.maxCountHiddenChar > defaultCountChars)
    {
      sizeGridXY = System.Convert.ToInt32(System.MathF.Ceiling((float)System.Math.Sqrt(stateManager.ActiveWordConfig.maxCountHiddenChar)));
      sizeGridXY += 2;
    }

    // Debug.Log($"countWord={sortedListHiddenWords.Count()}, countChars ={countChars}, maxLengthWord={maxLengthWord}");
    // Debug.Log($"word max length={wordWithMaxLength}, Need count rows={minNeedRows}, sizeGridXY={sizeGridXY}");
    GridHelper = new GridHelper(sizeGridXY, sizeGridXY);
    // Set transform grid.
    float scale = (float)defaultGridSize / sizeGridXY;
    // Debug.Log($"Scale grid ={scale}");
    _GridObject.transform.localScale = new Vector3(scale, scale, 1);

  }

  public void Reset()
  {
    foreach (var wordItem in hiddenWords)
    {
      GameObject.Destroy(wordItem.Value.gameObject);
    }

    hiddenWords.Clear();

    AllowWords.Clear();
    // OpenHiddenWords.Clear();
    OpenWords.Clear();
  }
}
