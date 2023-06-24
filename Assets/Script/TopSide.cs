using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TopSide : MonoBehaviour
{
  private LevelManager _levelManager => GameManager.Instance.LevelManager;
  private GameManager _gameManager => GameManager.Instance;
  private Vector3 _initialScaleSpriteCoin;
  private Vector3 _initialPositionSpriteCoin;
  private Vector3 _initialScaleSpriteRate;
  private Vector3 _initialPositionSpriteRate;
  [SerializeField] private TMPro.TextMeshProUGUI _statusText;
  [SerializeField] private Image _spriteRate;
  [SerializeField] private TMPro.TextMeshProUGUI _rate;
  [SerializeField] private Image _spriteCoin;
  [SerializeField] public GameObject coinObject;
  [SerializeField] private TMPro.TextMeshProUGUI _coins;
  public Vector3 spriteCoinPosition => _spriteCoin.gameObject.transform.position;

  private void Awake()
  {
    _initialScaleSpriteRate = _spriteRate.transform.localScale;
    _initialPositionSpriteRate = _spriteRate.transform.position;
    _initialScaleSpriteCoin = _spriteCoin.transform.localScale;
    _initialPositionSpriteCoin = _spriteCoin.transform.position;

    var configCoin = _gameManager.ResourceSystem.GetAllEntity().Find(t => t.typeEntity == TypeEntity.Coin);
    _spriteCoin.sprite = configCoin.sprite;
    _spriteRate.sprite = _gameManager.GameSettings.spriteRate;
    StateManager.OnChangeState += SetValue;
  }
  private void OnDestroy()
  {
    StateManager.OnChangeState -= SetValue;
  }

  public async UniTask AddCoin()
  {
    Vector3 upScale = new Vector3(1.5f, 1.5f, 0);

    float elapsedTime = 0f;
    float duration = .2f;
    float startTime = Time.time;

    while (elapsedTime < duration)
    {
      float progress = (Time.time - startTime) / duration;
      _spriteCoin.transform.localScale = Vector3.Lerp(_initialScaleSpriteCoin, upScale, progress);
      await UniTask.Yield();
      elapsedTime += Time.deltaTime;
    }

    SetDefault();
  }

  public async void SetValue(DataGame data)
  {
    _rate.text = data.rate.ToString();

    _coins.text = data.coins.ToString();

    string status = await Helpers.GetLocaledString(_gameManager.PlayerSetting.text.title);

    string name = string.IsNullOrEmpty(_gameManager.AppInfo.UserInfo.Name)
      ? await Helpers.GetLocaledString(_gameManager.GameSettings.noName.title)
      : _gameManager.AppInfo.UserInfo.Name;

    _statusText.text = string.Format("{0} <size=.2>{1}</size>", name, status);
  }

  private void SetDefault()
  {
    _spriteRate.transform.localScale = _initialScaleSpriteRate;
    _spriteRate.transform.position = _initialPositionSpriteRate;
    _spriteCoin.transform.localScale = _initialScaleSpriteCoin;
    _spriteCoin.transform.position = _initialPositionSpriteCoin;
  }
}
