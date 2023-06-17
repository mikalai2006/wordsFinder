using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HiddenCharMB : MonoBehaviour
{
  [SerializeField] private TMPro.TextMeshProUGUI _charText;
  public char charTextValue;
  private LevelManager _levelManager = LevelManager.Instance;
  [SerializeField] private Image _image;
  [SerializeField] private RectTransform _canvas;
  private GameSetting _gameSetting;
  public GridNode OccupiedNode;
  // ParticleSystem system
  // {
  //   get
  //   {
  //     if (_CachedSystem == null)
  //       _CachedSystem = GetComponent<ParticleSystem>();
  //     return _CachedSystem;
  //   }
  // }
  // private ParticleSystem _CachedSystem;

  public void Awake()
  {
    _gameSetting = GameManager.Instance.GameSettings;
    SetDefault();
  }

  public void SetValue(char currentChar)
  {
    _charText.text = currentChar.ToString();
    charTextValue = currentChar;
  }


  // public void SetSize(float size)
  // {
  //   // _charText.fontSize = size;
  //   _canvas.sizeDelta = new Vector2(size, size);
  // }


  private void SetDefault()
  {
    _image.color = _gameSetting.Theme.bgHiddenWord;
    _image.sprite = _gameSetting.Theme.bgImageHiddenWord;
    _charText.gameObject.SetActive(false);
  }


  public void Open(bool runEffect)
  {
    if (runEffect) RunOpenEffect();

    _image.color = _gameSetting.Theme.bgOpentHiddenWord;
    _image.sprite = _gameSetting.Theme.bgImageHiddenWord;
    _charText.color = _gameSetting.Theme.textOpentHiddenWord;
    _charText.gameObject.SetActive(true);
    OccupiedNode.SetOpen();
  }


  public void RunOpenEffect()
  {
    var _CachedSystem = GameObject.Instantiate(
      _gameSetting.prefabParticleSystem,
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
      new GradientColorKey(_gameSetting.Theme.bgHiddenWord, 1.0f),
      new GradientColorKey(_gameSetting.Theme.bgHiddenWord, 0.0f)
      }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f)
    });

    col.color = grad;
    // var colorOver = cs.colorOverLifetime;
    // colorOver.enabled = true;
    // colorOver.color = _gameSetting.Theme.bgHiddenWord; //new ParticleSystem.MinMaxGradient(_gameSetting.Theme.bgHiddenWord, _gameSetting.Theme.textFindHiddenWord);
    _CachedSystem.Play();
    Destroy(_CachedSystem.gameObject, 2f);
  }

  // public void FixedUpdate()
  // {
  //   if (_CachedSystem && !_CachedSystem.IsAlive())
  //   {
  //     Destroy(_CachedSystem.gameObject);
  //     Debug.Log("Destroy PS");
  //   }
  // }
  // public void StopOpenEffect()
  // {
  //   if (system.isPlaying)
  //   {
  //     system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
  //   }
  //   else
  //   {
  //     Debug.Log("No Ps");
  //   }
  // }

  public async UniTask ShowChar(bool runEffect)
  {
    Vector3 initialScale = transform.localScale;

    // for (float i = 0f; i < 180f; i += 10f)
    // {
    //   transform.rotation = Quaternion.Euler(0f, i, 0f);
    //   if (i == 90f)
    //   {
    //     _image.color = _gameSetting.colorWordSymbolYes;
    //   }
    //   yield return new WaitForSeconds(0.01f);
    // }
    // gameObject.transform.localScale = new Vector3(-1, 1, 1);

    Open(runEffect);
    await OpenNeighbours(runEffect);
    // await UniTask.Yield();
  }
  public async UniTask ShowCharAsNei(bool runEffect)
  {
    Vector3 initialScale = transform.localScale;

    // for (float i = 0f; i < 180f; i += 10f)
    // {
    //   transform.rotation = Quaternion.Euler(0f, i, 0f);
    //   if (i == 90f)
    //   {
    //     _image.color = _gameSetting.colorWordSymbolYes;
    //   }
    //   yield return new WaitForSeconds(0.01f);
    // }
    // gameObject.transform.localScale = new Vector3(-1, 1, 1);

    Open(runEffect);
    _image.color = _gameSetting.Theme.bgOpenNeiHiddenWord;
    _charText.color = _gameSetting.Theme.textOpenNeiHiddenWord;
    await OpenNeighbours(runEffect);
    // await UniTask.Yield();
  }

  public async UniTask OpenNeighbours(bool runEffect)
  {
    // open equals chars.
    List<GridNode> equalsCharNodes = GameManager.Instance.LevelManager.ManagerHiddenWords.GridHelper.FindNeighboursNodesOfByEqualChar(OccupiedNode);
    foreach (var equalCharNode in equalsCharNodes)
    {
      await equalCharNode.OccupiedChar.ShowCharAsNei(runEffect);
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

#if UNITY_EDITOR
  public override string ToString()
  {
    return "HiddenCharMB::: [text=" + charTextValue + "] \n";
  }
#endif
}
