using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : Singleton<LevelManager>
{
  public static event Action OnInitLevel;
  private GameManager _gameManager => GameManager.Instance;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  private StateManager _stateManager => GameManager.Instance.StateManager;
  [Header("File Storage Config")]
  public ManagerHiddenWords ManagerHiddenWords;
  private List<CharMB> _symbols;
  public List<CharMB> Symbols => _symbols;
  public GameObject SymbolsField;
  public TopSide topSide;
  public DialogLevel dialogLevel;
  public ButtonStar buttonStar;
  public ButtonHint buttonHint;
  public ButtonBomb buttonBomb;
  public ButtonLighting buttonLighting;
  public ButtonShuffle buttonShuffle;
  public Stat stat;

  protected override void Awake()
  {
    base.Awake();
    _symbols = new();
  }

  public async void InitLevel(GameLevelWord wordConfig)
  {
    _gameManager.InputManager.Disable();

    ResetLevel();

    OnInitLevel?.Invoke();

#if UNITY_EDITOR
    System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
    stopWatch.Start();
#endif

    GameManager.Instance.StateManager.SetActiveLevel(wordConfig);

    ManagerHiddenWords.Init(); // levelConfig, wordConfig

    // Check complete level.
    if (_stateManager.dataGame.levels.Count > 0)
    {
      var currentLevel = _stateManager.dataGame.levels.Find(t => t.id == wordConfig.name);
      bool isEndLevel = currentLevel.openWords.Count - currentLevel.countDopWords == currentLevel.countNeedWords && currentLevel.openWords.Count > 0;
      if (isEndLevel)
      {
        ManagerHiddenWords.NextLevel().Forget();
        // GameManager.Instance.StateManager.RefreshData();
        return;
      }
    }

    CreateChars(ManagerHiddenWords.WordForChars);

    // GameManager.Instance.DataManager.Save();
    GameManager.Instance.StateManager.RefreshData();

    // Show start info.
    _gameManager.InputManager.Disable();
    var result = await dialogLevel.ShowDialogStartRound();
    if (result.isOk)
    {
      _gameManager.InputManager.Enable();
    }

#if UNITY_EDITOR
    stopWatch.Stop();
    System.TimeSpan timeTaken = stopWatch.Elapsed;
    Debug.LogWarning($"Init Level by time {timeTaken.ToString(@"m\:ss\.ffff")}");
#endif
  }

  public void CreateChars(string str)
  {
    // Debug.Log($"str={str}");
    float baseRadius = GameManager.Instance.GameSettings.radius;
    var countCharGO = str.ToArray();
    float radius = baseRadius + (countCharGO.Length / 2) * 0.1f;
    for (int i = 0; i < countCharGO.Length; ++i)
    {
      float circleposition = (float)i / (float)countCharGO.Length;
      float x = Mathf.Sin(circleposition * Mathf.PI * 2.0f) * radius + SymbolsField.transform.position.x;
      float y = Mathf.Cos(circleposition * Mathf.PI * 2.0f) * radius + SymbolsField.transform.position.y;
      var symbolGO = GameObject.Instantiate(
         _gameSetting.PrefabSymbol,
         new Vector3(x, y, 0.0f),
         Quaternion.identity,
         SymbolsField.transform
     );
      symbolGO.Init(countCharGO.ElementAt(i));
      var size = radius - ((radius - baseRadius) * baseRadius) - .3f;
      symbolGO.SetSize(size);
      _symbols.Add(symbolGO);
    }
  }
  // public async void ShuffleChars()
  // {
  //   var existChars = Symbols;

  //   // get all positions.
  //   Dictionary<CharMB, char> existPositionsChars = new();
  //   for (int i = 0; i < existChars.Count; i++)
  //   {
  //     existPositionsChars.Add(existChars[i], existChars[i].charTextValue);
  //   }
  //   // shuffle positions.
  //   existChars = existChars.OrderBy(t => UnityEngine.Random.value).ToList();

  //   // set new position.
  //   List<UniTask> tasks = new();
  //   string newWord = "";
  //   for (int i = 0; i < existChars.Count; i++)
  //   {
  //     tasks.Add(existChars[i].SetPosition(existPositionsChars.ElementAt(i).Key.transform.position));
  //     newWord += existChars.ElementAt(i).charTextValue;
  //   }

  //   ManagerHiddenWords.SetWordForChars(newWord);

  //   await UniTask.WhenAll(tasks);
  //   GameManager.Instance.DataManager.Save();
  // }

  public void ResetSymbols()
  {
    foreach (var symbol in _symbols)
    {
      GameObject.Destroy(symbol.gameObject);
    }
    _symbols.Clear();
    // await UniTask.Yield();
  }

  public async UniTask CheckNextLevelPlayer()
  {
    if (_gameManager.PlayerSetting.countFindWordsForUp <= _stateManager.dataGame.rate)
    {
      // TODO Next level status player;
    }
    // _gameManager.InputManager.Disable();
    // var dialogWindow = new UILevelsOperation();
    // var result = await dialogWindow.ShowAndHide();
    // _gameManager.InputManager.Enable();
    await UniTask.Yield();
  }

  private void ResetLevel()
  {
    ResetSymbols();

    ManagerHiddenWords.Reset();
  }

}
