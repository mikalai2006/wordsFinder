
using UnityEngine;

public class GridSymbol
{
  public int x;
  public int y;

  public Vector2 position;

  public GridSymbol(int x, int y)
  {
    this.x = x;
    this.y = y;
    position = new Vector2(x, y);
  }
}