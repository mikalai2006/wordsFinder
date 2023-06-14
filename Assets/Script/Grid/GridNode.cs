using UnityEngine;
[System.Flags]
public enum StateNode
{
  Disable = 1 << 0,
  Empty = 1 << 1,
  Occupied = 1 << 2,
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
  public HiddenWordMB Word;

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
    Word = _word;
    StateNode |= StateNode.Occupied;
  }
}