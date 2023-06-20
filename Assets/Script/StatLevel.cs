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
  [SerializeField] private GameObject _totalObject;

  [Space(10)]
  [Header("Total")]
  [SerializeField] private SpriteRenderer spriteCoin;
  [SerializeField] private TMPro.TextMeshProUGUI _textTotalCoin;

  private TaskCompletionSource<DataResultLevelDialog> _processCompletionSource;
  private DataResultLevelDialog _result;

  private Vector3 defaultPositionWrapper = new Vector3(0, 15, 0);
  private Vector3 _initPositionColba;
  private Vector3 _initPositionHint;

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

    // Sequence mySequence = DOTween.Sequence();

    // mySequence.Append(transform.DOMove(new Vector3(0, 0, 0), 2f).SetEase(Ease.OutElastic));
    // mySequence.AppendCallback(() =>
    // {
    //   ShowStarWrap();
    // });
    transform.DOMove(new Vector3(0, 0, 0), 1.5f).SetEase(Ease.OutElastic).OnComplete(ShowStarWrap);
    _levelManager.shuffle.Hide();
    _levelManager.stat.Hide();

    return await _processCompletionSource.Task;
  }

  private void ShowStarWrap()
  {
    _wrapStar.SetActive(true);

    _wrapStar.transform
      .DOMove(new Vector3(0, -3, 0), duration / 2)
      .From()
      .OnComplete(async () => await AnimateStarStat());

  }

  public async UniTask AnimateStarStat()
  {
    _initPositionColba = _levelManager.colba.gameObject.transform.position;
    var positionTo = _spriteStar.transform.position;
    _levelManager.colba.gameObject.transform.DOMove(positionTo, duration).SetEase(Ease.OutBounce);

    await UniTask.Delay((int)(duration * 500));

    if (_stateManager.dataGame.activeLevelWord.star > 0)
    {
      _indexStarObject.transform.DOPunchScale(new Vector3(1, 1, 0), duration, 5, 1);
      _levelManager.colba.HideCounter();
      _levelManager.colba.ResetProgressBar();

      _indexCountStar.text = _stateManager.dataGame.activeLevelWord.star.ToString();

      AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);

      await UniTask.Delay((int)(duration * 500));

      int valueTotalStarCoin = _stateManager.dataGame.activeLevelWord.star * _stateManager.dataGame.activeLevelWord.star;

      _textStarTotalCoin.text = valueTotalStarCoin.ToString();

      AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);

    }

    ShowHintWrap();
  }

  private void ShowHintWrap()
  {
    _wrapHint.SetActive(true);

    _wrapHint.transform
      .DOMove(new Vector3(0, -3, 0), duration / 2)
      .From()
      .OnComplete(async () => await AnimateHitStat());
  }

  public async UniTask AnimateHitStat()
  {
    _initPositionHint = _levelManager.hint.gameObject.transform.position;
    var positionTo = _spriteHint.transform.position;
    _levelManager.hint.gameObject.transform.DOMove(positionTo, duration).SetEase(Ease.OutBounce);
    // Vector3[] waypoints = {
    //       _initPositionHint,
    //       // _initPositionHint + new Vector3(1, 1),
    //       // positionTo - new Vector3(1.5f, 1.5f),
    //       positionTo,
    //     };
    // _levelManager.hint.gameObject.transform.DOPath(waypoints, duration / 2, PathType.Linear);

    await UniTask.Delay((int)(duration * 500));

    if (_stateManager.dataGame.activeLevelWord.hint > 0)
    {
      _indexHintObject.transform.DOPunchScale(new Vector3(1, 1, 0), duration, 5, 1);
      _levelManager.hint.HideCounter();
      _levelManager.hint.ResetProgressBar();

      _indexCountHint.text = _stateManager.dataGame.activeLevelWord.hint.ToString();

      AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);

      await UniTask.Delay((int)(duration * 500));

      int valueTotalHintCoin = _stateManager.dataGame.activeLevelWord.hint * _stateManager.dataGame.activeLevelWord.hint;

      _textHintTotalCoin.text = valueTotalHintCoin.ToString();

      AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);
    }

    await AnimateTotal();
  }

  private async UniTask AnimateTotal()
  {
    await UniTask.Delay((int)(duration * 500));
    _totalObject.SetActive(true);
    _textTotalCoin.text = (int.Parse(_textHintTotalCoin.text) + int.Parse(_textStarTotalCoin.text)).ToString();

    await UniTask.Delay((int)(duration * 500));

    _buttonNext.gameObject.SetActive(true);
    _buttonDouble.gameObject.SetActive(true);
  }

  private void GoCoins()
  {
    var countNotUseHint = int.Parse(_textTotalCoin.text);

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
      .DOPath(waypoints, 1f, PathType.Linear)
      .SetEase(Ease.OutCubic)
      .OnComplete(() => newEntity.AddCoins(countNotUseHint));
  }

  public async void Next()
  {
    GoCoins();

    transform.DOMove(defaultPositionWrapper, 1f).SetEase(Ease.Linear);
    _levelManager.hint.gameObject.transform.DOMove(_initPositionHint, duration).SetEase(Ease.Linear);
    _levelManager.colba.gameObject.transform.DOMove(_initPositionColba, duration).SetEase(Ease.Linear);

    _levelManager.shuffle.Show();
    _levelManager.stat.Show();

    await UniTask.Delay(1000);

    _bg.SetActive(false);
    SetDefault();

    _result.isOk = true;
    _processCompletionSource.SetResult(_result);
  }

  private void SetDefault()
  {
    gameObject.transform.position = defaultPositionWrapper;
    _bg.SetActive(false);
    _buttonNext.gameObject.SetActive(false);
    _buttonDouble.gameObject.SetActive(false);
    // _textCountStar.text = "";
    // _textCountHint.text = "";
    _indexCountStar.text = "1";
    _indexCountHint.text = "1";
    _textStarTotalCoin.text = "0";
    _textHintTotalCoin.text = "0";
    _textTotalCoin.text = "0";
    _wrapStar.SetActive(false);
    _wrapHint.SetActive(false);
    _totalObject.SetActive(false);
  }
}
