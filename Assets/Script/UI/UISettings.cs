using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

public class UISettings : UILocaleBase
{
  [SerializeField] private UIDocument _uiDoc;
  public static event Action OnChangeLocale;
  private VisualElement _aside;
  private VisualElement _menu;
  private Button _exitButton;
  private Button _openMenuButton;
  private Button _closeMenuButton;
  private Button _LevelsButton;
  private Button _toMenuAppButton;
  private GameSetting GameSetting;
  [SerializeField] private AudioManager _audioManager => GameManager.Instance.audioManager;

  public virtual void Start()
  {

    _aside = _uiDoc.rootVisualElement.Q<VisualElement>("AsideBlok");
    _menu = _uiDoc.rootVisualElement.Q<VisualElement>("MenuBlok");
    _menu.style.display = DisplayStyle.None;
    _menu.Q<VisualElement>("MenuBlokWrapper").style.backgroundColor = new StyleColor(_gameSettings.Theme.bgColor);
    _aside.Q<VisualElement>("ImgCog").style.backgroundImage = new StyleBackground(_gameSettings.spriteCog);
    _aside.Q<VisualElement>("ImgShop").style.backgroundImage = new StyleBackground(_gameSettings.spriteShop);

    GameSetting = GameManager.Instance.GameSettings;

    _exitButton = _aside.Q<Button>("ExitBtn");
    _openMenuButton = _aside.Q<Button>("MenuBtn");
    _closeMenuButton = _menu.Q<Button>("CloseMenuBtn");
    _LevelsButton = _aside.Q<Button>("ShopBtn");
    _LevelsButton.clickable.clicked += () =>
    {
      ClickOpenListLevels();
    };
    _exitButton.clickable.clicked += () =>
    {
      ClickExitButton();
    };

    _openMenuButton.clickable.clicked += () =>
    {
      ClickOpenMenuButton();
    };

    _closeMenuButton.clickable.clicked += () =>
    {
      ClickCloseMenuButton();
    };

    _toMenuAppButton = _menu.Q<Button>("ToStartMenuBtn");
    _toMenuAppButton.clickable.clicked += () =>
    {
      ClickToStartMenuButton();
    };
    if (_gameManager.LevelManager == null)
    {
      _toMenuAppButton.style.display = DisplayStyle.None;
    }

    base.Localize(_uiDoc.rootVisualElement);
  }

  private async void ClickOpenListLevels()
  {
    _gameManager.InputManager.Disable();
    var dialogWindow = new UILevelsOperation();
    var result = await dialogWindow.ShowAndHide();
    _gameManager.InputManager.Enable();
  }

  private void ShowMenu()
  {
    _menu.style.display = DisplayStyle.Flex;

    if (_gameManager.LevelManager == null)
    {
      _toMenuAppButton.style.display = DisplayStyle.None;
    }
    else
    {
      _toMenuAppButton.style.display = DisplayStyle.Flex;
    }
  }
  private void HideMenu()
  {
    _menu.style.display = DisplayStyle.None;
  }

  private void ClickToStartMenuButton()
  {
    HideMenu();
    GameManager.Instance.ChangeState(GameState.CloseLevel);
  }

  private void ClickCloseMenuButton()
  {
    _gameManager.InputManager.Enable();
    HideMenu();
  }

  private void ClickOpenMenuButton()
  {
    _gameManager.InputManager.Disable();
    ShowMenu();
    CreateMenu();
  }

  private void CreateMenu()
  {
    var userSettings = _gameManager.AppInfo.userSettings;

    var menuBlok = _menu.Q<VisualElement>("Menu");
    var sliderVolumeEffect = menuBlok.Q<Slider>("VolumeEffect");
    sliderVolumeEffect.value = userSettings == null
      ? GameSetting.Audio.volumeEffect
      : userSettings.auv;
    _audioManager.EffectSource.volume = sliderVolumeEffect.value;
    sliderVolumeEffect.RegisterValueChangedCallback((ChangeEvent<float> evt) =>
    {
      _audioManager.EffectSource.volume = evt.newValue;
      userSettings.auv = evt.newValue;
    });

    var sliderVolumeMusic = menuBlok.Q<Slider>("VolumeMusic");
    sliderVolumeMusic.value = userSettings == null
      ? GameSetting.Audio.volumeMusic
      : userSettings.muv;
    _audioManager.MusicSource.volume = sliderVolumeMusic.value;
    sliderVolumeMusic.RegisterValueChangedCallback((ChangeEvent<float> evt) =>
    {
      _audioManager.MusicSource.volume = evt.newValue;
      userSettings.muv = evt.newValue;
    });

    var sliderTimeDelay = menuBlok.Q<SliderInt>("TimeDelay");
    sliderTimeDelay.value = userSettings == null
      ? GameSetting.timeDelayOverChar
      : userSettings.td;
    sliderTimeDelay.RegisterValueChangedCallback((ChangeEvent<int> evt) =>
    {
      userSettings.td = evt.newValue;
    });

    var dropdownLanguage = menuBlok.Q<DropdownField>("Language");

    dropdownLanguage.value = LocalizationSettings.SelectedLocale.name;
    dropdownLanguage.choices.Clear();
    for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
    {
      Locale locale = LocalizationSettings.AvailableLocales.Locales[i];
      dropdownLanguage.choices.Add(locale.name);
    }
    dropdownLanguage.RegisterValueChangedCallback((ChangeEvent<string> evt) =>
    {
      ChooseLanguage(evt.newValue);
    });
  }

  private void ChooseLanguage(string nameLanguage)
  {
    var userSettings = _gameManager.AppInfo.userSettings;

    // Debug.Log($"Choose lang={nameLanguage} | {selectedLocale} | {selectedLocale == nameLanguage}");
    int indexLocale = LocalizationSettings.AvailableLocales.Locales.FindIndex(t => t.name == nameLanguage);
    if (nameLanguage != LocalizationSettings.SelectedLocale.name && indexLocale != -1)
    {
      LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[indexLocale];
      userSettings.lang = nameLanguage;
      base.Localize(_uiDoc.rootVisualElement);
    }
    OnChangeLocale?.Invoke();
  }

  private void ClickExitButton()
  {
    // throw new NotImplementedException();
  }


}
