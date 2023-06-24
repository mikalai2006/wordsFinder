using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DialogLevel : MonoBehaviour
{
  private LevelManager _levelManager => GameManager.Instance.LevelManager;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  private StateManager _stateManager => GameManager.Instance.StateManager;
  private GameManager _gameManager => GameManager.Instance;
  private float duration => _gameSetting.timeGeneralAnimation;

  [SerializeField] private GameObject _bg;
  [SerializeField] private TMPro.TextMeshProUGUI _textMessageSmall;
  [SerializeField] private TMPro.TextMeshProUGUI _textHeader;
  [SerializeField] private TMPro.TextMeshProUGUI _textMessage;
  // [SerializeField] private SpriteRenderer _spriteStar;
  // [SerializeField] private GameObject _wrapStar;
  // [SerializeField] private GameObject _countStarObject;
  // private TMPro.TextMeshProUGUI _textCountStar;
  // [SerializeField] private GameObject _indexStarObject;
  // [SerializeField] private TMPro.TextMeshProUGUI _textStarTotalCoin;
  // private TMPro.TextMeshProUGUI _indexCountStar;
  // [SerializeField] private GameObject _wrapHint;
  // [SerializeField] private SpriteRenderer _spriteHint;
  // [SerializeField] private GameObject _indexHintObject;
  // [SerializeField] private GameObject _countHintObject;
  // private TMPro.TextMeshProUGUI _textCountHint;
  // private TMPro.TextMeshProUGUI _indexCountHint;
  // [SerializeField] private TMPro.TextMeshProUGUI _textHintTotalCoin;
  [SerializeField] private Button _buttonNext;
  [SerializeField] private Button _buttonDouble;
  [SerializeField] private Button _buttonOk;

  [Space(10)]
  [Header("Total")]
  [SerializeField] private GameObject _totalObject;
  [SerializeField] private SpriteRenderer spriteCoin;
  [SerializeField] private TMPro.TextMeshProUGUI _textTotalCoin;

  private TaskCompletionSource<DataDialogResult> _processCompletionSource;
  private DataDialogResult _result;

  private Vector3 defaultPositionWrapper = new Vector3(0, 20, 0);
  private Vector3 visiblePositionWrapper = new Vector3(0, 6.5f, 0);
  // private Vector3 _initPositionColba;
  // private Vector3 _initPositionHint;
  private int _countTotalCoins;
  // private int _valueTotalHintCoin;
  // private int _valueTotalStarCoin;

  private void Awake()
  {
    // // _textCountStar = _countStarObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    // // _textCountHint = _countHintObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    // _indexCountStar = _indexStarObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    // _indexCountHint = _indexHintObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    SetDefault();
  }

  public async UniTask<DataDialogResult> ShowDialogEndRound()
  {
    _processCompletionSource = new();
    _result = new();

    SetDefault();
    _buttonNext.gameObject.SetActive(true);
    _buttonDouble.gameObject.SetActive(true);

    _textHeader.text = await Helpers.GetLocalizedPluralString(
      "roundresult",
        new Dictionary<string, int> {
        {"value", _stateManager.dataGame.completeWords.Count + 1},
      }
    );

    _textMessage.text = await Helpers.GetLocaledString("completelevel");
    _countTotalCoins = _stateManager.dataGame.activeLevel.countDopWords;
    _textMessageSmall.text = await Helpers.GetLocalizedPluralString(
          "completelevel_d",
           new Dictionary<string, object> {
            {"count", _stateManager.dataGame.activeLevel.countDopWords},
          }
        );


    _bg.SetActive(true);
    gameObject.SetActive(true);

    // var initScale = transform.localScale;
    Sequence mySequence = DOTween.Sequence();
    mySequence.Append(
      transform
      .DOMove(visiblePositionWrapper, duration * 2)
      .From(defaultPositionWrapper, true)
      .SetEase(Ease.OutBack)
      .OnComplete(() =>
      {
        _totalObject.SetActive(true);
        _textTotalCoin.text = _countTotalCoins.ToString();
        AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);
      })
    );

    // transform
    //   .DOScale(initScale, duration * 3)
    //   .From(Vector3.zero)
    //   .SetEase(Ease.OutElastic)
    //   .OnComplete(() =>
    //   {
    //     _totalObject.SetActive(true);
    //     _textTotalCoin.text = _countTotalCoins.ToString();
    //     AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);
    //   })
    mySequence.Append(
      _textTotalCoin.transform
        .DOPunchScale(new Vector3(.1f, .1f, 0), duration, 5, 1)
    );
    return await _processCompletionSource.Task;
  }

  public void OnClickDoubleButton()
  {
    OnDoubleCoins();
  }

  public void OnDoubleCoins()
  {
    _countTotalCoins *= 2;
    _textTotalCoin.text = _countTotalCoins.ToString();
  }

  private async UniTask GoCoins()
  {
    GameEntity configEntity = _gameManager.ResourceSystem.GetAllEntity().Find(t => t.typeEntity == TypeEntity.Coin);
    // var newObj = GameObject.Instantiate(
    //   configEntity.prefab,
    //   spriteCoin.transform.position,
    //   Quaternion.identity
    // );
    // var newEntity = newObj.GetComponent<BaseEntity>();
    // newEntity.InitStandalone();
    // newEntity.SetColor(_gameSetting.Theme.entityActiveColor);
    // var positionFrom = spriteCoin.transform.position;// transform.position;
    // var positionTo = _levelManager.topSide.spriteCoinPosition;
    // Vector3[] waypoints = {
    //       positionFrom,
    //       positionFrom + new Vector3(-1.5f, 1.5f),
    //       positionTo - new Vector3(1.5f, 1.5f),
    //       positionTo,
    //     };

    // newObj.gameObject.transform
    //   .DOPath(waypoints, 1f, PathType.CatmullRom)
    //   .SetEase(Ease.OutCubic)
    //   .OnComplete(() => newEntity.AddCoins(_countTotalCoins));
    await _levelManager.CreateCoin(transform.position);
  }

  public async void CloseDialogEndRound()
  {
    await GoCoins();

    // _levelManager.buttonHint.gameObject.transform
    //   .DOJump(_initPositionHint, 2, 1, duration)
    //   .SetEase(Ease.OutQuad);
    // _levelManager.buttonStar.gameObject.transform
    //    .DOJump(_initPositionColba, 2, 1, duration)
    //    .SetEase(Ease.OutQuad);
    // // _levelManager.hint.gameObject.transform.DOMove(_initPositionHint, duration).SetEase(Ease.Linear);
    // // _levelManager.colba.gameObject.transform.DOMove(_initPositionColba, duration).SetEase(Ease.Linear);

    // await UniTask.Delay(500);
    transform
      .DOMove(defaultPositionWrapper, duration * 2)
      .SetEase(Ease.InBack)
      .OnComplete(() =>
      {
        // gameObject.SetActive(false);
        // transform.localPosition = defaultPositionWrapper;
        _buttonNext.gameObject.SetActive(false);
        _buttonDouble.gameObject.SetActive(false);
        _totalObject.SetActive(false);
      });

    await UniTask.Delay(1000);

    _result.isOk = true;
    _processCompletionSource.SetResult(_result);
  }


  public async UniTask<DataDialogResult> ShowDialogStartRound()
  {
    _processCompletionSource = new();
    _result = new();

    SetDefault();

    _textHeader.text = string.Format("{0} {1}",
      await Helpers.GetLocaledString("round"),
      _stateManager.dataGame.completeWords.Count + 1
    );
    _textMessage.text = "";

    var countHints = _stateManager.dataGame.activeLevel.hintLevel + _stateManager.dataGame.activeLevel.starLevel;
    var textMessageHints = await Helpers.GetLocalizedPluralString(
        "givestarthints",
        new Dictionary<string, object> {
          {"count", countHints}
        }
      );
    if (countHints == 0)
    {
      textMessageHints = await Helpers.GetLocalizedPluralString(
        "givestarthints_no",
        new Dictionary<string, object> {
          {"count", countHints}
        }
      );
    }
    _textMessageSmall.text = string.Format("{0}\r\n{1}\r\n{2}",
      await Helpers.GetLocaledString("policyround"),
      await Helpers.GetLocalizedPluralString(
        "roundconditdesc",
        new Dictionary<string, object> {
          {"count", _stateManager.dataGame.activeLevel.countNeedWords},
          {"count2", _levelManager.ManagerHiddenWords.AllowPotentialWords.Count}
        }
      ),
      textMessageHints
    );

    _bg.SetActive(true);
    _buttonOk.gameObject.SetActive(true);
    gameObject.SetActive(true);

    transform
      .DOMove(visiblePositionWrapper, duration * 2)
      .From(defaultPositionWrapper, true)
      .SetEase(Ease.OutBack)
      .OnComplete(() =>
      {

      });

    // transform
    //   .DOMove(Vector3.zero, duration)
    //   .SetEase(Ease.OutCubic);
    // // transform
    // // .DOPunchScale(new Vector3(.2f, .2f, 0), _gameSetting.timeGeneralAnimation)
    // // .SetEase(Ease.OutElastic);

    return await _processCompletionSource.Task;
  }

  public void CloseDialogStartRound()
  {
    transform
      .DOMove(defaultPositionWrapper, duration * 2)
      .SetEase(Ease.InBack)
      .OnComplete(() =>
      {
        gameObject.SetActive(false);
        _bg.SetActive(false);
        SetDefault();

        // _levelManager.buttonShuffle.Show();
        // _levelManager.stat.Show();
        _bg.SetActive(false);


        _result.isOk = true;
        _processCompletionSource.SetResult(_result);
      });

    // _levelManager.buttonShuffle.Show();
    // _levelManager.stat.Show();
  }

  private void SetDefault()
  {
    _textMessageSmall.color = _gameSetting.Theme.colorSecondary;
    _textMessage.color = _gameSetting.Theme.colorPrimary;
    _textHeader.color = _gameSetting.Theme.colorSecondary;

    _textMessageSmall.color = _gameSetting.Theme.colorPrimary;
    gameObject.SetActive(false);
    gameObject.transform.position = defaultPositionWrapper;
    _bg.SetActive(false);
    _buttonNext.gameObject.SetActive(false);
    _buttonDouble.gameObject.SetActive(false);
    _totalObject.SetActive(false);
    _buttonOk.gameObject.SetActive(false);
    // _textCountStar.text = "";
    // _textCountHint.text = "";
    // _indexCountStar.text = "?";
    // _indexCountHint.text = "?";
    // _textStarTotalCoin.text = "?";
    // _textHintTotalCoin.text = "?";
    _textTotalCoin.text = "";
    // _wrapStar.SetActive(false);
    // _wrapHint.SetActive(false);
  }


  //   var positionTo = _spriteStar.transform.position;
  //   Vector3[] waypoints = new[] {
  //     // _initPositionColba,
  //     _initPositionColba + new Vector3(.5f, -1f),
  //     // positionTo - new Vector3(0.5f, 0.5f),
  //     // positionTo - new Vector3(1, 1),
  //     positionTo,
  //   };
  //   mySequence.Append(
  //    _levelManager.buttonStar.gameObject.transform
  //      .DOJump(positionTo, 2, 1, duration)
  //      .SetEase(Ease.OutQuad)
  //  );

  // mySequence.Append(
  //   _indexStarObject.transform
  //     .DOPunchScale(new Vector3(1, 1, 0), duration, 5, 1)
  //     .OnComplete(() =>
  //     {
  //       AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);
  //       _levelManager.buttonStar.HideCounter();
  //       _levelManager.buttonStar.ResetProgressBar();

  //       _indexCountStar.text = _stateManager.dataGame.activeLevel.star.ToString();
  //       _valueTotalStarCoin = _stateManager.dataGame.activeLevel.star * _stateManager.dataGame.activeLevel.star;
  //       _textStarTotalCoin.text = _valueTotalStarCoin.ToString();
  //     })
  // );

  //   var positionToHint = _spriteHint.transform.position;

  //   mySequence.Append(
  //    _levelManager.buttonHint.gameObject.transform
  //      .DOJump(positionToHint, 2, 1, duration)
  //      .SetEase(Ease.OutQuad)
  //  );

  //   mySequence.Append(
  //     _indexHintObject.transform
  //       .DOPunchScale(new Vector3(1, 1, 0), duration, 5, 1)
  //       .OnComplete(() =>
  //       {
  //         AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);
  //         _levelManager.buttonHint.HideCounter();
  //         _levelManager.buttonHint.ResetProgressBar();

  //         _indexCountHint.text = _stateManager.dataGame.activeLevel.hint.ToString();

  //         _valueTotalHintCoin = _stateManager.dataGame.activeLevel.hint * _stateManager.dataGame.activeLevel.hint;

  //         _textHintTotalCoin.text = _valueTotalHintCoin.ToString();
  //         _totalObject.SetActive(true);
  //       })
  //   );

}
