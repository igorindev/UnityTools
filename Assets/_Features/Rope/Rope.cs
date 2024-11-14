using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public LineRenderer line;
    public Transform targetA;
    public Transform targetB;
    public Vector3 currentTarget;

    public List<Vector3> hits;

    void Start()
    {
        currentTarget = targetB.position;
    }

    void Update()
    {
        if (hits.Count == 0)
        {
            currentTarget = targetB.position;
        }

        line.positionCount = hits.Count + 2;

        line.SetPosition(0, targetA.position);
        line.SetPosition(1, currentTarget);

        int v = 2;

        for (int i = hits.Count - 1; i >= 0; i--)
        {
            line.SetPosition(v, hits[i]);
            v++;
        }

        if (hits.Count > 0)
        {
            Vector3 dir = Direction(hits[hits.Count - 1], targetA.position, out float magnitude);
            Debug.DrawRay(targetA.position, dir * magnitude, Color.red);

            bool value = Physics.Raycast(targetA.position, dir, out RaycastHit hit, magnitude);
            if (value == false)
            {
                currentTarget = hits[hits.Count - 1];
                hits.RemoveAt(hits.Count - 1);
                return;
            }
            else
            {
                if ((hit.point - hits[hits.Count - 1]).sqrMagnitude < 0.1f * 0.1f)
                {
                    currentTarget = hits[hits.Count - 1];
                    hits.RemoveAt(hits.Count - 1);
                    return;
                }
            }
        }

        {
            Vector3 dir = Direction(currentTarget, targetA.position, out float magnitude);
            Debug.DrawRay(targetA.position, dir * magnitude, Color.green);

            if (Physics.Raycast(targetA.position, dir, out RaycastHit hitInfo, magnitude))
            {
                if ((hitInfo.point - currentTarget).sqrMagnitude > 0.1f * 0.1f) //not the same
                {
                    hits.Add(currentTarget);
                    currentTarget = hitInfo.point;
                }
            }
        }
    }

    Vector3 Direction(Vector3 o1, Vector3 o2, out float magnitude)
    {
        Vector3 v = (o1 - o2);
        magnitude = v.magnitude;
        return v.normalized;
    }
}
