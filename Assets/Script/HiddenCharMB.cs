using System.Collections;
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

  public void Awake()
  {
    _gameSetting = GameManager.Instance.GameSetting;
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
    _image.color = _gameSetting.colorWordSymbol;
    _image.sprite = _gameSetting.bgChar;
    _charText.gameObject.SetActive(false);
  }
  public void Open()
  {
    _image.color = _gameSetting.colorWordSymbolYes;
    _image.sprite = _gameSetting.bgChar;
    _charText.gameObject.SetActive(true);
  }
  public IEnumerator ShowChar()
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
    yield return new WaitForSeconds(0.01f);
  }

}
