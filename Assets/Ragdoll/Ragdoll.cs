using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] Rigidbody[] rigidbodies;

    [ContextMenu("Start")]
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
    }

    public void RagdollSetActive(bool value, Rigidbody hittedRigidbody = null, Vector3 hitDir = default, float hitImpulseForce = 100)
    {
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].isKinematic = !value;
        }

        if (hittedRigidbody != null)
        {
            hittedRigidbody.AddForce(hitDir * hitImpulseForce, ForceMode.Impulse);
        }
    }

    [ContextMenu("True")]
    void RagdollSetActive()
    {
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].isKinematic = false;
        }
    }

    [ContextMenu("False")]
    void RagdollSetFalse()
    {
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].isKinematic = true;
        }
    }
}
