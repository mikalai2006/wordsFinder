using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Colba : MonoBehaviour
{
  private Vector3 _initScale;
  private Vector3 _initPosition;
  [SerializeField] private SpriteRenderer _sprite;
  [SerializeField] private SpriteRenderer _spriteProgress;
  [SerializeField] private TMPro.TextMeshProUGUI _countChars;
  [SerializeField] private TMPro.TextMeshProUGUI _countWords;
  private LevelManager _levelManager => GameManager.Instance.LevelManager;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  private GameManager _gameManager => GameManager.Instance;
  private float progressBasePositionY = -1.4f;

  private void Awake()
  {
    _initScale = transform.localScale;
    _initPosition = transform.position;
    _sprite.sprite = _gameSetting.spriteStar;
    StateManager.OnChangeState += SetValue;
  }

  private void Destroy()
  {
    StateManager.OnChangeState -= SetValue;
  }


  public void RunOpenEffect()
  {
    var _CachedSystem = GameObject.Instantiate(
      _gameSetting.Boom,
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
    if (_CachedSystem.isPlaying || _CachedSystem.isStopped) Destroy(_CachedSystem.gameObject, 2f);
  }


  public void CreateStar()
  {
    var potentialGroup = _levelManager.ManagerHiddenWords.GridHelper
      .GetGroupNodeChars()
      // .OrderBy(t => UnityEngine.Random.value)
      .FirstOrDefault();
    if (potentialGroup.Value != null && potentialGroup.Value.Count > 0)
    {
      var node = potentialGroup.Value.First();
      if (node != null)
      {
        var starEntity = _levelManager.ManagerHiddenWords.AddEntity(node.arrKey, TypeEntity.Star);
        if (gameObject != null)
        {
          // node.StateNode |= StateNode.Entity;
          _gameManager.StateManager.statePerk.needCreateStar -= 1;
          starEntity.SetPosition(_levelManager.ManagerHiddenWords.tilemap.WorldToCell(gameObject.transform.position));
        }
        else
        {
          Debug.LogWarning($"Not found {name}");
        }
      }
    }
  }


  public async UniTask AddChar()
  {
    Vector3 initialScale = _initScale;
    Vector3 initialPosition = _initPosition;
    Vector3 upScale = new Vector3(1.5f, 1.5f, 0);

    float elapsedTime = 0f;
    float duration = .2f;
    float startTime = Time.time;

    AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.addToColba);
    while (elapsedTime < duration)
    {
      float progress = (Time.time - startTime) / duration;
      transform.localScale = Vector3.Lerp(initialScale, upScale, progress);
      await UniTask.Yield();
      elapsedTime += Time.deltaTime;
    }
    RunOpenEffect();
    // var value = Convert.ToInt32(_text.text);
    // value++;
    // SetValue(value.ToString());
    SetDefault();
    // ChangeValue();
  }

  // private void ChangeValue()
  // {
  //   var _stateManager = _gameManager.StateManager;
  //   _stateManager.statePerk.countCharInOrder += 1;
  //   // _stateManager.statePerk.countWordInOrder += 1;
  //   _stateManager.statePerk.countCharForBonus += 1;
  //   _stateManager.statePerk.countCharForAddHint += 1;
  //   _stateManager.statePerk.countCharForAddCoin += 1;

  //   _stateManager.statePerk.countErrorForNullBonus = 0;

  //   // Add bonus index.
  //   if (_stateManager.statePerk.countCharForBonus >= _gameManager.GameSettings.PlayerSetting.countCharForBonus)
  //   {
  //     _stateManager.statePerk.countCharForBonus -= _gameManager.GameSettings.PlayerSetting.countCharForBonus;
  //     _stateManager.dataGame.activeLevel.index++;
  //   }

  //   // Add hint.
  //   if (_stateManager.statePerk.countCharForAddHint >= _gameManager.GameSettings.PlayerSetting.countCharForAddHint)
  //   {
  //     _stateManager.statePerk.countCharForAddHint -= _gameManager.GameSettings.PlayerSetting.countCharForAddHint;
  //     _stateManager.dataGame.activeLevel.hint++;
  //   }

  //   // Check add coin to grid.
  //   if (_stateManager.statePerk.countCharForAddCoin >= _gameManager.GameSettings.PlayerSetting.countCharForCoin)
  //   {
  //     _stateManager.statePerk.countCharForAddCoin -= _gameManager.GameSettings.PlayerSetting.countCharForCoin;
  //     CreateStar();
  //   }

  //   _stateManager.RefreshData();
  // }

  public async void SetValue(DataGame data, StatePerk statePerk)
  {
    // View new data.
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

    if (statePerk.needCreateStar > 0)
    {
      for (int i = 0; i < statePerk.needCreateStar; i++)
      {
        CreateStar();
      }
    }

    SetValueProgressBar(data, statePerk);
  }


  private void SetValueProgressBar(DataGame data, StatePerk statePerk)
  {
    var newPosition = (progressBasePositionY + 1.2f) + progressBasePositionY - progressBasePositionY * (float)statePerk.countCharForAddStar / _gameSetting.PlayerSetting.bonusCount.charStar;
    _spriteProgress.transform.localPosition
      = new Vector3(_spriteProgress.transform.localPosition.x, newPosition);
  }


  private void SetDefault()
  {
    transform.localScale = _initScale;
    transform.position = _initPosition;
  }
}
