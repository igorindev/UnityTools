using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class TriggerCaller : MonoBehaviour
{
    [SerializeField] UnityEvent onTriggerStart = null, onTriggerEnd = null;
    [SerializeField] string[] tags = { "Player" };
    
    Rigidbody rb;
    new Collider collider;

    void OnValidate()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        collider = GetComponent<Collider>();
        collider.isTrigger = true;

        if (TryGetComponent(out BoxCollider bc))
        {
            bc.center = Vector3.zero;
        }
        else if (TryGetComponent(out SphereCollider sc))
        {
            sc.center = Vector3.zero;
        }
        else if (TryGetComponent(out CapsuleCollider cc))
        {
            cc.center = Vector3.zero;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (other.CompareTag(tags[i]))
            {
                onTriggerStart?.Invoke();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (other.CompareTag(tags[i]))
            {
                onTriggerEnd?.Invoke();
            }
        }
    }
}
