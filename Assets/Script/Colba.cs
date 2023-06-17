using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Colba : MonoBehaviour
{
  [SerializeField] private Vector3 _scaleSprite;
  [SerializeField] private Vector3 _position;
  [SerializeField] private SpriteRenderer _sprite;
  [SerializeField] private TMPro.TextMeshProUGUI _countChars;
  [SerializeField] private TMPro.TextMeshProUGUI _countWords;
  private LevelManager _levelManager => GameManager.Instance.LevelManager;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;

  private void Awake()
  {
    _scaleSprite = _sprite.transform.localScale;
    _position = _sprite.transform.position;
    StateManager.OnChangeState += SetValue;
  }
  private void Destroy()
  {
    StateManager.OnChangeState -= SetValue;
  }

  public void RunOpenEffect()
  {
    var _CachedSystem = GameObject.Instantiate(
      _gameSetting.prefabParticleSystem,
      transform.position,
      Quaternion.identity
    );

    var main = _CachedSystem.main;
    main.startSize = new ParticleSystem.MinMaxCurve(0.05f, _levelManager.ManagerHiddenWords.scaleGrid / 2);

    var col = _CachedSystem.colorOverLifetime;
    col.enabled = true;

    Gradient grad = new Gradient();
    grad.SetKeys(new GradientColorKey[] {
      new GradientColorKey(_gameSetting.Theme.bgFindAllowWord, 1.0f),
      new GradientColorKey(_gameSetting.Theme.bgHiddenWord, 0.0f)
      }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f)
    });

    col.color = grad;
    _CachedSystem.Play();
    Destroy(_CachedSystem.gameObject, 2f);
  }


  public async UniTask AddChar()
  {
    Vector3 initialScale = _scaleSprite;
    Vector3 initialPosition = _position;
    Vector3 upScale = new Vector3(1.5f, 1.5f, 0);

    float elapsedTime = 0f;
    float duration = .2f;
    float startTime = Time.time;

    AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.addToColba);
    while (elapsedTime < duration)
    {
      float progress = (Time.time - startTime) / duration;
      _sprite.transform.localScale = Vector3.Lerp(initialScale, upScale, progress);
      await UniTask.Yield();
      elapsedTime += Time.deltaTime;
    }
    RunOpenEffect();
    // var value = Convert.ToInt32(_text.text);
    // value++;
    // SetValue(value.ToString());
    SetDefault();
  }

  public async void SetValue(DataGame data)
  {
    var dataPlural = new Dictionary<string, int> {
      {"count",  data.activeLevel.openWords.Count},
      {"count2", data.activeLevel.countWords},
    };
    var arguments = new[] { dataPlural };
    var textCountWords = await Helpers.GetLocalizedPluralString(
        new UnityEngine.Localization.LocalizedString(Constants.LanguageTable.LANG_TABLE_LOCALIZE, "countword"),
        arguments,
        dataPlural
        );

    _countWords.text = textCountWords;
    _countChars.text = data.activeLevel.countOpenChars.ToString();
  }

  private void SetDefault()
  {
    _sprite.transform.localScale = _scaleSprite;
    _sprite.transform.position = _position;
  }
}
