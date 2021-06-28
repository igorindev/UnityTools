using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [SerializeField] Transform startControlPoint;
    [SerializeField] Transform endControlPoint;

    public Transform[] controlPoints;
    //1 e 4 - inicio e fim
    //2 e 3 - controles

    Vector3 gizmosPosition;

    public void SetPoints(int id, Vector3 point)
    {
        controlPoints[id].position = point;
        controlPoints[2].position = new Vector3(controlPoints[id].position.x, controlPoints[id].position.y + 0.1f, controlPoints[id].position.z);
    }

    private void OnDrawGizmos()
    {
        for (float t = 0; t <= 1; t += 0.05f)
        {
            gizmosPosition = Mathf.Pow(1 - t, 3) * controlPoints[0].position +
            3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1].position +
            3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2].position +
            Mathf.Pow(t, 3) * t * controlPoints[3].position;

            Gizmos.DrawSphere(gizmosPosition, 0.05f);
        }

        Gizmos.DrawLine(new Vector3(controlPoints[0].position.x, controlPoints[0].position.y, controlPoints[0].position.z), 
            new Vector3(controlPoints[1].position.x, controlPoints[1].position.y, controlPoints[1].position.z));

        Gizmos.DrawLine(new Vector3(controlPoints[2].position.x, controlPoints[2].position.y, controlPoints[2].position.z),
           new Vector3(controlPoints[3].position.x, controlPoints[3].position.y, controlPoints[3].position.z));
    }
}
