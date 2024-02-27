using UnityEngine;

public class SplineBaked : MonoBehaviour
{
    [SerializeField] Vector3[] m_splinePoints = new Vector3[0];
    [SerializeField] Vector3[] m_splineControlPositions = new Vector3[0];

    int m_precision = 0;

    public Vector3[] SplinePoints { get => m_splinePoints; }
    public Vector3[] SplineControlPositions { get => m_splineControlPositions; }
    public int Precision { get => m_precision; }

    public void CreateSpline(Vector3[] splinePoints, Vector3[] splineControlPositions, int precision)
    {
        m_splinePoints = splinePoints;
        m_splineControlPositions = splineControlPositions;
        m_precision = precision;
    }

    void OnDrawGizmos()
    {
        if (m_splinePoints == null || m_splinePoints.Length == 0) return;

        Gizmos.color = Color.blue;
        for (int i = 0; i < m_splinePoints.Length - 1; i++)
        {
            Gizmos.DrawLine(m_splinePoints[i], m_splinePoints[i + 1]);
        }   
    }
}
