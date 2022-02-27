using UnityEngine;

public abstract class PlayerManagerController : ScriptableObject
{
    void Awake()
    {
        Initialize();
    }

    protected abstract void Initialize();
    protected abstract void Enable();
    protected abstract void Disable();
}

