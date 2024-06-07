using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    [SerializeField] Vector2Int gridSize = new Vector2Int(1, 1);
    [SerializeField] List<Vector2> points = new List<Vector2>();
    [SerializeField] UIGridRenderer uiGridRenderer;
    [SerializeField, Min(0)] float thickness = 10f;

    float width;
    float height;
    float unitWidth;
    float unitHeight;

    private void Update()
    {
        if (uiGridRenderer != null)
        {
            if (gridSize != uiGridRenderer.GridSize)
            {
                gridSize = uiGridRenderer.GridSize;
                SetVerticesDirty();
            }
        }
    }

    public float GetAngle(Vector2 me)
    {
        return Mathf.Atan2(me.y, me.x) * (180 / Mathf.PI);
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        unitWidth = width / gridSize.x;
        unitHeight = height / gridSize.y;

        if (points.Count < 2)
        {
            return;
        }

        float angle = 0;

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 point = points[i];

            if (i > 0 && i < points.Count - 1)
            {
                Vector2 r0 = (points[i] - points[i - 1]).normalized;
                Vector2 r1 = (points[i + 1] - points[i]).normalized;
                Vector2 dir = (r0 + r1).normalized;
                

                Debug.DrawRay(Vector3.zero, r0, Color.green, 1000000000);

                Debug.DrawRay(Vector3.zero, r1, Color.red, 1000000000);

                Debug.DrawRay(Vector3.zero, dir, Color.yellow, 1000000000);
                dir = Vector2.Perpendicular(dir);
                Debug.DrawRay(Vector3.zero, dir, Color.blue, 1000000000);

                angle = GetAngle(-dir);
                Debug.Log(angle);
            }

            DrawVerticesForPoint(point, vh, angle);
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            int index = i * 2;
            vh.AddTriangle(index + 0, index + 1, index + 3);
            vh.AddTriangle(index + 3, index + 2, index + 0);
        }
    }

    void DrawVerticesForPoint(Vector2 point, VertexHelper vh, float angle)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        //vertex.position = new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        //vertex.position = new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);
    }
}
