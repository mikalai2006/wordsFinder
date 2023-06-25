using UnityEngine;
using UnityEngine.EventSystems;

public class CharArea : MonoBehaviour, IPointerMoveHandler
{
  private LevelManager _levelManager => GameManager.Instance.LevelManager;
  private GameSetting _gameSettings => GameManager.Instance.GameSettings;
  private GameManager _gameManager => GameManager.Instance;
  [SerializeField] private BoxCollider2D _collider;
  private Camera _camera;

  private void Awake()
  {
    _camera = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();
  }

  private void DrawLine()
  {
    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(_gameManager.InputManager.clickPosition());
    _gameManager.LineManager.DrawLine(worldPosition);
  }

  public void OnPointerMove(PointerEventData eventData)
  {
    if (_gameManager.InputManager.Dragging)
    {
      DrawLine();
    }
  }
}