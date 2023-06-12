using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Colba : MonoBehaviour
{
  [SerializeField] private Transform _transform;
  [SerializeField] private Vector3 _scale;
  [SerializeField] private Vector3 _position;

  private void Awake()
  {
    _transform = gameObject.transform;
    _scale = _transform.localScale;
    _position = _transform.position;
  }

  public async UniTask AddChar()
  {
    transform.localScale += new Vector3(0.1f, 0.1f, 0);

    await UniTask.Delay(100);
    SetDefault();
  }

  private void SetDefault()
  {
    transform.localScale = _scale;
    transform.position = _position;
  }
}
