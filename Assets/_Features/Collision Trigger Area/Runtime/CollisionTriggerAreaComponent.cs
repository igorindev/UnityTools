using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class CollisionTriggerAreaComponent : MonoBehaviour
{
    [SerializeField] UnityEvent _onTriggerStart = null;
    [SerializeField] UnityEvent _onTriggerEnd = null;
    [SerializeField] string[] _tags = { "Player" };

    private Rigidbody _rb;
    private Collider _collider;

    public event UnityAction OnTriggerStart
    {
        add => _onTriggerStart.AddListener(value);
        remove => _onTriggerStart.RemoveListener(value);
    }

    public event UnityAction OnTriggerEnd
    {
        add => _onTriggerEnd.AddListener(value);
        remove => _onTriggerEnd.RemoveListener(value);
    }

    private void Reset()
    {
        Setup();
    }

    private void OnValidate()
    {
        Setup();
    }

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = true;
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < _tags.Length; i++)
        {
            if (other.CompareTag(_tags[i]))
            {
                _onTriggerStart?.Invoke();
                return;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < _tags.Length; i++)
        {
            if (other.CompareTag(_tags[i]))
            {
                _onTriggerEnd?.Invoke();
                return;
            }
        }
    }
}
