using System;
using Cysharp.Threading.Tasks;
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
  private VisualElement _languageBlock;
  private VisualElement _userRateImg;
  private VisualElement _userShortInfo;
  private Toggle _doDialog;
  private Label _userRate;
  private Label _userCoin;
  private Label _userName;
  private Button _exitButton;
  private Button _openMenuButton;
  private Button _closeMenuButton;
  private Button _shopButton;
  private Button _toMenuAppButton;
  private Button _okButton;
  private DropdownField _dropdownLanguage;
  private SliderInt _sliderTimeDelay;
  private Slider _sliderVolumeMusic;
  private Slider _sliderVolumeEffect;
  private DropdownField _dropdownTheme;

  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  [SerializeField] private AudioManager _audioManager => GameManager.Instance.audioManager;

  private void Awake()
  {
    StateManager.OnChangeState += SetValue;
    DialogLevel.OnHideDialog += ShowAside;
    DialogLevel.OnShowDialog += HideAside;
    GameManager.OnAfterStateChanged += AfterStateChanged;
  }

  private void OnDestroy()
  {
    StateManager.OnChangeState -= SetValue;
    DialogLevel.OnHideDialog -= ShowAside;
    DialogLevel.OnShowDialog -= HideAside;
    GameManager.OnAfterStateChanged -= AfterStateChanged;
  }

  public virtual void Start()
  {
    _aside = _uiDoc.rootVisualElement.Q<VisualElement>("AsideBlok");
    _userShortInfo = _uiDoc.rootVisualElement.Q<VisualElement>("UserShortInfo");
    _menu = _uiDoc.rootVisualElement.Q<VisualElement>("MenuBlok");
    _menu.style.display = DisplayStyle.None;

    _languageBlock = _menu.Q<VisualElement>("LanguageBlock");


    var menuBlok = _menu.Q<VisualElement>("Menu");
    _dropdownLanguage = menuBlok.Q<DropdownField>("Language");
    _sliderTimeDelay = menuBlok.Q<SliderInt>("TimeDelay");
    _sliderVolumeMusic = menuBlok.Q<Slider>("VolumeMusic");
    _sliderVolumeEffect = menuBlok.Q<Slider>("VolumeEffect");
    _dropdownTheme = menuBlok.Q<DropdownField>("Theme");
    _doDialog = menuBlok.Q<Toggle>("DoDialog");


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

    _toMenuAppButton.style.display = DisplayStyle.None;
    _userShortInfo.style.display = DisplayStyle.None;

    _userCoin = _aside.Q<Label>("UserCoin");
    _userCoinImg = _aside.Q<VisualElement>("UserCoinImg");
    var configCoin = _gameManager.ResourceSystem.GetAllEntity().Find(t => t.typeEntity == TypeEntity.Coin);
    _userCoinImg.style.backgroundImage = new StyleBackground(configCoin.sprite);

    _userRate = _aside.Q<Label>("UserRate");
    _userRateImg = _aside.Q<VisualElement>("UserRateImg");
    _userRateImg.style.backgroundImage = new StyleBackground(_gameSetting.spriteRate);

    _userName = _aside.Q<Label>("UserName");


    ChangeTheme(null);
    SetValue(_gameManager.StateManager.stateGame);

    Refresh();

    base.Initialize(_uiDoc.rootVisualElement);
  }


  private void AfterStateChanged(GameState state)
  {
    switch (state)
    {
      case GameState.CloseLevel:
      case GameState.LoadLevel:
        Refresh();
        break;
    }
  }

  private void Refresh()
  {
    Debug.Log($"{name}::: Refresh");
    if (_gameManager.LevelManager != null)
    {
      _userShortInfo.style.display = DisplayStyle.Flex;
    }
    else
    {
      _userShortInfo.style.display = DisplayStyle.None;
    }
  }


  private async void ChangeTheme(ChangeEvent<string> evt)
  {
    if (evt != null)
    {
      var allThemes = _gameManager.ResourceSystem.GetAllTheme();
      GameTheme chooseTheme = allThemes.Find(t => t.name == evt.newValue);

      _gameManager.SetTheme(chooseTheme);
      _gameManager.DataManager.SaveSettings();
    }


    var imgCog = _aside.Q<VisualElement>("ImgCog");
    imgCog.style.backgroundImage = new StyleBackground(_gameSettings.spriteCog);
    imgCog.style.unityBackgroundImageTintColor = new StyleColor(_gameManager.Theme.colorSecondary);

    // load avatar
    string placeholder = _gameManager.AppInfo.UserInfo.photo;
#if UNITY_EDITOR
    placeholder = "https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/CGIB4J3KPRTV5JCX6JLC6TAKLL6GYPN27SBYCQDUWEUW2QFCBB5ZNTCHKPVHHHKLTSBRYYIEMFB2C3CB37T4S7GIXKUP4KEL4CHGRPVOLHVSPIW77Z5TUSEOVQK5NDDSPJTVMAJAODQ4DXD6UEKJV4VLEUOPWOPU2Y664NQ5NIQUT2UBNRMVVWCQN52FYLVEI4DWLSZQ4FG6AZWBGKYTD5VJWXXAL46Z7B5XDCI=/islands-retina-medium";
#endif
    var imgAva = _aside.Q<VisualElement>("Ava");
    Texture2D avatarTexture = await Helpers.LoadTexture(placeholder);
    if (avatarTexture != null)
    {
      imgAva.style.backgroundImage = new StyleBackground(avatarTexture);
    }
    else
    {
      imgAva.style.display = DisplayStyle.None;
    }

    _userCoinImg.style.unityBackgroundImageTintColor = new StyleColor(_gameManager.Theme.colorSecondary);
    _userRateImg.style.unityBackgroundImageTintColor = new StyleColor(_gameManager.Theme.colorSecondary);

    _menu.Q<VisualElement>("MenuBlokWrapper").style.backgroundColor = new StyleColor(_gameManager.Theme.bgColor);

    var imgShop = _aside.Q<VisualElement>("ImgShop");
    imgShop.style.backgroundImage = new StyleBackground(_gameSettings.spriteShop);
    imgShop.style.unityBackgroundImageTintColor = new StyleColor(_gameManager.Theme.colorSecondary);

    _userName.text = await Helpers.GetName();


    base.Theming(_uiDoc.rootVisualElement);
  }

  private void SetValue(StateGame state)
  {
    _userCoin.text = string.Format("{0}", state.coins);
    _userRate.text = string.Format("{0}", state.rate);
  }

  private async void ClickOpenShop()
  {
    AudioManager.Instance.Click();
    _gameManager.InputManager.Disable();
    var dialogWindow = new UIShopOperation();
    var result = await dialogWindow.ShowAndHide();
    _gameManager.InputManager.Enable();
  }

  private void ShowAside()
  {
    // _aside.style.display = DisplayStyle.Flex;
    _openMenuButton.style.visibility = Visibility.Visible;
    _shopButton.style.visibility = Visibility.Visible;
  }
  private void HideAside()
  {
    // _aside.style.display = DisplayStyle.None;
    _openMenuButton.style.visibility = Visibility.Hidden;
    _shopButton.style.visibility = Visibility.Hidden;
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

    _dropdownLanguage.RegisterValueChangedCallback(ChooseLanguage);
    _dropdownTheme.RegisterValueChangedCallback(ChangeTheme);
    _doDialog.RegisterValueChangedCallback(ChangeDoDialog);
  }

  private void HideMenu()
  {
    _menu.style.display = DisplayStyle.None;

    _dropdownLanguage.UnregisterValueChangedCallback(ChooseLanguage);
    _dropdownTheme.UnregisterValueChangedCallback(ChangeTheme);
    _doDialog.UnregisterValueChangedCallback(ChangeDoDialog);
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
    var userSettings = _gameManager.AppInfo.setting;

    _sliderVolumeEffect.value = userSettings.auv;
    _audioManager.EffectSource.volume = _sliderVolumeEffect.value;
    _sliderVolumeEffect.RegisterValueChangedCallback((ChangeEvent<float> evt) =>
    {
      _audioManager.EffectSource.volume = evt.newValue;
      userSettings.auv = evt.newValue;
      _gameManager.DataManager.SaveSettings();
    });

    _sliderVolumeMusic.value = userSettings.muv;
    _audioManager.MusicSource.volume = _sliderVolumeMusic.value;
    _sliderVolumeMusic.RegisterValueChangedCallback((ChangeEvent<float> evt) =>
    {
      _audioManager.MusicSource.volume = evt.newValue;
      userSettings.muv = evt.newValue;
      _gameManager.DataManager.SaveSettings();
    });

    _sliderTimeDelay.value = userSettings.td;
    _sliderTimeDelay.RegisterValueChangedCallback((ChangeEvent<int> evt) =>
    {
      userSettings.td = evt.newValue;
      _gameManager.DataManager.SaveSettings();
    });



    _dropdownLanguage.value = LocalizationSettings.SelectedLocale.LocaleName;
    _dropdownLanguage.choices.Clear();
    for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
    {
      Locale locale = LocalizationSettings.AvailableLocales.Locales[i];
      _dropdownLanguage.choices.Add(locale.LocaleName);
    }


    if (_gameManager.LevelManager != null)
    {
      _languageBlock.style.display = DisplayStyle.None;
    }
    else
    {
      _languageBlock.style.display = DisplayStyle.Flex;
    }


    // Theme.
    var allThemes = _gameManager.ResourceSystem.GetAllTheme();

    _dropdownTheme.choices.Clear();
    for (int i = 0; i < allThemes.Count; i++)
    {
      GameTheme theme = allThemes[i];
      _dropdownTheme.choices.Add(theme.name);
    }
    _dropdownTheme.value = userSettings.theme;


    // DoDialog.
    _doDialog.value = userSettings.dod;
  }

  private void ChangeDoDialog(ChangeEvent<bool> evt)
  {
    var userSettings = _gameManager.AppInfo.setting;
    userSettings.dod = evt.newValue;
    _gameManager.DataManager.SaveSettings();
  }

  private async void ChooseLanguage(ChangeEvent<string> evt)
  {
    string nameLanguage = evt.newValue;

    var userSettings = _gameManager.AppInfo.setting;

    await LocalizationSettings.InitializationOperation.Task;

    // Debug.Log($"Choose lang={nameLanguage} | {selectedLocale} | {selectedLocale == nameLanguage}");
    Locale currentLocale = LocalizationSettings.AvailableLocales.Locales.Find(t => t.LocaleName == nameLanguage);
    if (currentLocale.Identifier.Code != LocalizationSettings.SelectedLocale.Identifier.Code)
    {
      LocalizationSettings.SelectedLocale = currentLocale;//LocalizationSettings.AvailableLocales.Locales[indexLocale];
      userSettings.lang = currentLocale.Identifier.Code;
      _gameManager.DataManager.SaveSettings();
      base.Initialize(_uiDoc.rootVisualElement);
      // Words words = _gameManager.WordsAll.Find(t => t.locale.Identifier.Code == LocalizationSettings.SelectedLocale.Identifier.Code);
      await _gameManager.SetActiveWords();

      await _gameManager.StateManager.SetActiveDataGame();

      // await UniTask.Delay(500);
    }


    OnChangeLocale?.Invoke();
  }

  private void ClickExitButton()
  {
    // throw new NotImplementedException();
  }


}
