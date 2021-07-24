using UnityEngine;

public class AISenseVision : AISense
{
    [SerializeField, Range(0, 180)] float visionFov;
    [SerializeField] float enterVisionDistance;
    [SerializeField] float exitVisionDistance;

    Collider[] colliders = new Collider[1];

    public override bool Sense(ref Transform target, ref Vector3 lastPosition)
    {
        //If there is no target
        if (target == null)
        {
            //Check if Join in range
            if (Physics.OverlapSphereNonAlloc(transform.TransformPoint(localSenseOrigin), enterVisionDistance, colliders, affectMask) > 0)
            {
                //Check if inside visible range
                if (true)
                {
                    target = colliders[0].transform;
                    lastPosition = target.position;
                    isSensed = true;
                    return true;
                }
            }

            isSensed = false;
            return false;
        }
        else
        {
            lastPosition = target.position;

            //Check if out of range
            if (Vector3.Distance(target.position, transform.TransformPoint(localSenseOrigin)) > exitVisionDistance)
            {
                target = null;
                isSensed = false;
                return false;
            }
            //In range but theres a obstacle
            else if (Physics.Linecast(transform.TransformPoint(localSenseOrigin), target.position))
            {
               // target = null;
               // isSensed = false;
               // return false;
            }
            isSensed = true;
            return true;
        }
    }

    //Player vision stimulus should find AI in range (the max NPC possible range) and update then trying to find the player

    void OnDrawGizmos()
    {
        Gizmos.color = new Vector4(0, 0.5f, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.TransformPoint(localSenseOrigin) + Vector3.up * 0.01f, enterVisionDistance);

        Gizmos.color = new Vector4(0.5f, 0, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.TransformPoint(localSenseOrigin) + Vector3.up * 0.01f, exitVisionDistance);

        Quaternion rr = Quaternion.AngleAxis(visionFov, transform.up);
        Quaternion rl = Quaternion.AngleAxis(-visionFov, transform.up);
        Vector3 dirr = rr * transform.forward;
        Vector3 dirl = rl * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.TransformPoint(localSenseOrigin) + Vector3.up * 0.01f, dirr * exitVisionDistance);
        Gizmos.DrawRay(transform.TransformPoint(localSenseOrigin) + Vector3.up * 0.01f, dirl * exitVisionDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.TransformPoint(localSenseOrigin) + Vector3.up * 0.01f, dirr * enterVisionDistance);
        Gizmos.DrawRay(transform.TransformPoint(localSenseOrigin) + Vector3.up * 0.01f, dirl * enterVisionDistance);
    }
}
