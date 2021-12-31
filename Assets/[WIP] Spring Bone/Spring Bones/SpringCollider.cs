using UnityEngine;

namespace SpringSystem
{
    public class SpringCollider : MonoBehaviour
    {
        public float radius = 0.5f;

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}