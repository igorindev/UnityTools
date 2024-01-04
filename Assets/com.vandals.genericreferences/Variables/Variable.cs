using System;
using UnityEngine;

[Serializable]
public abstract class Variable : ScriptableObject { }

[Serializable]
public abstract class Variable<T> : Variable
{
    public bool debug;
#if UNITY_EDITOR
    [Multiline]
    public string developerDescription = "";
#endif
    public T value = default;
    public Action<T> onValueChange = null;

    public void Set(T Value)
    {
        if (debug)
        {
            Debug.Log($"Setting {name}: {Value}");
        }
        onValueChange?.Invoke(Value);
        value = Value;
    }

    public T GetValue()
    {
        return value;
    }
}