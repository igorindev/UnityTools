using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKHeadLookTarget : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] [Range(0, 1)] float lookWeight = 1;
    Animator anim;

    public Transform Target { get => target; set => target = value; }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (anim != null)
        {
            if (Target != null)
            {
                anim.SetLookAtWeight(lookWeight);
                anim.SetLookAtPosition(Target.position);
            }
        }
    }
}
