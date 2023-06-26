using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;


public abstract class UILocaleBase : MonoBehaviour
{
  [SerializeField] public LocalizedStringTable _localization;
  private VisualElement _box;

  private List<LocalizeObj> _elementList;
  protected GameManager _gameManager => GameManager.Instance;
  protected GameSetting _gameSettings => GameManager.Instance.GameSettings;

  public void Initialize(VisualElement root)
  {
    _box = root;
    Localize();
    Theming(_box);
  }

  public void Theming(VisualElement root)
  {
    _box = root;

    UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(_box);
    List<VisualElement> list = builder.Class("text-primary").ToList();
    foreach (var item in list)
    {
      item.style.color = _gameManager.Theme.colorPrimary;
    }

    UQueryBuilder<VisualElement> builderSecondary = new UQueryBuilder<VisualElement>(_box);
    List<VisualElement> listSecondary = builderSecondary.Class("text-secondary").ToList();
    foreach (var item in listSecondary)
    {
      item.style.color = _gameManager.Theme.colorSecondary;
    }

  }

  private async void Localize()
  {

    // await LocalizationSettings.InitializationOperation.Task;

    var op = _localization.GetTableAsync();
    await op.Task;
    _elementList = HelperUI.FindAllTextElements(_box);
    OnTableLoaded(op);
  }
  private void OnTableLoaded(AsyncOperationHandle<StringTable> op)
  {
    StringTable table = op.Result;

    foreach (var item in _elementList)
    {
      var entry = op.Result[item.Key];
      if (entry != null)
        item.Element.text = entry.LocalizedValue;
      else
        Debug.LogWarning($"No {op.Result.LocaleIdentifier.Code} translation for key: '{item.Key}'");
    }
  }
}

