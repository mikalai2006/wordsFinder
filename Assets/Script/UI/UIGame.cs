using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Loader;
using UnityEngine;
using UnityEngine.UIElements;

public class UIGame : UILocaleBase
{
  [SerializeField] private UIDocument _uiDoc;
  private Button _colba;
  private Button _shuffle;
  private GameSetting GameSetting;
  [SerializeField] private AudioManager _audioManager => GameManager.Instance.audioManager;

  private void Awake()
  {
    UISettings.OnChangeLocale += ChangeLocale;
  }

  private void OnDestroy()
  {
    UISettings.OnChangeLocale -= ChangeLocale;
  }

  public virtual void Start()
  {
    GameSetting = GameManager.Instance.GameSettings;

    _colba = _uiDoc.rootVisualElement.Q<Button>("Colba");

    _shuffle = _uiDoc.rootVisualElement.Q<Button>("Shuffle");
    _shuffle.clickable.clicked += () =>
    {
      ClickShuffle();
    };

    base.Localize(_uiDoc.rootVisualElement);
  }

  private void ClickShuffle()
  {
  }

  private void ChangeLocale()
  {
    base.Localize(_uiDoc.rootVisualElement);
  }

}
