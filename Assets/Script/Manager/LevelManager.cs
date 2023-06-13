using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : Singleton<LevelManager>
{
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  [Header("File Storage Config")]
  public ManagerHiddenWords ManagerHiddenWords;
  private List<SymbolMB> _symbols;
  public List<SymbolMB> Symbols => _symbols;
  public GameObject SymbolsField;
  protected override void Awake()
  {
    base.Awake();
    _symbols = new();
  }

  public void LoadLevel()
  {
    LoadNeedWords();
    CreateSymbols(ManagerHiddenWords.WordForChars);
  }

  public void LoadNeedWords()
  {
    var data = GameManager.Instance.DataManager.DataGame;

    ManagerHiddenWords.LoadWords(data);
  }

  public void CreateLevel()
  {
    CreateNeedWords();
    CreateSymbols(ManagerHiddenWords.WordForChars);
    GameManager.Instance.DataManager.Save();
  }

  public void CreateNeedWords()
  {
    // find all allow words.
    var potentialWords = GameManager.Instance.Words.data
      .Where(t => t.Length <= _gameSetting.GameLevels.Levels[0].maxCountChar)
      .OrderBy(t => Random.value)
      .ToList();

    // create hidden words.
    ManagerHiddenWords.CreateWords(potentialWords);
  }

  public void CreateSymbols(string str)
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

    CreateLevel();

    await UniTask.Yield();
  }

}
