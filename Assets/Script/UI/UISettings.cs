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
  private VisualElement _userCoinImg;
  private VisualElement _userRateImg;
  private Label _userRate;
  private Label _userCoin;
  private Label _userName;
  private Button _exitButton;
  private Button _openMenuButton;
  private Button _closeMenuButton;
  private Button _shopButton;
  private Button _toMenuAppButton;
  private Button _okButton;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  [SerializeField] private AudioManager _audioManager => GameManager.Instance.audioManager;

  private void Awake()
  {
    StateManager.OnChangeState += SetValue;
  }

  private void OnDestroy()
  {
    StateManager.OnChangeState -= SetValue;
  }

  public async virtual void Start()
  {
    _aside = _uiDoc.rootVisualElement.Q<VisualElement>("AsideBlok");
    _menu = _uiDoc.rootVisualElement.Q<VisualElement>("MenuBlok");
    _menu.style.display = DisplayStyle.None;

    _exitButton = _aside.Q<Button>("ExitBtn");
    _openMenuButton = _aside.Q<Button>("MenuBtn");
    _shopButton = _aside.Q<Button>("ShopBtn");

    _okButton = _menu.Q<Button>("Ok");
    _closeMenuButton = _menu.Q<Button>("CloseMenuBtn");

    _shopButton.clickable.clicked += () =>
    {
      ClickOpenShop();
    };
    _exitButton.clickable.clicked += () =>
    {
      ClickExitButton();
    };
    _okButton.clickable.clicked += () =>
    {
      ClickCloseMenuButton();
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

    _userCoin = _aside.Q<Label>("UserCoin");
    _userCoinImg = _aside.Q<VisualElement>("UserCoinImg");
    var configCoin = _gameManager.ResourceSystem.GetAllEntity().Find(t => t.typeEntity == TypeEntity.Coin);
    _userCoinImg.style.backgroundImage = new StyleBackground(configCoin.sprite);

    _userRate = _aside.Q<Label>("UserRate");
    _userRateImg = _aside.Q<VisualElement>("UserRateImg");
    _userRateImg.style.backgroundImage = new StyleBackground(_gameSetting.spriteRate);

    _userName = _aside.Q<Label>("UserName");

    _userName.text = string.IsNullOrEmpty(_gameManager.AppInfo.UserInfo.Name)
      ? await Helpers.GetLocaledString(_gameSettings.noName.title)
      : _gameManager.AppInfo.UserInfo.Name;

    ChangeTheme();
    SetValue(_gameManager.StateManager.dataGame);

    base.Initialize(_uiDoc.rootVisualElement);
  }


  private void ChangeTheme()
  {

    var imgCog = _aside.Q<VisualElement>("ImgCog");
    imgCog.style.backgroundImage = new StyleBackground(_gameSettings.spriteCog);
    imgCog.style.unityBackgroundImageTintColor = new StyleColor(_gameManager.Theme.colorPrimary);

    _userCoinImg.style.unityBackgroundImageTintColor = new StyleColor(_gameManager.Theme.entityColor);
    _userRateImg.style.unityBackgroundImageTintColor = new StyleColor(_gameManager.Theme.entityColor);

    _menu.Q<VisualElement>("MenuBlokWrapper").style.backgroundColor = new StyleColor(_gameManager.Theme.bgColor);

    var imgShop = _aside.Q<VisualElement>("ImgShop");
    imgShop.style.backgroundImage = new StyleBackground(_gameSettings.spriteShop);
    imgShop.style.unityBackgroundImageTintColor = new StyleColor(_gameManager.Theme.colorPrimary);

    base.Theming(_uiDoc.rootVisualElement);
  }

  private void SetValue(DataGame data)
  {
    _userCoin.text = string.Format("{0}", data.coins);
    _userRate.text = string.Format("{0}", data.rate);
  }

  private async void ClickOpenShop()
  {
    AudioManager.Instance.Click();
    _gameManager.InputManager.Disable();
    var dialogWindow = new UIShopOperation();
    var result = await dialogWindow.ShowAndHide();
    _gameManager.InputManager.Enable();
  }

  private void ShowMenu()
  {
    base.Theming(_uiDoc.rootVisualElement);
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
    AudioManager.Instance.Click();
    HideMenu();
    GameManager.Instance.ChangeState(GameState.CloseLevel);
  }

  private void ClickCloseMenuButton()
  {
    AudioManager.Instance.Click();
    _gameManager.InputManager.Enable();
    HideMenu();
  }

  private void ClickOpenMenuButton()
  {
    AudioManager.Instance.Click();
    _gameManager.InputManager.Disable();
    ShowMenu();
    CreateMenu();
  }

  private void CreateMenu()
  {
    var userSettings = _gameManager.AppInfo.userSettings;

    var menuBlok = _menu.Q<VisualElement>("Menu");
    var sliderVolumeEffect = menuBlok.Q<Slider>("VolumeEffect");
    sliderVolumeEffect.value = userSettings.auv;
    _audioManager.EffectSource.volume = sliderVolumeEffect.value;
    sliderVolumeEffect.RegisterValueChangedCallback((ChangeEvent<float> evt) =>
    {
      _audioManager.EffectSource.volume = evt.newValue;
      userSettings.auv = evt.newValue;
    });

    var sliderVolumeMusic = menuBlok.Q<Slider>("VolumeMusic");
    sliderVolumeMusic.value = userSettings.muv;
    _audioManager.MusicSource.volume = sliderVolumeMusic.value;
    sliderVolumeMusic.RegisterValueChangedCallback((ChangeEvent<float> evt) =>
    {
      _audioManager.MusicSource.volume = evt.newValue;
      userSettings.muv = evt.newValue;
    });

    var sliderTimeDelay = menuBlok.Q<SliderInt>("TimeDelay");
    sliderTimeDelay.value = userSettings.td;
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


    // Theme.
    var dropdownTheme = menuBlok.Q<DropdownField>("Theme");

    var allThemes = _gameManager.ResourceSystem.GetAllTheme();

    dropdownTheme.choices.Clear();
    for (int i = 0; i < allThemes.Count; i++)
    {
      GameTheme theme = allThemes[i];
      dropdownTheme.choices.Add(theme.name);
    }
    dropdownTheme.value = userSettings.theme;
    dropdownTheme.RegisterValueChangedCallback((ChangeEvent<string> evt) =>
    {
      GameTheme chooseTheme = allThemes.Find(t => t.name == evt.newValue);

      _gameManager.SetTheme(chooseTheme);

      ChangeTheme();
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
      base.Initialize(_uiDoc.rootVisualElement);
    }
    OnChangeLocale?.Invoke();
  }

  private void ClickExitButton()
  {
    // throw new NotImplementedException();
  }


}
