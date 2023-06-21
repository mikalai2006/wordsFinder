using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TopSide : MonoBehaviour
{
  private Vector3 _scaleSpriteCoin;
  private Vector3 _positionSpriteCoin;
  private Vector3 _scaleSpriteRate;
  private Vector3 _positionSpriteRate;
  [SerializeField] private Image _spriteRate;
  [SerializeField] private TMPro.TextMeshProUGUI _rate;
  [SerializeField] private Image _spriteCoin;
  [SerializeField] private TMPro.TextMeshProUGUI _coins;
  public Vector3 spriteCoinPosition => _spriteCoin.gameObject.transform.position;
  private LevelManager _levelManager => GameManager.Instance.LevelManager;
  private GameManager _gameManager => GameManager.Instance;

  private void Awake()
  {
    _scaleSpriteRate = _spriteRate.transform.localScale;
    _positionSpriteRate = _spriteRate.transform.position;
    _scaleSpriteCoin = _spriteCoin.transform.localScale;
    _positionSpriteCoin = _spriteCoin.transform.position;

    _spriteCoin.sprite = _gameManager.GameSettings.spriteCoin;
    _spriteRate.sprite = _gameManager.GameSettings.spriteRate;
    StateManager.OnChangeState += SetValue;
  }
  private void OnDestroy()
  {
    StateManager.OnChangeState -= SetValue;
  }

  public async UniTask AddCoin()
  {
    Vector3 initialScale = _scaleSpriteCoin;
    // Vector3 initialPosition = _spriteCoin.transform.localPosition;
    Vector3 upScale = new Vector3(1.5f, 1.5f, 0);

    float elapsedTime = 0f;
    float duration = .2f;
    float startTime = Time.time;

    AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.addCoin);
    while (elapsedTime < duration)
    {
      float progress = (Time.time - startTime) / duration;
      _spriteCoin.transform.localScale = Vector3.Lerp(initialScale, upScale, progress);
      await UniTask.Yield();
      elapsedTime += Time.deltaTime;
    }
    // _spriteCoin.transform.localScale = initialScale;
    SetDefault();
  }

  public void SetValue(DataGame data, StatePerk statePerk)
  {
    // Debug.Log($"TopSide setValue {data.rate}");
    _rate.text = data.rate.ToString();
    _coins.text = data.coins.ToString();
  }

  private void SetDefault()
  {
    _spriteRate.transform.localScale = _scaleSpriteRate;
    _spriteRate.transform.position = _positionSpriteRate;
    _spriteCoin.transform.localScale = _scaleSpriteCoin;
    _spriteCoin.transform.position = _positionSpriteCoin;
  }
}
