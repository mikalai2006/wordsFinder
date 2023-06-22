// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Cysharp.Threading.Tasks;
// using DG.Tweening;
// using UnityEngine;
// using UnityEngine.UI;

// public class StatLevel : MonoBehaviour
// {
//   private LevelManager _levelManager => GameManager.Instance.LevelManager;
//   private GameSetting _gameSetting => GameManager.Instance.GameSettings;
//   private StateManager _stateManager => GameManager.Instance.StateManager;
//   private GameManager _gameManager => GameManager.Instance;
//   private float duration => _gameSetting.timeGeneralAnimation;

//   [SerializeField] private GameObject _bg;
//   [SerializeField] private TMPro.TextMeshProUGUI _textFoundDopWords;
//   [SerializeField] private TMPro.TextMeshProUGUI _textHeader;
//   [SerializeField] private TMPro.TextMeshProUGUI _textMessage;
//   // [SerializeField] private SpriteRenderer _spriteStar;
//   // [SerializeField] private GameObject _wrapStar;
//   // [SerializeField] private GameObject _countStarObject;
//   // private TMPro.TextMeshProUGUI _textCountStar;
//   [SerializeField] private GameObject _indexStarObject;
//   [SerializeField] private TMPro.TextMeshProUGUI _textStarTotalCoin;
//   private TMPro.TextMeshProUGUI _indexCountStar;
//   // [SerializeField] private GameObject _wrapHint;
//   // [SerializeField] private SpriteRenderer _spriteHint;
//   [SerializeField] private GameObject _indexHintObject;
//   // [SerializeField] private GameObject _countHintObject;
//   // private TMPro.TextMeshProUGUI _textCountHint;
//   private TMPro.TextMeshProUGUI _indexCountHint;
//   [SerializeField] private TMPro.TextMeshProUGUI _textHintTotalCoin;
//   [SerializeField] private Button _buttonNext;
//   [SerializeField] private Button _buttonDouble;
//   [SerializeField] private Button _buttonOk;

//   [Space(10)]
//   [Header("Total")]
//   [SerializeField] private GameObject _totalObject;
//   [SerializeField] private SpriteRenderer spriteCoin;
//   [SerializeField] private TMPro.TextMeshProUGUI _textTotalCoin;

//   private TaskCompletionSource<DataResultUIDialog> _processCompletionSource;
//   private DataResultUIDialog _result;

//   private Vector3 defaultPositionWrapper = new Vector3(0, 15, 0);
//   // private Vector3 _initPositionColba;
//   // private Vector3 _initPositionHint;
//   private int _countTotalCoins;
//   // private int _valueTotalHintCoin;
//   // private int _valueTotalStarCoin;

//   private void Awake()
//   {
//     // _textCountStar = _countStarObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
//     // _textCountHint = _countHintObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
//     _indexCountStar = _indexStarObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
//     _indexCountHint = _indexHintObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
//     SetDefault();
//   }

//   public async UniTask<DataResultUIDialog> Show()
//   {
//     _processCompletionSource = new();
//     _result = new();

//     SetDefault();

//     _bg.SetActive(true);

//     gameObject.SetActive(true);

//     _countTotalCoins = _stateManager.dataGame.activeLevel.countDopWords;
//     _textFoundDopWords.text = await Helpers.GetLocalizedPluralString(
//           "completelevel_d",
//            new Dictionary<string, object> {
//             {"count", _stateManager.dataGame.activeLevel.countDopWords},
//           }
//         );

//     Sequence mySequence = DOTween.Sequence();
//     mySequence.Append(
//       transform
//       .DOPunchScale(new Vector3(.2f, .2f, 0), _gameSetting.timeGeneralAnimation)
//       .SetEase(Ease.OutElastic)
//       .OnComplete(() =>
//       {
//         // _initPositionColba = _levelManager.buttonStar.gameObject.transform.position;
//         // _initPositionHint = _levelManager.buttonHint.gameObject.transform.position;
//         _totalObject.SetActive(true);
//       })
//     );

//     mySequence.Append(
//       _totalObject.transform
//         .DOPunchScale(new Vector3(1, 1, 0), duration, 5, 1)
//         .OnComplete(() =>
//         {
//           AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);
//           _textTotalCoin.text = _countTotalCoins.ToString();

//           _buttonNext.gameObject.SetActive(true);
//           _buttonDouble.gameObject.SetActive(true);

//           _buttonNext.transform
//             .DOPunchScale(new Vector3(.5f, .5f, 0), _gameSetting.timeGeneralAnimation)
//             .SetEase(Ease.OutBack);
//           _buttonDouble.transform
//             .DOPunchScale(new Vector3(.5f, .5f, 0), _gameSetting.timeGeneralAnimation)
//             .SetEase(Ease.OutBack);
//         })
//     );
//     return await _processCompletionSource.Task;
//   }

//   public async UniTask<DataResultUIDialog> ShowStart()
//   {
//     _processCompletionSource = new();
//     _result = new();

//     SetDefault();

//     _bg.SetActive(true);
//     _buttonOk.gameObject.SetActive(true);
//     gameObject.SetActive(true);

//     _countTotalCoins = _stateManager.dataGame.activeLevel.countDopWords;
//     _textFoundDopWords.text = await Helpers.GetLocalizedPluralString(
//           "completelevel_d",
//            new Dictionary<string, object> {
//             {"count", _stateManager.dataGame.activeLevel.countDopWords},
//           }
//         );

//     Sequence mySequence = DOTween.Sequence();
//     mySequence.Append(
//       transform
//       .DOPunchScale(new Vector3(.2f, .2f, 0), _gameSetting.timeGeneralAnimation)
//       .SetEase(Ease.OutElastic)
//     );

//     return await _processCompletionSource.Task;
//   }


//   public void OnClickDoubleButton()
//   {
//     OnDoubleCoins();
//   }

//   public void OnDoubleCoins()
//   {
//     _countTotalCoins *= 2;
//     _textTotalCoin.text = _countTotalCoins.ToString();
//   }

//   private void GoCoins()
//   {
//     var newObj = GameObject.Instantiate(
//       _gameSetting.PrefabCoin,
//       spriteCoin.transform.position,
//       Quaternion.identity
//     );
//     var newEntity = newObj.GetComponent<BaseEntity>();
//     newEntity.InitStandalone();
//     newEntity.SetColor(_gameSetting.Theme.entityActiveColor);
//     var positionFrom = spriteCoin.transform.position;// transform.position;
//     var positionTo = _levelManager.topSide.spriteCoinPosition;
//     Vector3[] waypoints = {
//           positionFrom,
//           positionFrom + new Vector3(-1.5f, 1.5f),
//           positionTo - new Vector3(1.5f, 1.5f),
//           positionTo,
//         };

//     newObj.gameObject.transform
//       .DOPath(waypoints, 1f, PathType.CatmullRom)
//       .SetEase(Ease.OutCubic)
//       .OnComplete(() => newEntity.AddCoins(_countTotalCoins));
//   }

//   public async void Next()
//   {
//     GoCoins();

//     // _levelManager.buttonHint.gameObject.transform
//     //   .DOJump(_initPositionHint, 2, 1, duration)
//     //   .SetEase(Ease.OutQuad);
//     // _levelManager.buttonStar.gameObject.transform
//     //    .DOJump(_initPositionColba, 2, 1, duration)
//     //    .SetEase(Ease.OutQuad);
//     // // _levelManager.hint.gameObject.transform.DOMove(_initPositionHint, duration).SetEase(Ease.Linear);
//     // // _levelManager.colba.gameObject.transform.DOMove(_initPositionColba, duration).SetEase(Ease.Linear);

//     await UniTask.Delay(500);
//     transform
//       .DOMove(defaultPositionWrapper, duration)
//       .SetEase(Ease.Linear)
//       .OnComplete(() =>
//       {
//         gameObject.SetActive(false);

//       });

//     _levelManager.buttonShuffle.Show();
//     _levelManager.stat.Show();

//     await UniTask.Delay(500);

//     _bg.SetActive(false);
//     SetDefault();

//     _result.isOk = true;
//     _processCompletionSource.SetResult(_result);
//   }

//   public async void NextStart()
//   {
//     transform
//       .DOMove(defaultPositionWrapper, duration)
//       .SetEase(Ease.Linear)
//       .OnComplete(() =>
//       {
//         gameObject.SetActive(false);

//       });

//     // _levelManager.buttonShuffle.Show();
//     // _levelManager.stat.Show();

//     await UniTask.Delay(500);

//     _bg.SetActive(false);
//     SetDefault();

//     _result.isOk = true;
//     _processCompletionSource.SetResult(_result);
//   }

//   private void SetDefault()
//   {
//     _textFoundDopWords.color = _gameSetting.Theme.colorSecondary;
//     _textMessage.color = _gameSetting.Theme.colorPrimary;
//     _textHeader.color = _gameSetting.Theme.colorSecondary;

//     _textFoundDopWords.color = _gameSetting.Theme.colorPrimary;
//     gameObject.SetActive(false);
//     gameObject.transform.position = new Vector3(0, 0, 0);
//     _bg.SetActive(false);
//     _buttonNext.gameObject.SetActive(false);
//     _buttonDouble.gameObject.SetActive(false);
//     _buttonOk.gameObject.SetActive(false);
//     // _textCountStar.text = "";
//     // _textCountHint.text = "";
//     _indexCountStar.text = "?";
//     _indexCountHint.text = "?";
//     _textStarTotalCoin.text = "?";
//     _textHintTotalCoin.text = "?";
//     _textTotalCoin.text = "?";
//     // _wrapStar.SetActive(false);
//     // _wrapHint.SetActive(false);
//     _totalObject.SetActive(false);
//   }


//   //   var positionTo = _spriteStar.transform.position;
//   //   Vector3[] waypoints = new[] {
//   //     // _initPositionColba,
//   //     _initPositionColba + new Vector3(.5f, -1f),
//   //     // positionTo - new Vector3(0.5f, 0.5f),
//   //     // positionTo - new Vector3(1, 1),
//   //     positionTo,
//   //   };
//   //   mySequence.Append(
//   //    _levelManager.buttonStar.gameObject.transform
//   //      .DOJump(positionTo, 2, 1, duration)
//   //      .SetEase(Ease.OutQuad)
//   //  );

//   // mySequence.Append(
//   //   _indexStarObject.transform
//   //     .DOPunchScale(new Vector3(1, 1, 0), duration, 5, 1)
//   //     .OnComplete(() =>
//   //     {
//   //       AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);
//   //       _levelManager.buttonStar.HideCounter();
//   //       _levelManager.buttonStar.ResetProgressBar();

//   //       _indexCountStar.text = _stateManager.dataGame.activeLevel.star.ToString();
//   //       _valueTotalStarCoin = _stateManager.dataGame.activeLevel.star * _stateManager.dataGame.activeLevel.star;
//   //       _textStarTotalCoin.text = _valueTotalStarCoin.ToString();
//   //     })
//   // );

//   //   var positionToHint = _spriteHint.transform.position;

//   //   mySequence.Append(
//   //    _levelManager.buttonHint.gameObject.transform
//   //      .DOJump(positionToHint, 2, 1, duration)
//   //      .SetEase(Ease.OutQuad)
//   //  );

//   //   mySequence.Append(
//   //     _indexHintObject.transform
//   //       .DOPunchScale(new Vector3(1, 1, 0), duration, 5, 1)
//   //       .OnComplete(() =>
//   //       {
//   //         AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.calculateCoin);
//   //         _levelManager.buttonHint.HideCounter();
//   //         _levelManager.buttonHint.ResetProgressBar();

//   //         _indexCountHint.text = _stateManager.dataGame.activeLevel.hint.ToString();

//   //         _valueTotalHintCoin = _stateManager.dataGame.activeLevel.hint * _stateManager.dataGame.activeLevel.hint;

//   //         _textHintTotalCoin.text = _valueTotalHintCoin.ToString();
//   //         _totalObject.SetActive(true);
//   //       })
//   //   );

// }
