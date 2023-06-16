using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
  private GameSetting _gameSetting;
  [SerializeField] private LineRenderer _lineRenderer;
  [SerializeField] private Material _material;
  private List<Vector3> listPoints = new List<Vector3>();

  private void Awake()
  {
    _gameSetting = GameManager.Instance.GameSettings;
    _lineRenderer.startColor = _gameSetting.Theme.colorLine;
    _lineRenderer.endColor = _gameSetting.Theme.colorLine;
  }

  public void ResetLine()
  {
    listPoints.Clear();
    // _lineRenderer.SetPositions(new Vector3[0]);
    DrawLine();
  }

  public void DrawLine()
  {
    // _lineRenderer.positionCount = listPoints.Count;
    _lineRenderer.startWidth = _gameSetting.lineWidth;
    _lineRenderer.endWidth = _gameSetting.lineWidth;
    // _lineRenderer.SetPositions(listPoints.ToArray());

    //get smoothed values
    Vector3[] smoothedPoints = LineSmoother.SmoothLine(listPoints.ToArray(), 0.15f);

    _lineRenderer.positionCount = smoothedPoints.Length;
    // _lineRenderer.SetVertexCount(smoothedPoints.Length);
    _lineRenderer.SetPositions(smoothedPoints);
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
