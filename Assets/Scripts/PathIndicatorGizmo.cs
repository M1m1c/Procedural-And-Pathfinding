using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathIndicatorGizmo : MonoBehaviour
{
    [SerializeField] private Color lineColor = Color.white;
    private LineRenderer lineRenderer;
    private List<Vector3> positions = new List<Vector3>();

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }

    public void SetupPath(List<HexTile> tiles, Vector3 startPos)
    {
        positions.Clear();
        positions.Add(startPos);
        foreach (var item in tiles)
        {
            positions.Add(item.transform.position);
        }

        DrawPath(positions);
    }

    public void RemovefirstPosition()
    {
        if (positions.Count != 0)
        {
            positions.RemoveAt(0);
        }
        DrawPath(positions);
    }

    private void DrawPath(List<Vector3> path)
    {

        if (path.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        var points = new Vector3[path.Count];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = path[i];
        }
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }
}
