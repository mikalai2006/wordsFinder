using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
// using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class SymbolMB : MonoBehaviour, IPointerEnterHandler
{
  [SerializeField] private TMPro.TextMeshProUGUI _charText;
  private InputManager _inputManager;
  private Camera _camera;
  public char charTextValue;
  private LevelManager _levelManager = LevelManager.Instance;
  [SerializeField] private Collider2D _collider;
  [SerializeField] private Image _image;
  [SerializeField] private RectTransform _canvas;

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
    _image.color = GameManager.Instance.GameSetting.colorSymbol;
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

      if (rayHit.collider.gameObject == gameObject) // _model.gameObject
      {
        // if (context.interaction is PressInteraction || context.interaction is TapInteraction)
        // {
        var control = context.control.ToString().Split('/');
        // Debug.Log($"Press::: {gameObject.name}| {control[1]}");
        _inputManager.SetDragging(true);
        if (control[1] == "Mouse")
        {
          ChooseSymbol();
        }
        // }
      }
    }
    else if (context.canceled && _inputManager.Dragging)
    {
      // Debug.Log($"Disable dragging::: {gameObject.name}");
      _inputManager.SetDragging(false);
      await _levelManager.ManagerHiddenWords.CheckChoosedWord();
    }
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (_inputManager.Dragging)
    {
      // Debug.Log($"OnPointerEnter::: {gameObject.name}");
      // _collider.gameObject.SetActive(false);
      ChooseSymbol();
    }
  }

  private void ChooseSymbol()
  {
    _levelManager.ManagerHiddenWords.AddChoosedChar(this);
    _image.color = GameManager.Instance.GameSetting.colorChooseSymbol;
  }

  public void ResetObject()
  {
    // _collider.gameObject.SetActive(true);
    _image.color = GameManager.Instance.GameSetting.colorSymbol;
  }
}
