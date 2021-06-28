using System.Collections;
using UnityEngine;

public class Parabole : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] Transform start;
    [SerializeField] Transform end;
    [Header("Configuration")]
    [SerializeField, Min(2)] int numOfPoints;
    [SerializeField] float height;

    void OnDrawGizmos()
    {
        for (int i = 0; i < numOfPoints; i++)
        {
            Gizmos.DrawSphere(SampleParabola(start.position, end.position, height, Mathf.Clamp01((float)i/(numOfPoints-1))), 0.1f);
        }
    }

    Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
    {
        float parabolicT = t * 2 - 1;
        if (Mathf.Abs(start.y - end.y) < 0.1f)
        {
            //start and end are roughly level, pretend they are - simpler solution with less steps
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result.y += (-parabolicT * parabolicT + 1) * height;
            return result;
        }
        else
        {
            //start and end are not level, gets more complicated
            Vector3 travelDirection = end - start;
            Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
            Vector3 up = Vector3.Cross(right, travelDirection);
            if (end.y > start.y) up = -up;
            Vector3 result = start + t * travelDirection;
            result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
            return result;
        }
    }

}
