using UnityEngine;

namespace IKSolver
{
    [RequireComponent(typeof(Animator))]
    public class IKHead : MonoBehaviour
    {
        [SerializeField] [Range(0, 1)] float weight = 1;
        [SerializeField] float visibilityAngle = 45;
        [SerializeField] float lookSpeed = 5;
        [SerializeField] float rangeRadius = 5;
        [SerializeField] Transform target;

        Animator anim;
        float value;
        Vector3 currentTarget;
        Vector3 a;

        void Start()
        {
            anim = GetComponent<Animator>();
        }

        void OnAnimatorIK(int layerIndex)
        {
            if (target != null)
            {
                Vector3 pos = target.position;
                pos.y += 1.5f;

                currentTarget = Vector3.SmoothDamp(currentTarget, pos, ref a, 0.2f);

                IsVisible(currentTarget);
            }
            else
            {
                value -= Time.deltaTime * lookSpeed;
                value = Mathf.Clamp(value, 0, weight);
            }

            anim.SetLookAtPosition(currentTarget);
            anim.SetLookAtWeight(value);
        }

        void IsVisible(Vector3 t)
        {
            if (MathExtension.TargetDirection(t, transform.position, transform.forward, visibilityAngle))
            {
                value += Time.deltaTime * lookSpeed;
                value = Mathf.Clamp(value, 0, weight);
            }
            else
            {
                value -= Time.deltaTime * lookSpeed;
                value = Mathf.Clamp(value, 0, weight);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Vector4(0, 1, 0, 0.4f);
            Gizmos.DrawSphere(transform.position, rangeRadius);
        }
    }
}