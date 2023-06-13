using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shuffle : MonoBehaviour, IPointerDownHandler
{
  public static event Action<string> OnShuffleWord;
  [SerializeField] private Transform _transform;
  [SerializeField] private Vector3 _scale;
  [SerializeField] private Vector3 _position;
  [SerializeField] private SpriteRenderer _sprite;
  private LevelManager _levelManager => GameManager.Instance.LevelManager;

  private void Awake()
  {
    _transform = gameObject.transform;
    _scale = _transform.localScale;
    _position = _transform.position;
  }

  // public async UniTask SetPosition(Vector3 newPos)
  // {
  //   Vector3 initialPosition = gameObj.transform.position;

  //   float elapsedTime = 0f;
  //   float duration = .2f;
  //   float startTime = Time.time;

  //   while (elapsedTime < duration)
  //   {
  //     float progress = (Time.time - startTime) / duration;
  //     gameObj.transform.position = Vector3.Lerp(initialPosition, newPos, progress);
  //     await UniTask.Yield();
  //     elapsedTime += Time.deltaTime;
  //   }
  // }

  private void SetDefault()
  {
    transform.localScale = _scale;
    transform.position = _position;
  }

  public async void OnPointerDown(PointerEventData eventData)
  {
    var existChars = _levelManager.Symbols;

    // get all positions.
    Dictionary<SymbolMB, char> existPositionsChars = new();
    for (int i = 0; i < existChars.Count; i++)
    {
      existPositionsChars.Add(existChars[i], existChars[i].charTextValue);
    }
    // shuffle positions.
    existChars = existChars.OrderBy(t => UnityEngine.Random.value).ToList();

    // set new position.
    List<UniTask> tasks = new();
    string newWord = "";
    for (int i = 0; i < existChars.Count; i++)
    {
      tasks.Add(existChars[i].SetPosition(existPositionsChars.ElementAt(i).Key.transform.position));
      newWord += existChars.ElementAt(i).charTextValue;
    }
    OnShuffleWord?.Invoke(newWord);

    await UniTask.WhenAll(tasks);
    GameManager.Instance.DataManager.Save();
  }
}
