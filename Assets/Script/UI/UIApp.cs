using UnityEngine;
using UnityEngine.UIElements;

public class UIApp : UILocaleBase
{
  [SerializeField] private UIDocument _uiMenuApp;
  [SerializeField] private UIDocument _uiSettings;
  private GameSetting GameSetting;
  [SerializeField] private AudioManager _audioManager => GameManager.Instance.audioManager;
  private GameObject _enviromnment;

  public virtual void Start()
  {

    // base.Localize(_uiDoc.rootVisualElement);
  }

  public void Init(GameObject environment)
  {
    _enviromnment = environment;
  }
}
