using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
  private GameSetting _gameSetting;
  [SerializeField] private LineRenderer _lineRenderer;
  private List<Vector3> listPoints = new List<Vector3>();

  private void Awake()
  {
    _gameSetting = GameManager.Instance.GameSettings;
  }

  public void ResetLine()
  {
    listPoints.Clear();
    // _lineRenderer.SetPositions(new Vector3[0]);
    DrawLine();
  }

  public void DrawLine()
  {
    _lineRenderer.positionCount = listPoints.Count;
    _lineRenderer.startWidth = _gameSetting.lineWidth;
    _lineRenderer.endWidth = _gameSetting.lineWidth;
    _lineRenderer.SetPositions(listPoints.ToArray());
  }

  public void AddPoint(Vector3 position)
  {
    listPoints.Add(position);
  }

  public void RemovePoint(Vector3 position)
  {
    listPoints.Remove(position);
  }
}
