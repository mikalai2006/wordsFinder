using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Loader;
using UnityEngine;
using UnityEngine.UIElements;

public class UILevels : UILocaleBase
{
  [SerializeField] private UIDocument _uiDoc;
  [SerializeField] private VisualTreeAsset LevelBlok;
  [SerializeField] private VisualElement _root;
  private GameObject _enviromnment;
  [SerializeField] private ScrollView _listLevels;
  private TaskCompletionSource<DataResultLevelDialog> _processCompletionSource;
  private DataResultLevelDialog _result;

  private void Awake()
  {

    LevelManager.OnInitLevel += Hide;
  }

  private void OnDestroy()
  {

    LevelManager.OnInitLevel -= Hide;
  }

  public virtual void Start()
  {
    _root = _uiDoc.rootVisualElement;

    var exitBtn = _root.Q<Button>("ExitBtn");
    exitBtn.clickable.clicked += () => ClickClose();

    _listLevels = _root.Q<ScrollView>("ListLevels");

    FillLevels();

    base.Localize(_root);
  }

  private void Hide()
  {
    _root.style.display = DisplayStyle.None;
  }

  private void FillLevels()
  {
    foreach (var level in _gameSettings.GameLevels)
    {
      _listLevels.Add(new Label() { text = level.title });
      for (int i = 0; i < level.words.Count; i++)
      {
        var currentWord = level.words[i];
        var levelBlok = LevelBlok.Instantiate();
        levelBlok.Q<Label>("Name").text = currentWord.word;
        levelBlok.Q<Button>("GoBtn").clickable.clicked += () =>
        {
          InitLevel(level, currentWord);
        };
        _listLevels.Add(levelBlok);
      }
    }
  }

  private async void InitLevel(GameLevel levelConfig, GameLevelWord wordConfig)
  {
    var operations = new Queue<ILoadingOperation>();
    operations.Enqueue(new GameInitOperation());
    await _gameManager.LoadingScreenProvider.LoadAndDestroy(operations);
    _gameManager.LevelManager.InitLevel(levelConfig, wordConfig);
  }

  private void ClickClose()
  {
    _result.isOk = false;
    _processCompletionSource.SetResult(_result);
  }

  public void Init(GameObject environment)
  {
    _enviromnment = environment;
  }

  public async UniTask<DataResultLevelDialog> ProcessAction()
  {

    _processCompletionSource = new TaskCompletionSource<DataResultLevelDialog>();

    return await _processCompletionSource.Task;
  }
}
