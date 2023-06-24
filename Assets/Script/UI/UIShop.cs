using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Loader;
using UnityEngine;
using UnityEngine.UIElements;

public class UIShop : UILocaleBase
{
  [SerializeField] private UIDocument _uiDoc;
  [SerializeField] private VisualTreeAsset _shopItem;
  [SerializeField] private VisualElement _root;
  private GameObject _enviromnment;
  [SerializeField] private ScrollView _listItems;
  private TaskCompletionSource<DataDialogResult> _processCompletionSource;
  private DataDialogResult _result;

  private void Awake()
  {

    LevelManager.OnInitLevel += Hide;
  }

  private void OnDestroy()
  {

    LevelManager.OnInitLevel -= Hide;
  }

  public virtual void Start()
  {
    _root = _uiDoc.rootVisualElement;
    _root.Q<VisualElement>("ShopBlokWrapper").style.backgroundColor = new StyleColor(_gameSettings.Theme.bgColor);

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

  private async void FillItems()
  {
    _listItems.Clear();

    foreach (var item in _gameSettings.ShopItems)
    {
      var blokItem = _shopItem.Instantiate();
      blokItem.Q<VisualElement>("ShopItem").style.backgroundColor = _gameSettings.Theme.bgColor;
      var title = await Helpers.GetLocaledString(item.text.title);
      blokItem.Q<Label>("Name").text = title;

      var textCost = await Helpers.GetLocalizedPluralString(
          "coin",
           new Dictionary<string, object> {
            {"count",  item.cost},
          }
        );
      blokItem.Q<Label>("Price").text = string.Format("{0} <size=12>{1}</size>", item.cost, textCost);

      // var textCountWords = await Helpers.GetLocalizedPluralString(
      //     "costitemtext",
      //      new Dictionary<string, object> {
      //       {"count",  item.count},
      //       {"cost", item.cost},
      //       {"name", title},
      //     }
      //   );
      var description = await Helpers.GetLocaledString(item.text.description);
      blokItem.Q<Label>("Description").text = string.Format(
        "{0}",
        // textCountWords,
        description
        );

      blokItem.Q<VisualElement>("Img").style.backgroundImage = new StyleBackground(item.entity.sprite);

      // Button buy for coin.
      var buttonForCoin = blokItem.Q<Button>("Buy");
      buttonForCoin.Q<VisualElement>("Img").style.backgroundImage = new StyleBackground(_gameSettings.spriteBuy);
      buttonForCoin.clickable.clicked += () =>
        {
          // TODO Buy for coin.
          AudioManager.Instance.Click();
          _gameManager.StateManager.BuyHint(item);

        };
      var textButtonForCoin = buttonForCoin.Q<Label>("Text");
      if (_gameManager.StateManager.dataGame.coins < item.cost)
      {
        buttonForCoin.SetEnabled(false);
        textButtonForCoin.text = await Helpers.GetLocaledString("lackscoin");
      }
      else
      {
        buttonForCoin.SetEnabled(true);
        textButtonForCoin.text = await Helpers.GetLocalizedPluralString(
          "buyforcoin",
           new Dictionary<string, object> {
            {"cost",  item.cost},
          }
        );
      }

      // Button buy for coin.
      var buttonForAdv = blokItem.Q<Button>("Adv");
      buttonForAdv.Q<VisualElement>("Img").style.backgroundImage = new StyleBackground(_gameSettings.spriteAdv);
      buttonForAdv.clickable.clicked += () =>
        {
          // TODO Buy for adv.
          AudioManager.Instance.Click();
        };

      _listItems.Add(blokItem);
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
