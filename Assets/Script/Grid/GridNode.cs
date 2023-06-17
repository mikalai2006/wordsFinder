using UnityEngine;
[System.Flags]
public enum StateNode
{
  Disable = 1 << 0,
  Empty = 1 << 1,
  Occupied = 1 << 2,
  Open = 1 << 3,
  Coin = 1 << 4,
  Char = 1 << 5,
  Word = 1 << 6,
  Hint = 1 << 7
}


public class GridNode
{
  public int x;
  public int y;
  public StateNode StateNode = StateNode.Empty;
  GridHelper _gridHelper;
  Grid<GridNode> _grid;
  public Vector2 position;
  public int countVacantRight;

  public HiddenCharMB OccupiedChar;
  public HiddenWordMB OccupiedWord;

  public GridNode TopNode => _gridHelper.GetNode(x, y + 1);
  public GridNode BottomNode => _gridHelper.GetNode(x, y - 1);
  // public GridNode LeftNode => _gridHelper.GetNode(x - 1, y);
  // public GridNode RightNode => _gridHelper.GetNode(x + 1, y);

  public GridNode(Grid<GridNode> grid, GridHelper gridHelper, int x, int y)
  {
    _grid = grid;
    _gridHelper = gridHelper;
    position = new Vector2(x * _grid.cellSize, y * _grid.cellSize);
    countVacantRight = _grid.GetWidth() - x;
    this.x = x;
    this.y = y;
  }

  public void SetOccupiedChar(HiddenCharMB _occupiedChar, HiddenWordMB _word)
  {
    OccupiedChar = _occupiedChar;
    OccupiedWord = _word;
    StateNode |= StateNode.Occupied;
    _occupiedChar.OccupiedNode = this;
  }
  public void SetOpen()
  {
    StateNode |= StateNode.Open;
  }


#if UNITY_EDITOR
  public override string ToString()
  {
    return "GridNode:::" +
        "[x" + position.x + ",y" + position.y + "] \n" +
        "OccupiedUnit=" + OccupiedChar?.ToString() + ",\n" +
        "GuestedUnit=" + OccupiedWord?.ToString() + ",\n" +
        "StateNode=" + System.Convert.ToString((int)StateNode, 2) + ",\n";
  }
#endif
}