using UnityEngine;

public abstract class AISense : MonoBehaviour
{
    [SerializeField] protected Vector3 localSenseOrigin;
    [SerializeField] protected LayerMask affectMask;
    [ReadOnly] [SerializeField] protected bool isSensed;
    public abstract bool Sense(ref Transform target, ref Vector3 lastPosition);
}
