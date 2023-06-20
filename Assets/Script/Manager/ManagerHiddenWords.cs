using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ManagerHiddenWords : MonoBehaviour
{
  // public static event Action OnChangeData;
  [SerializeField] public Tilemap tilemap;
  [SerializeField] public Tilemap tilemapEntities;
  [SerializeField] private Grid _GridObject;
  public GridHelper GridHelper { get; private set; }
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  private StateManager _stateManager => GameManager.Instance.StateManager;
  private LevelManager _levelManager => GameManager.Instance.LevelManager;
  protected GameManager _gameManager => GameManager.Instance;
  [SerializeField] private LineManager _lineManager;

  [SerializeField] private ChoosedWordMB _choosedWordMB;
  [SerializeField] private HiddenWordMB _hiddenWordMB;
  public SerializableDictionary<string, HiddenWordMB> HiddenWords = new SerializableDictionary<string, HiddenWordMB>();
  private string _wordForChars;
  public string WordForChars => _wordForChars;
  public Dictionary<string, int> NeedWords;
  public Dictionary<string, int> AllowPotentialWords;
  public Dictionary<string, int> OpenWords;
  public Dictionary<string, int> OpenNeedWords;
  public List<CharMB> listChoosedGameObjects;
  public string choosedWord => string.Join("", listChoosedGameObjects.Select(t => t.GetComponent<CharMB>().charTextValue).ToList());

  public SerializableDictionary<Vector2, string> OpenChars = new();
  public SerializeEntity Entities = new();
  // public List<GameObject> EntitiesGameObjects = new();

  public float scaleGrid;

  private void Awake()
  {

    listChoosedGameObjects = new();

    NeedWords = new();

    OpenWords = new();

    AllowPotentialWords = new();

    OpenNeedWords = new();

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
  public void Init() // GameLevel levelConfig, GameLevelWord wordConfig
  {
    var wordConfig = _stateManager.ActiveWordConfig;
    var word = wordConfig.word;

    _levelManager.shuffle.gameObject.SetActive(true);
    _levelManager.colba.gameObject.SetActive(true);
    _levelManager.hint.gameObject.SetActive(true);
    HiddenWords.Clear();

    var data = _stateManager.dataGame.activeLevelWord;

    if (!string.IsNullOrEmpty(data.word))
    {
      SetWordForChars(data.word);

      OpenWords = data.openWords.ToDictionary(t => t, t => 0);

      foreach (var item in data.openChars)
      {
        OpenChars.Add(item.Key, item.Value);
      }
      foreach (var item in data.ent)
      {
        Entities.Add(item.Key, item.Value);
      }
    }
    else
    {
      SetWordForChars(word);
    }

    CreateAllowWords();

    List<string> _hiddenWords = new();
    if (!string.IsNullOrEmpty(data.word))
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
    OpenOpenedChars();

    // OpenNeighbours().Forget();

    // Create entities.
    var keysEntity = Entities.Keys.ToList();
    foreach (var item in keysEntity)
    {
      AddEntity(item, Entities[item]);
    }
  }


  public BaseEntity AddEntity(Vector2Int pos, TypeEntity typeEntity)
  {
    var node = pos == Vector2Int.zero
      ? GridHelper.GetRandomNodeWithChar()
      : GridHelper.GetNode(pos);
    GameObject prefab = _gameSetting.PrefabStar;
    switch (typeEntity)
    {
      case TypeEntity.Bomb:
        prefab = _gameSetting.PrefabBomb;
        break;
      case TypeEntity.Lighting:
        prefab = _gameSetting.PrefabLighting;
        break;
      case TypeEntity.Coin:
        prefab = _gameSetting.PrefabCoin;
        break;
    }
    var newObj = GameObject.Instantiate(
          prefab,
          node.position,
          Quaternion.identity,
          tilemapEntities.transform
        );

    var newEntity = newObj.GetComponent<BaseEntity>();
    newEntity.Init(node);
    node.StateNode |= StateNode.Entity;
    if (!Entities.ContainsKey(node.arrKey))
    {
      Entities.Add(node.arrKey, typeEntity);
    }

    // GameManager.Instance.DataManager.Save();
    _stateManager.RefreshData();

    return newEntity;
  }

  public void RemoveEntity(BaseEntity entity)
  {
    if (Entities.ContainsKey(entity.OccupiedNode.arrKey))
    {
      Entities.Remove(entity.OccupiedNode.arrKey);
      entity.OccupiedNode.SetOccupiedEntity(null);
    }

    // GameManager.Instance.DataManager.Save();
    _stateManager.RefreshData();
  }

  public void OpenOpenedChars()
  {
    for (int i = 0; i < OpenChars.Count; i++)
    {
      var item = OpenChars.ElementAt(i);
      var node = GridHelper.GetNode(item.Key);
      node.OccupiedChar.ShowCharAsNei(false).Forget();
      node.SetHint();
    }
  }

  public void AddOpenChar(HiddenCharMB occupiedChar)
  {
    OpenChars.Add(occupiedChar.OccupiedNode.arrKey, occupiedChar.charTextValue.ToString());
  }

  public void RemoveOpenChar(HiddenCharMB occupiedChar)
  {
    OpenChars.Remove(occupiedChar.OccupiedNode.arrKey);
  }

  public void CreateGameObjectHiddenWords(List<string> words)
  {
    HiddenWords.Clear();

    var listWords = words; //.OrderBy(t => -t.Length).ToList();
    for (int i = 0; i < listWords.Count; i++)
    {
      var wordGameObject = CreateWord(listWords[i], i);
      // hiddenWords.Add(listWords[i], wordGameObject);
      HiddenWords[listWords[i]] = wordGameObject;
      if (OpenWords.ContainsKey(listWords[i]))
      {
        CheckWord(listWords[i]);
      }
    }
  }

  public void CheckWord(string word)
  {
    if (HiddenWords.ContainsKey(word))
    {
      HiddenWords[word].ShowWord().Forget();
    }
  }

  public List<string> CreateHiddenWords()
  {
    List<string> hiddenWords = new();

    int countChar = 0;
    foreach (var word in NeedWords)
    {
      if (countChar > _stateManager.ActiveWordConfig.maxCountHiddenChar) break;
      if (OpenWords.ContainsKey(word.Key)) continue;

      var newCountChar = countChar + word.Key.Length;
      if (newCountChar < _stateManager.ActiveWordConfig.maxCountHiddenChar)
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
    var potentialWords = _gameManager.Words.data
        .Where(t => t.Length <= WordForChars.Length)
        .OrderBy(t => Random.value)
        .ToList();

    var maxCountWords = _stateManager.ActiveWordConfig.maxNeedFindWords > potentialWords.Count
      ? potentialWords.Count
      : _stateManager.ActiveWordConfig.maxNeedFindWords;

    var savedAllowWords = _stateManager.dataGame.activeLevelWord.allowWords;

    foreach (var word in potentialWords)
    {
      var res = Helpers.IntersectWithRepetitons(WordForChars, word);

      if (res.Count() == word.Length)
      {
        if (NeedWords.Count < maxCountWords && savedAllowWords == null)
        {
          NeedWords.Add(word, 0);
        }
        AllowPotentialWords.Add(word, 0);
      }
    }

    if (savedAllowWords != null)
    {
      NeedWords = savedAllowWords.ToDictionary(t => t, t => 0);

      foreach (var openWord in NeedWords)
      {
        if (OpenWords.ContainsKey(openWord.Key))
        {
          OpenNeedWords.Add(openWord.Key, 0);
        }
      }
    }
    // Debug.LogWarning($"Add {AllowWords.Count} potential words ({WordForChars}) [maxCountWords={maxCountWords}]");
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
          tilemap.transform
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
    if (HiddenWords.ContainsKey(choosedWord))
    {
      if (OpenWords.ContainsKey(choosedWord))
      {
        // already open hidden word.
        await _choosedWordMB.ExistHiddenWord(HiddenWords[choosedWord]);
        // CheckWord(choosedWord);
      }
      else
      {
        // open new hidden word.
        OpenWords.Add(choosedWord, 1);
        OpenNeedWords.Add(choosedWord, 1);
        await _choosedWordMB.OpenHiddenWord(HiddenWords[choosedWord]);
        _stateManager.OpenHiddenWord(choosedWord);
      }
    }
    else if (AllowPotentialWords.ContainsKey(choosedWord))
    {
      if (OpenWords.ContainsKey(choosedWord))
      {
        // already open allow word.
        // await _choosedWordMB.OpenAllowWord(colba);
        await _choosedWordMB.ExistAllowWord();
      }
      else
      {
        if (NeedWords.ContainsKey(choosedWord))
        {
          OpenNeedWords.Add(choosedWord, 1);
        }
        // open new allow word.
        OpenWords.Add(choosedWord, 1);
        await _choosedWordMB.OpenAllowWord();
        _stateManager.OpenAllowWord(choosedWord);
      }
    }
    else
    {
      // Debug.Log($"------Not found {choosedWord}");
      await _choosedWordMB.NoWord();
      _stateManager.DeRunPerk(choosedWord);
    }

    foreach (var obj in listChoosedGameObjects)
    {
      obj.GetComponent<CharMB>().ResetObject();
    }
    _lineManager.ResetLine();
    listChoosedGameObjects.Clear();

    bool isOpenAllNeedWords = OpenNeedWords.Count == Mathf.Min(_stateManager.ActiveWordConfig.maxNeedFindWords, NeedWords.Count);// AllowWords.Count;
    bool isOpenAllHiddenWords = OpenWords.Keys.Intersect(HiddenWords.Keys).Count() == HiddenWords.Count();
    if (isOpenAllNeedWords)
    {
      await UniTask.Delay(500);
      Debug.Log("Next level");
      await NextLevel();
    }
    else if (isOpenAllHiddenWords)
    {
      Debug.Log("Refresh hiddenWords");
      RefreshHiddenWords();
    }
    else
    {
      // GameManager.Instance.DataManager.Save();
      _stateManager.RefreshData();
    }
    // OnChangeData?.Invoke();
  }

  // public async UniTask OpenNeighbours()
  // {
  //   // open equals chars.
  //   List<GridNode> equalsCharNodes = GridHelper.FindEqualsHiddenNeighbours();
  //   Debug.Log($"equalsCharNodes count={equalsCharNodes.Count}");
  //   foreach (var equalCharNode in equalsCharNodes)
  //   {
  //     await equalCharNode.OccupiedChar.ShowCharAsNei(false);
  //   }
  // }

  public void AddChoosedChar(CharMB charGameObject)
  {
    if (!listChoosedGameObjects.Contains(charGameObject))
    {
      AudioManager.Instance.PlayClipEffect(_gameSetting.Audio.addChar);
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
        AudioManager.Instance.PlayClipEffect(_gameSetting.Audio.removeChar);
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

    // calculate count char of by words.
    // var sortedListHiddenWords = _hiddenWords.OrderBy(t => t.Length);
    // int countChars = sortedListHiddenWords.OrderBy(t => t.Length).Select(t => t.Length).Sum();
    // countChars += sortedListHiddenWords.Count();
    // string wordWithMaxLength = sortedListHiddenWords.Last();
    // int maxLengthWord = wordWithMaxLength.Length;
    // if (maxLengthWord < defaultGridSize) maxLengthWord = defaultGridSize;
    // int minNeedRows = (int)System.Math.Ceiling((double)countChars / maxLengthWord);

    var sizeGridXY = defaultGridSize;

    if (_stateManager.ActiveWordConfig.maxCountHiddenChar > defaultCountChars)
    {
      sizeGridXY = System.Convert.ToInt32(System.MathF.Ceiling((float)System.Math.Sqrt(_stateManager.ActiveWordConfig.maxCountHiddenChar)));
      sizeGridXY += 2;
    }

    // Debug.Log($"countWord={sortedListHiddenWords.Count()}, countChars ={countChars}, maxLengthWord={maxLengthWord}");
    // Debug.Log($"word max length={wordWithMaxLength}, Need count rows={minNeedRows}, sizeGridXY={sizeGridXY}");
    GridHelper = new GridHelper(sizeGridXY, sizeGridXY);
    // Set transform grid.
    float scale = (float)defaultGridSize / sizeGridXY;
    // Debug.Log($"Scale grid ={scale}");
    _GridObject.transform.localScale = new Vector3(scale, scale, 1);
    scaleGrid = scale;
  }


  private void RefreshHiddenWords()
  {
    Entities.Clear();

    Helpers.DestroyChildren(tilemapEntities.transform);
    Helpers.DestroyChildren(tilemap.transform);
    // foreach (var wordItem in HiddenWords)
    // {
    //   GameObject.Destroy(wordItem.Value.gameObject);
    // }

    HiddenWords.Clear();

    var _hiddenWords = CreateHiddenWords();

    SetScaleChars(_hiddenWords);

    CreateGameObjectHiddenWords(_hiddenWords);

    // CreateEntities();
    _stateManager.RefreshData();
    // GameManager.Instance.DataManager.Save();
  }


  public async UniTask NextLevel()
  {
    // await _levelManager.shuffle.Destroy();
    // await _levelManager.colba.Destroy();
    // await _levelManager.hint.Destroy();
    // _stateManager.RefreshData();
    // await _levelManager.NextLevel();
    // AllowWords.Clear();
    // // OpenHiddenWords.Clear();
    // OpenWords.Clear();
    // // GameManager.Instance.DataManager.Save();
    _gameManager.InputManager.Disable();
    _stateManager.RefreshData();
    var result = await _levelManager.statLevel.Show();
    if (result.isOk)
    {
      // _gameManager.DataManager.Save();
      // Reset();
      var newConfigWord = _stateManager.GetConfigNextLevel();
      _levelManager.InitLevel(newConfigWord);
      // await _levelManager.NextLevel();
    }
    _gameManager.InputManager.Enable();
  }


  public void Reset()
  {
    _levelManager.ResetSymbols();

    foreach (var wordItem in HiddenWords)
    {
      GameObject.Destroy(wordItem.Value.gameObject);
    }

    HiddenWords.Clear();

    NeedWords.Clear();

    OpenWords.Clear();

    OpenChars.Clear();

    Entities.Clear();

    AllowPotentialWords.Clear();

    OpenNeedWords.Clear();

    Helpers.DestroyChildren(tilemapEntities.transform);
    Helpers.DestroyChildren(tilemap.transform);
  }
}
