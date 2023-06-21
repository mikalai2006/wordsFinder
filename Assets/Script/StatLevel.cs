using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StatLevel : MonoBehaviour
{
  private LevelManager _levelManager => GameManager.Instance.LevelManager;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  private StateManager _stateManager => GameManager.Instance.StateManager;
  private GameManager _gameManager => GameManager.Instance;
  private float duration => _gameSetting.timeGeneralAnimation;

  [SerializeField] private GameObject _bg;
  [SerializeField] private SpriteRenderer _spriteStar;
  [SerializeField] private GameObject _wrapStar;
  // [SerializeField] private GameObject _countStarObject;
  // private TMPro.TextMeshProUGUI _textCountStar;
  [SerializeField] private GameObject _indexStarObject;
  [SerializeField] private TMPro.TextMeshProUGUI _textStarTotalCoin;
  private TMPro.TextMeshProUGUI _indexCountStar;
  [SerializeField] private GameObject _wrapHint;
  [SerializeField] private SpriteRenderer _spriteHint;
  [SerializeField] private GameObject _indexHintObject;
  // [SerializeField] private GameObject _countHintObject;
  // private TMPro.TextMeshProUGUI _textCountHint;
  private TMPro.TextMeshProUGUI _indexCountHint;
  [SerializeField] private TMPro.TextMeshProUGUI _textHintTotalCoin;
  [SerializeField] private Button _buttonNext;
  [SerializeField] private Button _buttonDouble;

  [Space(10)]
  [Header("Total")]
  [SerializeField] private GameObject _totalObject;
  [SerializeField] private SpriteRenderer spriteCoin;
  [SerializeField] private TMPro.TextMeshProUGUI _textTotalCoin;

  private TaskCompletionSource<DataResultLevelDialog> _processCompletionSource;
  private DataResultLevelDialog _result;

  private Vector3 defaultPositionWrapper = new Vector3(0, 15, 0);
  private Vector3 _initPositionColba;
  private Vector3 _initPositionHint;
  private int _countTotalCoins;
  private int _valueTotalHintCoin;
  private int _valueTotalStarCoin;

  private void Awake()
  {
    // _textCountStar = _countStarObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    // _textCountHint = _countHintObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    _indexCountStar = _indexStarObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    _indexCountHint = _indexHintObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    SetDefault();
  }

  public async UniTask<DataResultLevelDialog> Show()
  {
    _processCompletionSource = new();
    _result = new();

    SetDefault();

    _bg.SetActive(true);

    gameObject.SetActive(true);

    Sequence mySequence = DOTween.Sequence();
    mySequence.Append(
      transform
      .DOPunchScale(new Vector3(.2f, .2f, 0), _gameSetting.timeGeneralAnimation)
      .SetEase(Ease.OutElastic)
      .OnComplete(() =>
      {
        _levelManager.shuffle.Hide();
        _levelManager.stat.Hide();
        _initPositionColba = _levelManager.buttonStar.gameObject.transform.position;
        _initPositionHint = _levelManager.buttonHint.gameObject.transform.position;
      })
    );

    var positionTo = _spriteStar.transform.position;
    Vector3[] waypoints = new[] {
      // _initPositionColba,
      _initPositionColba + new Vector3(.5f, -1f),
      // positionTo - new Vector3(0.5f, 0.5f),
      // positionTo - new Vector3(1, 1),
      positionTo,
    };
    mySequence.Append(
     _levelManager.buttonStar.gameObject.transform
       .DOJump(positionTo, 2, 1, duration)
       .SetEase(Ease.OutQuad)
   );
    // mySequence.Append(
    //   _levelManager.colba.gameObject.transform
    //     .DOPath(waypoints, duration * 2, PathType.CatmullRom)
    //     .SetEase(Ease.OutQuad)
    // );
    // mySequence.Join(_levelManager.colba.gameObject.transform.DOScale(new Vector3(3, 3, 3), duration * 2));

    mySequence.Append(
      _indexStarObject.transform
        .DOPunchScale(new Vector3(1, 1, 0), duration, 5, 1)
        .OnComplete(() =>
        {
          AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);
          _levelManager.buttonStar.HideCounter();
          _levelManager.buttonStar.ResetProgressBar();

          _indexCountStar.text = _stateManager.dataGame.activeLevel.star.ToString();
          _valueTotalStarCoin = _stateManager.dataGame.activeLevel.star * _stateManager.dataGame.activeLevel.star;
          _textStarTotalCoin.text = _valueTotalStarCoin.ToString();
        })
    );

    var positionToHint = _spriteHint.transform.position;
    // Vector3[] waypointsToHint = {
    //   _initPositionHint,
    //   _initPositionHint + new Vector3(0, 0),
    //   // positionToHint - new Vector3(0.5f, 0.5f),
    //   positionToHint,
    // };
    // mySequence.Append(
    //   _levelManager.hint.gameObject.transform
    //     .DOPath(waypointsToHint, duration * 2, PathType.Linear)
    //     .SetEase(Ease.OutQuad)
    // );

    mySequence.Append(
     _levelManager.buttonHint.gameObject.transform
       .DOJump(positionToHint, 2, 1, duration)
       .SetEase(Ease.OutQuad)
   );

    mySequence.Append(
      _indexHintObject.transform
        .DOPunchScale(new Vector3(1, 1, 0), duration, 5, 1)
        .OnComplete(() =>
        {
          AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);
          _levelManager.buttonHint.HideCounter();
          _levelManager.buttonHint.ResetProgressBar();

          _indexCountHint.text = _stateManager.dataGame.activeLevel.hint.ToString();

          _valueTotalHintCoin = _stateManager.dataGame.activeLevel.hint * _stateManager.dataGame.activeLevel.hint;

          _textHintTotalCoin.text = _valueTotalHintCoin.ToString();
          _totalObject.SetActive(true);
        })
    );
    mySequence.Append(
      _totalObject.transform
        .DOPunchScale(new Vector3(1, 1, 0), duration, 5, 1)
        .OnComplete(() =>
        {
          AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);
          _countTotalCoins = _valueTotalHintCoin + _valueTotalStarCoin;
          _textTotalCoin.text = _countTotalCoins.ToString();

          _buttonNext.gameObject.SetActive(true);
          _buttonDouble.gameObject.SetActive(true);
        })
    );
    // mySequence.AppendCallback(() =>
    // {
    //   ShowStarWrap();
    // });
    // transform.DOMove(new Vector3(0, 0, 0), 1.5f).SetEase(Ease.OutElastic).OnComplete(ShowStarWrap);

    return await _processCompletionSource.Task;
  }

  // private void ShowStarWrap()
  // {
  //   _wrapStar.transform
  //     .DOMove(new Vector3(0, -3, 0), duration / 2)
  //     .From()
  //     .OnComplete(async () => await AnimateStarStat());

  // }

  // public async UniTask AnimateStarStat()
  // {
  //   _initPositionColba = _levelManager.colba.gameObject.transform.position;
  //   var positionTo = _spriteStar.transform.position;
  //   _levelManager.colba.gameObject.transform.DOMove(positionTo, duration).SetEase(Ease.Linear);

  //   await UniTask.Delay((int)(duration * 500));

  //   if (_stateManager.dataGame.activeLevel.star > 0)
  //   {
  //     _indexStarObject.transform.DOPunchScale(new Vector3(1, 1, 0), duration, 5, 1);
  //     _levelManager.colba.HideCounter();
  //     _levelManager.colba.ResetProgressBar();

  //     _indexCountStar.text = _stateManager.dataGame.activeLevel.star.ToString();

  //     AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);

  //     await UniTask.Delay((int)(duration * 500));

  //     int valueTotalStarCoin = _stateManager.dataGame.activeLevel.star * _stateManager.dataGame.activeLevel.star;

  //     _textStarTotalCoin.text = valueTotalStarCoin.ToString();

  //     AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);

  //   }

  //   ShowHintWrap();
  // }

  // private void ShowHintWrap()
  // {
  //   _wrapHint.SetActive(true);

  //   _wrapHint.transform
  //     .DOMove(new Vector3(0, -3, 0), duration / 2)
  //     .From()
  //     .OnComplete(async () => await AnimateHitStat());
  // }

  // public async UniTask AnimateHitStat()
  // {
  //   _initPositionHint = _levelManager.hint.gameObject.transform.position;
  //   var positionTo = _spriteHint.transform.position;
  //   _levelManager.hint.gameObject.transform.DOMove(positionTo, duration).SetEase(Ease.Linear);
  //   // Vector3[] waypoints = {
  //   //       _initPositionHint,
  //   //       // _initPositionHint + new Vector3(1, 1),
  //   //       positionTo - new Vector3(0.5f, 0.5f),
  //   //       positionTo,
  //   //     };
  //   // _levelManager.hint.gameObject.transform.DOPath(waypoints, duration, PathType.Linear).SetEase(Ease.Linear);

  //   await UniTask.Delay((int)(duration * 500));

  //   if (_stateManager.dataGame.activeLevel.hint > 0)
  //   {
  //     _indexHintObject.transform.DOPunchScale(new Vector3(1, 1, 0), duration, 5, 1);
  //     _levelManager.hint.HideCounter();
  //     _levelManager.hint.ResetProgressBar();

  //     _indexCountHint.text = _stateManager.dataGame.activeLevel.hint.ToString();

  //     AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);

  //     await UniTask.Delay((int)(duration * 500));

  //     int valueTotalHintCoin = _stateManager.dataGame.activeLevel.hint * _stateManager.dataGame.activeLevel.hint;

  //     _textHintTotalCoin.text = valueTotalHintCoin.ToString();

  //     AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);
  //   }

  //   await AnimateTotal();
  // }

  // private async UniTask AnimateTotal()
  // {
  //   await UniTask.Delay((int)(duration * 500));
  //   _totalObject.SetActive(true);
  //   _textTotalCoin.text = (int.Parse(_textHintTotalCoin.text) + int.Parse(_textStarTotalCoin.text)).ToString();

  //   await UniTask.Delay((int)(duration * 500));

  //   _buttonNext.gameObject.SetActive(true);
  //   _buttonDouble.gameObject.SetActive(true);
  // }

  public void OnClickDoubleButton()
  {
    OnDoubleCoins();
  }

  public void OnDoubleCoins()
  {
    _countTotalCoins *= 2;
    _textTotalCoin.text = _countTotalCoins.ToString();
  }

  private void GoCoins()
  {
    var newObj = GameObject.Instantiate(
      _gameSetting.PrefabCoin,
      _levelManager.statLevel.spriteCoin.transform.position,
      Quaternion.identity
    );
    var newEntity = newObj.GetComponent<BaseEntity>();
    newEntity.InitStandalone();
    newEntity.SetColor(_gameSetting.Theme.entityActiveColor);
    var positionFrom = _levelManager.statLevel.spriteCoin.transform.position;// transform.position;
    var positionTo = _levelManager.topSide.spriteCoinPosition;
    Vector3[] waypoints = {
          positionFrom,
          positionFrom + new Vector3(-1.5f, 1.5f),
          positionTo - new Vector3(1.5f, 1.5f),
          positionTo,
        };

    newObj.gameObject.transform
      .DOPath(waypoints, 1f, PathType.CatmullRom)
      .SetEase(Ease.OutCubic)
      .OnComplete(() => newEntity.AddCoins(_countTotalCoins));
  }

  public async void Next()
  {
    GoCoins();

    _levelManager.buttonHint.gameObject.transform
      .DOJump(_initPositionHint, 2, 1, duration)
      .SetEase(Ease.OutQuad);
    _levelManager.buttonStar.gameObject.transform
       .DOJump(_initPositionColba, 2, 1, duration)
       .SetEase(Ease.OutQuad);
    // _levelManager.hint.gameObject.transform.DOMove(_initPositionHint, duration).SetEase(Ease.Linear);
    // _levelManager.colba.gameObject.transform.DOMove(_initPositionColba, duration).SetEase(Ease.Linear);

    await UniTask.Delay(500);
    transform
      .DOMove(defaultPositionWrapper, duration)
      .SetEase(Ease.Linear)
      .OnComplete(() =>
      {
        gameObject.SetActive(false);

      });

    _levelManager.shuffle.Show();
    _levelManager.stat.Show();

    await UniTask.Delay(500);

    _bg.SetActive(false);
    SetDefault();

    _result.isOk = true;
    _processCompletionSource.SetResult(_result);
  }

  private void SetDefault()
  {
    gameObject.SetActive(false);
    gameObject.transform.position = new Vector3(0, 0, 0);
    _bg.SetActive(false);
    _buttonNext.gameObject.SetActive(false);
    _buttonDouble.gameObject.SetActive(false);
    // _textCountStar.text = "";
    // _textCountHint.text = "";
    _indexCountStar.text = "?";
    _indexCountHint.text = "?";
    _textStarTotalCoin.text = "?";
    _textHintTotalCoin.text = "?";
    _textTotalCoin.text = "?";
    // _wrapStar.SetActive(false);
    // _wrapHint.SetActive(false);
    _totalObject.SetActive(false);
  }
}
