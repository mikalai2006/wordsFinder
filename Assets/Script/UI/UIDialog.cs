using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class UIDialog : UILocaleBase
{
  [SerializeField] private UIDocument _uiDoc;
  private VisualElement _wrap;
  private Label _headerText;
  private Label _messageText;
  private Button _buttonOk;
  private Button _buttonCancel;
  private TaskCompletionSource<DataDialogResult> _processCompletionSource;
  public UnityEvent processAction;
  private DataDialog _dataDialog;
  private DataDialogResult _dataResultDialog;
  private VisualElement _root;

  private void Start()
  {
    _root = _uiDoc.rootVisualElement;

    _headerText = _root.Q<Label>("HeaderText");
    _messageText = _root.Q<Label>("MessageText");

    _root.Q<VisualElement>("DialogWrapper").style.backgroundColor = new StyleColor(_gameSettings.Theme.bgColor);

    _buttonOk = _root.Q<Button>("Ok");
    _buttonOk.clickable.clicked += OnClickOk;

    _buttonCancel = _root.Q<Button>("Cancel");
    _buttonCancel.style.display = DisplayStyle.None;
    _buttonCancel.clickable.clicked += OnClickCancel;

    base.Initialize(_root);
  }

  public async Task<DataDialogResult> ProcessAction(DataDialog dataDialog)
  {
    _dataResultDialog = new DataDialogResult();
    _dataDialog = dataDialog;

    if (!string.IsNullOrEmpty(_dataDialog.headerText)) _headerText.text = _dataDialog.headerText;
    if (!string.IsNullOrEmpty(_dataDialog.messageText)) _messageText.text = _dataDialog.messageText;

    if (_dataDialog.showCancelButton)
    {
      _buttonCancel.style.display = DisplayStyle.Flex;
    }

    _processCompletionSource = new TaskCompletionSource<DataDialogResult>();

    return await _processCompletionSource.Task;
  }

  private void OnClickOk()
  {
    AudioManager.Instance.Click();
    _dataResultDialog.isOk = true;
    _processCompletionSource.SetResult(_dataResultDialog);

    processAction?.Invoke();
  }

  private void OnClickCancel()
  {
    AudioManager.Instance.Click();
    _dataResultDialog.isOk = false;
    _processCompletionSource.SetResult(_dataResultDialog);

    processAction?.Invoke();
  }
}

