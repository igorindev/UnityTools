using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] Rigidbody[] rigidbodies;

    [ContextMenu("Start")]
    private void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
    }

    public void ToogleRagdoll(bool value, Rigidbody hittedRigidbody = null, Vector3 hitImpulse = default)
    {
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].isKinematic = !value;
        }

        if (hittedRigidbody != null)
        {
            hittedRigidbody.AddForce(hitImpulse, ForceMode.Impulse);
        }
    }

    [ContextMenu("True")]
    private void RagdollSetActive()
    {
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].isKinematic = false;
        }
    }

    [ContextMenu("False")]
    private void RagdollSetFalse()
    {
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].isKinematic = true;
        }
    }
}
