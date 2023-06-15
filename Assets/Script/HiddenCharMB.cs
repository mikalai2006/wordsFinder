using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HiddenCharMB : MonoBehaviour
{
  [SerializeField] private TMPro.TextMeshProUGUI _charText;
  public char charTextValue;
  private LevelManager _dataManager = LevelManager.Instance;
  [SerializeField] private Image _image;
  [SerializeField] private RectTransform _canvas;
  private GameSetting _gameSetting;
  public float tickPerSecond;
  public GridNode OccupiedNode;

  public void Awake()
  {
    _gameSetting = GameManager.Instance.GameSettings;
    SetDefault();
  }

  public void SetValue(char currentChar)
  {
    _charText.text = currentChar.ToString();
    charTextValue = currentChar;
  }
  public void SetSize(float size)
  {
    _charText.fontSize = size;
    _canvas.sizeDelta = new Vector2(size, size);
  }
  private void SetDefault()
  {
    _image.color = _gameSetting.bgHiddenWord;
    _image.sprite = _gameSetting.bgImageHiddenWord;
    _charText.gameObject.SetActive(false);
  }
  public void Open()
  {
    _image.color = _gameSetting.bgOpentHiddenWord;
    _image.sprite = _gameSetting.bgImageHiddenWord;
    _charText.color = _gameSetting.textOpentHiddenWord;
    _charText.gameObject.SetActive(true);
  }
  public async UniTask ShowChar()
  {
    Vector3 initialScale = transform.localScale;

    // for (float i = 0f; i < 180f; i += 10f)
    // {
    //   transform.rotation = Quaternion.Euler(0f, i, 0f);
    //   if (i == 90f)
    //   {
    //     _image.color = _gameSetting.colorWordSymbolYes;
    //   }
    //   yield return new WaitForSeconds(0.01f);
    // }
    // gameObject.transform.localScale = new Vector3(-1, 1, 1);

    Open();
    await UniTask.Yield();
  }

  public async UniTask FocusOpenChar()
  {
    var startScale = transform.localScale;

    Vector3 initialScale = transform.localScale;
    Vector3 upScale = new Vector3(1.2f, 1.2f, 0f);
    var duration = .5f;
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
      float progress = elapsedTime / duration;
      transform.localScale = Vector3.Lerp(initialScale, upScale, progress);
      elapsedTime += Time.deltaTime;
      await UniTask.Yield();
    }
    transform.localScale = initialScale;


  }
}
