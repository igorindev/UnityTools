using UnityEngine;

public class AISenseVision : AISense
{
    [SerializeField, Range(0, 180)] float visionFov;
    [SerializeField] float enterVisionDistance;
    [SerializeField] float exitVisionDistance;

    [SerializeField] Transform target;

    public Vector3 LastSawPos { get; private set; }

    public override bool Sense(ref Transform target, ref Vector3 lastPosition)
    {
        Ray ray = new Ray(transform.TransformPoint(localSenseOrigin), transform.forward);

        //If there is no target
        if (target == null)
        {
            //Check if Join in range
            if (Physics.SphereCast(ray, enterVisionDistance, out RaycastHit hit, affectMask))
            {
                //Check if inside visible range
                if (true)
                {
                    target = hit.transform;
                    lastPosition = target.position;
                    return true;
                }
            }

            return false;
        }
        else
        {
            lastPosition = target.position;

            //Check if out of range
            if (Vector3.Distance(target.position, transform.TransformPoint(localSenseOrigin)) > exitVisionDistance)
            {
                target = null;
            }
            //In range but theres a obstacle
            else if (Physics.Linecast(transform.TransformPoint(localSenseOrigin), target.position))
            {
                target = null;
            }

            return true;
        }
    }

    void OnDrawGizmosSelected()
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
