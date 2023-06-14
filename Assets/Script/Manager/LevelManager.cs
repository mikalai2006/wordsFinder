using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : Singleton<LevelManager>
{
  public static event Action OnInitLevel;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  [Header("File Storage Config")]
  public ManagerHiddenWords ManagerHiddenWords;
  private List<CharMB> _symbols;
  public List<CharMB> Symbols => _symbols;
  public GameObject SymbolsField;
  protected override void Awake()
  {
    base.Awake();
    _symbols = new();
  }

  //   public void LoadLevel(DataLevel data)
  //   {
  // #if UNITY_EDITOR
  //     System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
  //     stopWatch.Start();
  // #endif

  //     ManagerHiddenWords.LoadWords(data);
  //     CreateSymbols(ManagerHiddenWords.WordForChars);

  // #if UNITY_EDITOR
  //     stopWatch.Stop();
  //     System.TimeSpan timeTaken = stopWatch.Elapsed;
  //     Debug.LogWarning($"Load Level by time {timeTaken.ToString(@"m\:ss\.ffff")}");
  // #endif
  //   }

  public void InitLevel(GameLevel levelConfig, GameLevelWord wordConfig)
  {
    OnInitLevel?.Invoke();

#if UNITY_EDITOR
    System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
    stopWatch.Start();
#endif



    GameManager.Instance.StateManager.SetActiveLevel(levelConfig, wordConfig);

    ManagerHiddenWords.Init(levelConfig, wordConfig);

    GameManager.Instance.StateManager.RefreshData();

    CreateChars(ManagerHiddenWords.WordForChars);

    GameManager.Instance.DataManager.Save();

#if UNITY_EDITOR
    stopWatch.Stop();
    System.TimeSpan timeTaken = stopWatch.Elapsed;
    Debug.LogWarning($"Init Level by time {timeTaken.ToString(@"m\:ss\.ffff")}");
#endif
  }

  public void CreateChars(string str)
  {
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

  private async UniTask ResetSymbols()
  {
    foreach (var symbol in _symbols)
    {
      GameObject.Destroy(symbol);
    }
    await UniTask.Yield();
  }

  public async UniTask NextLevel()
  {

    await ResetSymbols();

    // CreateLevel();

    await UniTask.Yield();
  }

}
