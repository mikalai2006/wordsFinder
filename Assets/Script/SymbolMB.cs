using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
// using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class SymbolMB : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  [SerializeField] private TMPro.TextMeshProUGUI _charText;
  private InputManager _inputManager;
  private Camera _camera;
  public char charTextValue;
  private LevelManager _levelManager => GameManager.Instance.LevelManager;
  private GameSetting _gameSettings => GameManager.Instance.GameSettings;
  [SerializeField] private Collider2D _collider;
  [SerializeField] private Image _image;
  [SerializeField] private RectTransform _canvas;
  private float timeOverChar = 0f;
  private CancellationTokenSource cancelTokenSource;

  private void OnEnable()
  {
    _inputManager = new InputManager();
    _inputManager.Click += OnClick;
    _inputManager.Enable();
  }

  private void OnDisable()
  {
    _inputManager.Click -= OnClick;
    _inputManager.Disable();
  }

  private void Awake()
  {
    _camera = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();
    _image.color = _gameSettings.colorSymbol;
  }


  public void Init(char symbol)
  {
    _charText.text = symbol.ToString();
    charTextValue = symbol;
  }

  public void SetSize(float size)
  {
    _charText.fontSize = size;
    _canvas.sizeDelta = new Vector2(size, size);
  }

  public async void OnClick(InputAction.CallbackContext context)
  {
    if (context.performed)
    {
      var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(_inputManager.clickPosition()));
      if (!rayHit.collider) return;

      if (rayHit.collider.gameObject == gameObject)
      {
        var control = context.control.ToString().Split('/');
        _inputManager.SetDragging(true);
        if (control[1] == "Mouse")
        {
          cancelTokenSource = new CancellationTokenSource();
          StartWaitDelay(cancelTokenSource.Token).Forget();
        }
      }
    }
    else if (context.canceled && _inputManager.Dragging)
    {
      _inputManager.SetDragging(false);
      await _levelManager.ManagerHiddenWords.CheckChoosedWord();
    }
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (_inputManager.Dragging)
    {
      cancelTokenSource = new CancellationTokenSource();
      StartWaitDelay(cancelTokenSource.Token).Forget();
    }
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    if (_inputManager.Dragging)
    {
      StopWaitDelay();
    }
  }

  private void StopWaitDelay()
  {
    cancelTokenSource.Cancel();
    cancelTokenSource.Dispose();
  }

  public async UniTask StartWaitDelay(CancellationToken cancellationToken)
  {
    if (_levelManager.ManagerHiddenWords.listChoosedGameObjects.Count != 0)
    {
      await UniTask.Delay(_gameSettings.timeDelayOverChar, cancellationToken: cancellationToken);
    }

    if (!cancellationToken.IsCancellationRequested)
    {
      ChooseSymbol();
    }
  }

  private void ChooseSymbol()
  {
    _levelManager.ManagerHiddenWords.AddChoosedChar(this);
    _image.color = _gameSettings.colorChooseSymbol;
  }

  public void ResetObject()
  {
    _image.color = _gameSettings.colorSymbol;
  }
}
