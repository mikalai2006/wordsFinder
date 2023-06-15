using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridHelper
{
  private Grid<GridNode> _grid;
  public Grid<GridNode> Grid
  {
    get { return _grid; }
    private set { }
  }

  public GridHelper(int width, int height, float cellSize = 0.5f)
  {
    _grid = new Grid<GridNode>(width, height, cellSize, this, (Grid<GridNode> grid, GridHelper gridHelper, int x, int y) => new GridNode(grid, gridHelper, x, y));
  }

  public List<GridNode> GetAllGridNodes()
  {
    List<GridNode> list = new List<GridNode>(_grid.GetWidth() * _grid.GetHeight());
    if (_grid != null)
    {
      for (int x = 0; x < _grid.GetWidth(); x++)
      {
        for (int y = 0; y < _grid.GetHeight(); y++)
        {
          list.Add(_grid.GetGridObject(new Vector3Int(x, y)));
        }
      }
    }
    return list;
  }

  public GridNode GetNode(int x, int y)
  {
    return _grid.GetGridObject(new Vector3Int(x, y));
  }
  public GridNode GetNode(Vector3Int pos)
  {
    return _grid.GetGridObject(pos);
  }

  public List<GridNode> FindNodeForSpawnWord(string word, int index)
  {
    var countChars = word.Length;
    List<GridNode> result = new();
    var potentialNodes = GetAllGridNodes()
      .Where(t => t.x <= _grid.GetWidth() - countChars && t.y <= index && t.StateNode.HasFlag(StateNode.Empty))
      .OrderBy(t => t.y)
      .ThenBy(t => t.x);

    var chooseNode = potentialNodes.First();
    result = GetLineNodesByCount(chooseNode, countChars);
    return result;
  }

  private List<GridNode> GetLineNodesByCount(GridNode startNode, int count)
  {
    List<GridNode> result = GetAllGridNodes()
      .Where(t => t.y == startNode.y && t.x >= startNode.x && t.x < startNode.x + count)
      .OrderBy(t => t.x)
      .ToList();
    foreach (var node in result)
    {
      node.StateNode ^= StateNode.Empty;
    }
    var lastNode = result.LastOrDefault();
    if (GetNode(lastNode.x + 1, lastNode.y) != null)
    {
      GetNode(lastNode.x + 1, lastNode.y).StateNode ^= StateNode.Empty;
    }

    return result;
  }

  public List<GridNode> FindNeighboursNodesOfByEqualChar(GridNode startNode)
  {
    List<GridNode> result = new();

    if (
        startNode.TopNode != null
        && startNode.TopNode.OccupiedChar != null
        && startNode.OccupiedChar.charTextValue == startNode.TopNode.OccupiedChar.charTextValue
      )
    {
      result.Add(startNode.TopNode);
    }
    if (
        startNode.BottomNode != null
        && startNode.BottomNode.OccupiedChar != null
        && startNode.OccupiedChar.charTextValue == startNode.BottomNode.OccupiedChar.charTextValue
      )
    {
      result.Add(startNode.BottomNode);
    }

    return result;
  }
}
