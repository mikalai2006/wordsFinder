using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HiddenCharMB : MonoBehaviour, IPointerDownHandler
{
  [SerializeField] private TMPro.TextMeshProUGUI _charText;
  public char charTextValue;
  private GameManager _gameManager => GameManager.Instance;
  private LevelManager _levelManager => LevelManager.Instance;
  private StateManager _stateManager => GameManager.Instance.StateManager;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  [SerializeField] private Image _image;
  [SerializeField] private RectTransform _canvas;
  public GridNode OccupiedNode;

  public void Awake()
  {
    SetDefault();

    GameManager.OnChangeTheme += ChangeTheme;
  }

  private void OnDestroy()
  {
    GameManager.OnChangeTheme -= ChangeTheme;
  }

  private void ChangeTheme()
  {
    _image.color = _gameManager.Theme.bgHiddenWord;
    _image.sprite = _gameManager.Theme.bgImageHiddenWord;

    if (OccupiedNode == null) return;
    // if (!OccupiedNode.StateNode.HasFlag(StateNode.Open)) return;

    if (OccupiedNode.StateNode.HasFlag(StateNode.Open))
    {
      _image.color = _gameManager.Theme.bgOpentHiddenWord;
      _image.sprite = _gameManager.Theme.bgImageHiddenWord;
      _charText.color = _gameManager.Theme.textOpentHiddenWord;
    }
    else if (OccupiedNode.StateNode.HasFlag(StateNode.Hint))
    {
      _image.color = _gameManager.Theme.bgOpenNeiHiddenWord;
      _charText.color = _gameManager.Theme.textOpenNeiHiddenWord;
    }
  }

  public void SetChar(char currentChar)
  {
    ChangeTheme();
    _charText.text = currentChar.ToString();
    charTextValue = currentChar;
  }


  private void SetDefault()
  {
    _image.color = _gameManager.Theme.bgHiddenWord;
    _image.sprite = _gameManager.Theme.bgImageHiddenWord;
    _charText.gameObject.SetActive(false);
  }


  public void RunOpenEffect()
  {
    var _CachedSystem = GameObject.Instantiate(
      _gameSetting.Boom,
      transform.position,
      Quaternion.identity
    );

    var main = _CachedSystem.main;
    // main.startColor = new ParticleSystem.MinMaxGradient(_gameSetting.Theme.bgHiddenWord, _gameSetting.Theme.textFindHiddenWord);
    main.startSize = new ParticleSystem.MinMaxCurve(0.05f, _levelManager.ManagerHiddenWords.scaleGrid / 2);

    var col = _CachedSystem.colorOverLifetime;
    col.enabled = true;

    Gradient grad = new Gradient();
    grad.SetKeys(new GradientColorKey[] {
      new GradientColorKey(_gameManager.Theme.bgHiddenWord, 1.0f),
      new GradientColorKey(_gameManager.Theme.bgHiddenWord, 0.0f)
      }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f)
    });

    col.color = grad;

    _CachedSystem.Play();

    Destroy(_CachedSystem.gameObject, 2f);
  }


  public async UniTask ShowChar(bool runEffect, char _char)
  {
    Vector3 initialScale = transform.localScale;

    int valueBonusSaveHintLetter;
    _stateManager.dataGame.bonus.TryGetValue(TypeBonus.SaveHintLetter, out valueBonusSaveHintLetter);

    OccupiedNode.SetOpen();

    Open(runEffect);

    ChangeTheme();

    // Add coin.
    if (runEffect && (!OccupiedNode.StateNode.HasFlag(StateNode.Hint) || valueBonusSaveHintLetter > 0))
    {
      // play sound.
      _gameManager.audioManager.PlayClipEffect(_gameSetting.Audio.openHiddenChar);

      // _levelManager.CreateCoin(
      //   transform.position,
      //   _levelManager.topSide.spriteCoinPosition,
      //   1
      // ).Forget();
      _stateManager.IncrementCoin(1);

      _levelManager.CreateLetter(transform.position, _levelManager.buttonFlask.transform.position, charTextValue).Forget();
    }

    OccupiedNode.SetHint(false);

    if (runEffect)
    {
      await OpenNeighbours(runEffect);
    }

    await UniTask.Yield();
  }


  public async UniTask ShowCharAsHint(bool runEffect)
  {
    _levelManager.ManagerHiddenWords.AddOpenChar(this);

    OccupiedNode.SetHint();

    Open(runEffect);

    ChangeTheme();

    if (!runEffect) return;

    // play sound.
    _gameManager.audioManager.PlayClipEffect(_gameSetting.Audio.openHintChar);

    // Check hinted all chars of by word.
    int countOpenChar = 0;
    foreach (var hiddenChar in OccupiedNode.OccupiedWord.Chars)
    {
      if (
        hiddenChar.OccupiedNode.StateNode.HasFlag(StateNode.Hint)
        || hiddenChar.OccupiedNode.StateNode.HasFlag(StateNode.Open)
        )
      {
        countOpenChar++;
      }
    }
    if (countOpenChar == OccupiedNode.OccupiedWord._word.Length)
    {
      OccupiedNode.OccupiedWord.AutoOpenWord().Forget();
    }

    if (_stateManager.dataGame.bonus.ContainsKey(TypeBonus.OpenNeighbours))
    {
      await OpenNeighbours(runEffect);
    }
    await UniTask.Yield();
  }

  public void Open(bool runEffect)
  {
    if (runEffect)
    {
      RunOpenEffect();
    }

    // Remove open char.
    if (
      _stateManager.dataGame.activeLevel.openChars.ContainsKey(OccupiedNode.arrKey)
      &&
      _levelManager.ManagerHiddenWords.OpenWords.ContainsKey(OccupiedNode.OccupiedWord._word)
      )
    {
      _levelManager.ManagerHiddenWords.RemoveOpenChar(this);
    }
    _charText.gameObject.SetActive(true);

    // Check exist bonus of by node.
    if (OccupiedNode.StateNode.HasFlag(StateNode.Bonus))
    {
      OccupiedNode.BonusEntity.Collect();
    }
    // Check exist entity of by node.
    if (OccupiedNode.StateNode.HasFlag(StateNode.Entity))
    {
      OccupiedNode.OccupiedEntity.Run().Forget();
    }
  }


  public async UniTask OpenNeighbours(bool runEffect)
  {
    int valueBonusOpenNeighbours;
    _stateManager.dataGame.bonus.TryGetValue(TypeBonus.OpenNeighbours, out valueBonusOpenNeighbours);

    if (valueBonusOpenNeighbours <= 0) return;

    // open equals chars.
    List<GridNode> equalsCharNodes = GameManager.Instance.LevelManager.ManagerHiddenWords.GridHelper.FindNeighboursNodesOfByEqualChar(OccupiedNode);
    foreach (var equalCharNode in equalsCharNodes)
    {
      await equalCharNode.OccupiedChar.ShowCharAsHint(runEffect);
    }
    //await UniTask.Yield();
  }


  public async UniTask FocusOpenChar()
  {
    var startScale = transform.localScale;

    Vector3 initialScale = transform.localScale;
    Vector3 upScale = new Vector3(1.2f, 1.2f, 0f);
    var duration = .5f;
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
      float progress = elapsedTime / duration;
      transform.localScale = Vector3.Lerp(initialScale, upScale, progress);
      elapsedTime += Time.deltaTime;
      await UniTask.Yield();
    }
    transform.localScale = initialScale;
  }


  public async void OnPointerDown(PointerEventData eventData)
  {
    // Show message
    _gameManager.InputManager.Disable();

    var message = await Helpers.GetLocaledString("openletterbyads");
    var dialogConfirm = new DialogProvider(new DataDialog()
    {
      message = message,
      showCancelButton = true,
    });

    var result = await dialogConfirm.ShowAndHide();
    if (result.isOk)
    {
      // TODO run ads.

    }
    _gameManager.InputManager.Enable();
  }


#if UNITY_EDITOR
  public override string ToString()
  {
    return "HiddenCharMB::: [text=" + charTextValue + "] \n";
  }
#endif
}
