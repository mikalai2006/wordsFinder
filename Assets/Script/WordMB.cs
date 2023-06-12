using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WordMB : MonoBehaviour
{
  [SerializeField] private CharMB charMB;
  private LevelManager _dataManager = LevelManager.Instance;
  private List<CharMB> _chars;
  private List<CharMB> _charsGameObject;
  //   private InputManager _inputManager;
  //   private Camera _camera;
  //   [SerializeField] private Collider2D _collider;

  private void Awake()
  {
    _chars = new List<CharMB>();
    for (int i = 0; i < 20; i++)
    {
      var newChar = GameObject.Instantiate(
        charMB,
        new Vector3(i, 0, 0),
        Quaternion.identity,
        gameObject.transform
      );
      //newChar.gameObject.SetActive(false);
      _chars.Add(newChar);
    }
    _charsGameObject = new();
  }

  public void CreateWord()
  {

  }

  public void DrawWord(string choosedWord)
  {
    ResetWord();
    gameObject.transform.localPosition = new Vector3(-choosedWord.Length / 2f + .5f, 0, 0);
    for (int i = 0; i < choosedWord.Length; i++)
    {
      var currentChar = choosedWord.ElementAt(i);
      var currentCharMB = _chars.ElementAt(i);
      _charsGameObject.Add(currentCharMB);
      currentCharMB.gameObject.SetActive(true);
      currentCharMB.SetValue(currentChar);
    }
  }

  public async UniTask YesWord(HiddenWordMB hiddenWordMB)
  {
    List<UniTask> listTasks = new();
    for (int i = 0; i < _charsGameObject.Count; i++)
    {
      var currentCharMB = _charsGameObject.ElementAt(i);

      var needHiddenChar = hiddenWordMB.Chars.Find(t => t.charTextValue == currentCharMB.charTextValue);

      if (needHiddenChar == null) continue;

      listTasks.Add(currentCharMB.CheckYes(needHiddenChar));
    }
    AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.yesWord);
    await UniTask.WhenAll(listTasks);
  }
  public async UniTask YesPotentialWord(Colba colba)
  {
    List<UniTask> listTasks = new();
    for (int i = 0; i < _charsGameObject.Count; i++)
    {
      var currentCharMB = _charsGameObject.ElementAt(i);

      listTasks.Add(currentCharMB.CheckPotentialYes(colba.gameObject, i * (50 + i * 10)));
    }
    AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.yesWord);
    await UniTask.WhenAll(listTasks);
  }

  public async UniTask NoWord()
  {
    await NotFoundWord();

    List<UniTask> listTasks = new();

    for (int i = 0; i < _charsGameObject.Count; i++)
    {
      var currentCharMB = _charsGameObject.ElementAt(i);

      // await currentCharMB.CheckNo();
      listTasks.Add(currentCharMB.CheckNo());
    }
    AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.noWord);
    await UniTask.WhenAll(listTasks);
  }

  private void ResetWord()
  {
    foreach (var ch in _charsGameObject)
    {
      ch.gameObject.SetActive(false);
    }
    _charsGameObject.Clear();
  }

  public async UniTask NotFoundWord()
  {
    Vector3 initialPosition = transform.localPosition;
    Vector3 upPosition = initialPosition - new Vector3(-.5f, 0, 0);
    var duration = .1f;

    for (float time = 0; time < duration * 2; time += Time.deltaTime)
    {
      float progress = Mathf.PingPong(time, duration) / duration;
      transform.localPosition = Vector3.Lerp(initialPosition, upPosition, progress);
      await UniTask.Yield();// yield return null;
    }
    transform.localPosition = initialPosition;
  }

  public async UniTask ExistPotentialWord(HiddenWordMB hiddenWordMB)
  {
    List<UniTask> listTasks = new();
    for (int i = 0; i < _charsGameObject.Count; i++)
    {
      var currentCharMB = _charsGameObject.ElementAt(i);

      var needHiddenChar = hiddenWordMB.Chars.Find(t => t.charTextValue == currentCharMB.charTextValue);

      if (needHiddenChar == null) continue;

      listTasks.Add(currentCharMB.CheckExist(needHiddenChar));
    }
    AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.yesWord);
    await UniTask.WhenAll(listTasks);
  }
}
