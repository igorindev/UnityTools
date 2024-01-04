using System;
using System.Collections.Generic;

[Serializable]
public abstract class ListVariable<T> : Variable<List<T>>
{
    public Action<T> onListValueChange;
    public Action<T, int> onListValueIndexChange;

    public void Set(T thing, int index)
    {
        if (value.Count > index)
        {
            value[index] = thing;
            onListValueChange?.Invoke(thing);
            onListValueIndexChange?.Invoke(thing, index);
        }
    }

    public void Add(T thing)
    {
        if (!value.Contains(thing))
        {
            value.Add(thing);
            onListValueChange?.Invoke(thing);
        }
    }

    public void Remove(T thing)
    {
        if (value.Contains(thing))
        {
            value.Remove(thing);
            onListValueChange?.Invoke(thing);
        }
    }

    public void Clear()
    {
        foreach (var item in value)
        {
            Remove(item);
        }
        onValueChange.Invoke(value);
    }
}