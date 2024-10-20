using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode, AddComponentMenu("Spline/Spline Creator")]
public class SplineCreator : MonoBehaviour
{
#if UNITY_EDITOR
    public List<GameObject> controlPoints = new List<GameObject>();

    [Header("Config")]
    [Min(2)] public int precision = 10;

    [Header("Line Render")]
    public Color color = Color.white;
    [Min(0.1f)] public float width = 0.2f;

    Vector3[] movePositions;
    LineRenderer lineRenderer;

    void Start()
    {
        if (lineRenderer == null)
        {
            if (TryGetComponent(out LineRenderer l))
            {
                lineRenderer = l;
            }
            else
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }

            lineRenderer.useWorldSpace = true;
            lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
        }
    }

    void Update()
    {
        if (lineRenderer != null && controlPoints != null && controlPoints.Count >= 3)
        {
            // update line renderer
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;

            lineRenderer.positionCount = precision * (controlPoints.Count - 2);
            movePositions = new Vector3[precision * (controlPoints.Count - 2)];

            Vector3 p0, p1, p2;
            for (int j = 0; j < controlPoints.Count - 2; j++)
            {
                // check control points
                if (controlPoints[j] == null || controlPoints[j + 1] == null || controlPoints[j + 2] == null)
                {
                    return;
                }
                // determine control points of segment
                p0 = 0.5f * (controlPoints[j].transform.position
                + controlPoints[j + 1].transform.position);
                p1 = controlPoints[j + 1].transform.position;
                p2 = 0.5f * (controlPoints[j + 1].transform.position
                + controlPoints[j + 2].transform.position);

                // set points of quadratic Bezier curve
                Vector3 position;
                float t;
                float pointStep = 1.0f / precision;
                if (j == controlPoints.Count - 3)
                {
                    pointStep = 1.0f / (precision - 1.0f);
                    // last point of last segment should reach p2
                }
                for (int i = 0; i < precision; i++)
                {
                    t = i * pointStep;
                    position = (1.0f - t) * (1.0f - t) * p0 + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
                    lineRenderer.SetPosition(i + j * precision, position);
                    movePositions[i + j * precision] = position;
                }
            }
        }
    }

    public void Bake()
    {
        SplineBaked bake = gameObject.AddComponent<SplineBaked>();

        Vector3[] splineControlPoints = new Vector3[controlPoints.Count];
        for (int i = 0; i < controlPoints.Count; i++)
        {
            splineControlPoints[i] = controlPoints[i].transform.position;
            DestroyImmediate(controlPoints[i]);
        }

        bake.CreateSpline(movePositions, splineControlPoints, precision);

        DestroyImmediate(GetComponent<LineRenderer>());
        DestroyImmediate(this);
    }

#endif
}
