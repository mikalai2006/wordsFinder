using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HiddenCharMB : MonoBehaviour
{
  [SerializeField] private TMPro.TextMeshProUGUI _charText;
  public char charTextValue;
  private LevelManager _levelManager => LevelManager.Instance;
  private StateManager _stateManager => GameManager.Instance.StateManager;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  [SerializeField] private Image _image;
  [SerializeField] private RectTransform _canvas;
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
    SetDefault();
  }

  public void SetChar(char currentChar)
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

    // Remove open char.
    if (
      _stateManager.dataGame.activeLevel.openChars.ContainsKey(OccupiedNode.arrKey)
      &&
      _levelManager.ManagerHiddenWords.OpenWords.ContainsKey(OccupiedNode.OccupiedWord._word)
      )
    {
      _levelManager.ManagerHiddenWords.RemoveOpenChar(this);
    }

    _image.color = _gameSetting.Theme.bgOpentHiddenWord;
    _image.sprite = _gameSetting.Theme.bgImageHiddenWord;
    _charText.color = _gameSetting.Theme.textOpentHiddenWord;
    _charText.gameObject.SetActive(true);
    OccupiedNode.SetOpen();

    // Check exist entity of by node.
    if (OccupiedNode.StateNode.HasFlag(StateNode.Entity))
    {
      OccupiedNode.OccupiedEntity.Run();
    }
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

    Open(runEffect);

    // Add coin.
    if (runEffect && !OccupiedNode.StateNode.HasFlag(StateNode.Hint)) AddCoin();

    // await OpenNeighbours(runEffect);
    await UniTask.Yield();
  }


  public async UniTask ShowCharAsNei(bool runEffect)
  {
    Open(runEffect);

    _image.color = _gameSetting.Theme.bgOpenNeiHiddenWord;
    _charText.color = _gameSetting.Theme.textOpenNeiHiddenWord;
    // await OpenNeighbours(runEffect);
    await UniTask.Yield();
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


  // public void CreateCoin()
  // {
  //   if (_stateManager.statePerk.needCreateCoin > 0)
  //   {
  //     for (int i = 0; i < _stateManager.statePerk.needCreateCoin; i++)
  //     {
  //       var node = _levelManager.ManagerHiddenWords.GridHelper.GetRandomNodeWithChar();
  //       if (node != null)
  //       {
  //         var coinEntity = _levelManager.ManagerHiddenWords.AddEntity(node.arrKey, TypeEntity.Coin);
  //         _stateManager.statePerk.needCreateCoin -= 1;

  //         // coinEntity.SetPosition(_levelManager.ManagerHiddenWords.tilemap.WorldToCell(gameObject.transform.position));
  //         coinEntity.SetPosition(_levelManager.ManagerHiddenWords.tilemap.WorldToCell(gameObject.transform.position));
  //       }
  //     }
  //   }
  // }

  public void AddCoin()
  {
    var nodePosition = new Vector3Int(OccupiedNode.x, OccupiedNode.y);
    var position = _levelManager.ManagerHiddenWords.tilemap.CellToWorld(nodePosition);
    var newObj = GameObject.Instantiate(
      _gameSetting.PrefabCoin,
      position,
      Quaternion.identity
    );
    var newEntity = newObj.GetComponent<BaseEntity>();
    newEntity.InitStandalone();
    newEntity.SetColor(_gameSetting.Theme.entityActiveColor);
    var positionFrom = position;
    var positionTo = _levelManager.topSide.spriteCoinPosition;
    Vector3[] waypoints = {
          positionFrom,
          positionFrom + new Vector3(1, 1),
          positionTo - new Vector3(1.5f, 2.5f),
          positionTo,
        };
    iTween.MoveTo(newObj, iTween.Hash(
      "path", waypoints,
      "time", 2,
      "easetype", iTween.EaseType.easeOutCubic,
      "oncomplete", "OnCompleteEffect"
      ));
  }

#if UNITY_EDITOR
  public override string ToString()
  {
    return "HiddenCharMB::: [text=" + charTextValue + "] \n";
  }
#endif
}
