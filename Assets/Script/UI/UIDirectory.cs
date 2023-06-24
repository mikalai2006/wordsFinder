using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class UIDirectory : UILocaleBase
{
  [SerializeField] private UIDocument _uiDoc;
  [SerializeField] private VisualTreeAsset _directoryItem;
  [SerializeField] private VisualElement _root;
  // private GameObject _enviromnment;
  [SerializeField] private ScrollView _listItems;
  private TaskCompletionSource<DataDialogResult> _processCompletionSource;
  private DataDialogResult _result;
  private string _activeWord;

  public virtual void Start()
  {
    _root = _uiDoc.rootVisualElement;

    _root.Q<VisualElement>("DirectoryBlokWrapper").style.backgroundColor = new StyleColor(_gameSettings.Theme.bgColor);

    var exitBtn = _root.Q<Button>("ExitBtn");
    exitBtn.clickable.clicked += () => ClickClose();

    _listItems = _root.Q<ScrollView>("ListItems");

    FillItems();

    base.Initialize(_root);
  }

  private void Hide()
  {
    _root.style.display = DisplayStyle.None;
  }

  private void FillItems()
  {
    _listItems.Clear();

    var dataGame = _gameManager.StateManager.dataGame;
    List<string> reverse = Enumerable.Reverse(dataGame.activeLevel.openWords).ToList();
    foreach (var item in reverse)
    {
      var blokItem = _directoryItem.Instantiate();
      blokItem.Q<VisualElement>("DirectoryItem").style.backgroundColor = _gameSettings.Theme.bgColor;
      blokItem.Q<Label>("Word").text = item;

      // Button request.
      var buttonGoRead = blokItem.Q<Button>("GoRead");
      // buttonGoRead.Q<VisualElement>("Img").style.backgroundImage = new StyleBackground(_gameSettings.spriteBuy);
      buttonGoRead.clickable.clicked += async () =>
        {
          AudioManager.Instance.Click();

          // TODO Request.
          _activeWord = item;
          await GoRequest(item);

        };

      _listItems.Add(blokItem);
    }
  }

  private async UniTask<string> GoRequest(string word)
  {
    string path = string.Format(
      "{0}{1}{2}",
      _gameSettings.APIDirectory.host,
      _gameSettings.APIDirectory.expression,
      word
    );
    UnityWebRequest webRequest = UnityWebRequest.Get(path);
    webRequest.SetRequestHeader("Authorization", "Basic " + _gameSettings.APIDirectory.token);
    webRequest.SetRequestHeader("Content-Type", "application/json");

    webRequest.SendWebRequest();

    while (!webRequest.isDone)
    {
      await Task.Yield();
    }

    if (webRequest.result == UnityWebRequest.Result.ConnectionError)
    {
      var message = await Helpers.GetLocaledString("disconnect");
      var dialog = new DialogProvider(new DataDialog()
      {
        messageText = message,
        showCancelButton = false
      });
      await dialog.ShowAndHide();

      return webRequest.error;
    }
    else
    {
      var responseText = JsonUtility.FromJson<APIDirectoryResponse>(webRequest.downloadHandler.text);

      if (!string.IsNullOrEmpty(responseText.extract))
      {
        var message = Helpers.StripHTML(responseText.extract);
        var dialog = new DialogProvider(new DataDialog()
        {
          headerText = _activeWord,
          messageText = message,
          showCancelButton = false
        });
        await dialog.ShowAndHide();
      }
      else
      {
        var message = await Helpers.GetLocaledString("nofoundword");
        var dialog = new DialogProvider(new DataDialog()
        {
          messageText = message,
          showCancelButton = false
        });
        await dialog.ShowAndHide();
      }

      return webRequest.downloadHandler.text;
    }
  }

  private void ClickClose()
  {
    AudioManager.Instance.Click();
    _result.isOk = false;
    _processCompletionSource.SetResult(_result);
  }

  public async UniTask<DataDialogResult> ProcessAction()
  {

    _processCompletionSource = new TaskCompletionSource<DataDialogResult>();

    return await _processCompletionSource.Task;
  }
}

[System.Serializable]
public struct APIDirectoryResponse
{
  public string extract;
  public string extract_html;
  public int pageid;
}
