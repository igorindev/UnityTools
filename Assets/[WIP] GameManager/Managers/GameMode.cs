using UnityEngine;

public abstract class GameMode : ScriptableObject
{
    void Awake()
    {
        Initialize();
    }

    protected abstract void Initialize();
    protected abstract void OnUpdate();
}
