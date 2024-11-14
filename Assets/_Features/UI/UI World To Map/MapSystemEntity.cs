using UnityEngine;

public class MapSystemEntity : MonoBehaviour
{
    [SerializeField] MapSystem _mapSystem;

    void Start()
    {
        _mapSystem.AddEntity(transform, null);
    }
}
