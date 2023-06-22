using System.Linq;
using Cysharp.Threading.Tasks;

public class ButtonLighting : BaseButton
{
  #region UnityMethods
  protected override void Awake()
  {
    base.Awake();

    spriteBg.sprite = _gameSetting.spriteLighting;
    spriteMask.sprite = _gameSetting.spriteLighting;

    StateManager.OnChangeState += SetValue;
  }
  protected override void OnDestroy()
  {
    base.OnDestroy();

    StateManager.OnChangeState -= SetValue;
  }
  #endregion

  public override void SetValue(DataGame data, StatePerk statePerk)
  {
    value = data.lighting;
    base.SetValue(data, statePerk);
  }

  public override void RunHint()
  {
    var nodes = _levelManager.ManagerHiddenWords.GridHelper.GetGroupNodeChars();
    if (nodes.Count <= 0)
    {
      // TODO Show dialog - no hidden char
      return;
    };

    var nodesForShow = nodes.OrderBy(t => -t.Value.Count).First().Value;

    int countRunHit = 0;
    foreach (var node in nodesForShow)
    {
      if (node != null)
      {
        node.OccupiedChar.ShowCharAsNei(true).Forget();
        _levelManager.ManagerHiddenWords.AddOpenChar(node.OccupiedChar);
        node.SetHint();
        countRunHit++;
      }
    }
    if (countRunHit > 0) _stateManager.UseBomb();
  }
}
