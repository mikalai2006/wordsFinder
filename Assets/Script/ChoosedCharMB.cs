using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ChoosedCharMB : MonoBehaviour
{
  [SerializeField] private TMPro.TextMeshProUGUI _charText;
  [SerializeField] public RectTransform RectTransform;

  [HideInInspector] public char charTextValue;
  private LevelManager _levelManager => GameManager.Instance.LevelManager;
  [SerializeField] private Image _image;
  private GameSetting _gameSetting;
  private Vector3 _initPosition;

  public void Start()
  {
    _gameSetting = GameManager.Instance.GameSettings;
    _initPosition = gameObject.transform.position;
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
    RectTransform.sizeDelta = new Vector2(size, size);
    // Debug.Log($"change position ={(1 - size) * index}::: {index}");
    // transform.position -= new Vector3(1 - size, 0);
    // gameObject.transform.localPosition -= new Vector3(1 - size, 0);
  }

  public async UniTask CheckYes(HiddenCharMB needHiddenChar, int delay)
  {
    _image.color = _gameSetting.bgFindHiddenWord;
    _charText.color = _gameSetting.textFindHiddenWord;
    await UniTask.Delay(delay);

    Vector3 initialScale = transform.localScale;
    Vector3 initialPosition = transform.position;
    Vector3 upScale = new Vector3(0.5f, 0.5f, 0);

    float elapsedTime = 0f;
    float duration = .5f;
    float startTime = Time.time;

    while (elapsedTime < duration)
    {
      // The center of the arc
      Vector3 center = (initialPosition + needHiddenChar.gameObject.transform.position) * 0.5F;

      // move the center a bit downwards to make the arc vertical
      center -= new Vector3(0, 1, 0);
      // Interpolate over the arc relative to center
      Vector3 riseRelCenter = initialPosition - center;
      Vector3 setRelCenter = needHiddenChar.gameObject.transform.position - center;

      float progress = (Time.time - startTime) / duration; //elapsedTime / duration;
      transform.localScale = Vector3.Lerp(initialScale, upScale, progress);
      transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, progress);
      transform.position += center;
      await UniTask.Yield();
      elapsedTime += Time.deltaTime;
    }
    transform.localScale = initialScale;
    needHiddenChar.ShowChar().Forget();

    AudioManager.Instance.PlayClipEffect(GameManager.Instance.GameSettings.Audio.openWord);
    // Vector3 initialScale = transform.localScale;
    // Vector3 upScale = new Vector3(0.4f, 0.4f, 0f);
    // var duration = .5f;
    // float elapsedTime = 0f;

    // while (elapsedTime < duration)
    // {
    //   // float progress = Mathf.PingPong(time, duration) / duration;
    //   float progress = elapsedTime / duration;
    //   transform.localScale = Vector3.Lerp(initialScale, upScale, progress);
    //   transform.position = Vector3.Lerp(transform.position, needHiddenChar.transform.position, progress);
    //   elapsedTime += Time.deltaTime;
    //   await UniTask.Yield();
    // }
    // transform.localScale = initialScale;

    SetDefault();
  }

  public async UniTask OpenCharAllowWord(Colba colba, int delay)
  {
    _image.color = _gameSetting.bgFindAllowWord;
    _charText.color = _gameSetting.textFindAllowWord;
    await UniTask.Delay(delay);

    Vector3 initialScale = transform.localScale;
    Vector3 initialPosition = transform.position;
    Vector3 upScale = new Vector3(0.5f, 0.5f, 0);

    float elapsedTime = 0f;
    float duration = .5f;
    float startTime = Time.time;

    while (elapsedTime < duration)
    {
      // The center of the arc
      Vector3 center = (initialPosition + colba.gameObject.transform.position) * 0.5F;

      // move the center a bit downwards to make the arc vertical
      center -= new Vector3(0, 1, 0);
      // Interpolate over the arc relative to center
      Vector3 riseRelCenter = initialPosition - center;
      Vector3 setRelCenter = colba.gameObject.transform.position - center;

      float progress = (Time.time - startTime) / duration; //elapsedTime / duration;
      transform.localScale = Vector3.Lerp(initialScale, upScale, progress);
      transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, progress);
      transform.position += center;
      await UniTask.Yield();
      elapsedTime += Time.deltaTime;
    }
    transform.localScale = initialScale;
    // SetDefault();
    // gameObject.SetActive(false);

    await colba.AddChar();
  }

  public async UniTask CheckNo()
  {
    _image.color = _gameSetting.bgNotFoundWord;

    await UniTask.Delay(300); // yield return new WaitForSeconds(.3f);

    SetDefault();
  }

  public void SetDefault()
  {
    gameObject.SetActive(false);
    _image.color = _gameSetting.bgChoosedWord;
    _charText.color = _gameSetting.textChoosedWord;
    transform.position = _initPosition;
  }

  public async UniTask CheckExist()
  {
    Vector3 initialScale = transform.localScale;
    Vector3 upScale = new Vector3(1.5f, 1.5f, 0f);
    var duration = .5f;
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
      // float progress = Mathf.PingPong(time, duration) / duration;
      float progress = elapsedTime / duration;
      transform.localScale = Vector3.Slerp(initialScale, upScale, progress);
      elapsedTime += Time.deltaTime;
      await UniTask.Yield();
    }
    transform.localScale = initialScale;

    SetDefault();
  }
}
