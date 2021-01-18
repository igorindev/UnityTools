using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class TriggerCaller : MonoBehaviour
{
    [SerializeField] UnityEvent onTriggerStart = null, onTriggerEnd = null;
    [SerializeField] string Tag = "Player";
    [Header("Colors")]
    [SerializeField] Color inside = new Vector4(0, 1, 0, 0.5f);
    [SerializeField] Color outside = new Vector4(1, 0, 0, 0.5f);

    bool playerInside = false;
    
    [SerializeField] Collider box;

    public bool PlayerInside { get => playerInside; set => playerInside = value; }

    private void Reset()
    {
        GetComponent<Rigidbody>().useGravity = false;
        box = GetComponent<Collider>();
        box.isTrigger = true;
        gameObject.layer = 23;

        if (TryGetComponent(out BoxCollider bc))
        {
            bc.center = Vector3.zero;
        }
        else if(TryGetComponent(out SphereCollider sc))
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
        if (other.CompareTag(Tag))
        {
            onTriggerStart?.Invoke();
            playerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tag))
        {
            onTriggerEnd?.Invoke();
            playerInside = false;
        }
    }
}
