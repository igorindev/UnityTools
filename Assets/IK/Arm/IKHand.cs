using UnityEngine;

namespace IKSolver
{
    [RequireComponent(typeof(Animator))]
    public class IKHand : MonoBehaviour
    {
        [SerializeField] bool enableHandIk;
        Animator anim;

        public Vector3 GrabPosition { get; set; }
        bool grabbing;

        void Start()
        {
            anim = GetComponent<Animator>();
        }

        void FixedUpdate()
        {
            if (!enableHandIk) { return; }
            if (anim == null) { Debug.LogError("No Animator assigned"); return; }
        }

        void OnAnimatorIK(int layerIndex)
        {
            if (!enableHandIk) { return; }

            if (grabbing)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

                anim.SetIKPosition(AvatarIKGoal.LeftHand, GrabPosition);
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            }
        }
    }
}