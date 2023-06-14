using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HiddenWordMB : MonoBehaviour
{
  [SerializeField] private HiddenCharMB charMB;
  private DataManager _dataManager = DataManager.Instance;
  public List<HiddenCharMB> Chars;
  public string _word;
  private ManagerHiddenWords _managerHiddenWords;
  //   private InputManager _inputManager;
  //   private Camera _camera;
  //   [SerializeField] private Collider2D _collider;

  private void Awake()
  {
    Chars = new List<HiddenCharMB>();
  }

  public void DrawWord(string word)
  {
    var _size = 0.6f;
    gameObject.transform.localPosition = new Vector3(-3, _managerHiddenWords.hiddenWords.Count * _size, 0);

    _word = word;
    ResetWord();
    for (int i = 0; i < word.Length; i++)
    {
      var newChar = GameObject.Instantiate(
        charMB,
        Vector3.zero,
        Quaternion.identity,
        gameObject.transform
      );
      newChar.transform.localPosition = new Vector3(i * _size, 0, 0);
      var currentChar = word.ElementAt(i);
      // var currentCharMB = Chars.ElementAt(i);
      newChar.gameObject.SetActive(true);
      newChar.SetValue(currentChar);
      newChar.SetSize(_size);
      Chars.Add(newChar);
    }
  }

  private void ResetWord()
  {
    // foreach (var ch in _chars)
    // {
    //   ch.gameObject.SetActive(false);
    // }
  }
  public void ShowWord()
  {
    foreach (var charObj in Chars)
    {
      charObj.ShowChar().Forget();
    }
  }

  public async UniTask FocusOpenWord()
  {
    List<UniTask> tasks = new();
    foreach (var charObj in Chars)
    {
      tasks.Add(charObj.FocusOpenChar());
    }
    await UniTask.WhenAll(tasks);
  }

  public void Init(ManagerHiddenWords managerHiddenWords)
  {
    _managerHiddenWords = managerHiddenWords;
  }
}
