using System;
using System.Collections.Generic;
using UnityEngine;

#region Variables
[Serializable]
public class StringReference : Reference<string> { }

[Serializable]
public class FloatReference : Reference<float> { }

[Serializable]
public class IntReference : Reference<int> { }

[Serializable]
public class BoolReference : Reference<bool> { }

[Serializable]
public class Vector2Reference : Reference<Vector2> { }

[Serializable]
public class AudioClipReference : Reference<AudioClip> { }
#endregion

#region List
[Serializable]
public abstract class ReferenceList<T> : Reference<List<T>>
{
    public Action<T> onListValueChange;

    public void AddRange(List<T> thing)
    {
        foreach (var item in thing)
        {
            Add(item);
        }
    }

    public void Add(T thing)
    {
        Value.Add(thing);
        onListValueChange?.Invoke(thing);
    }

    public void Remove(T thing)
    {
        if (Value.Contains(thing))
        {
            Value.Remove(thing);
            onListValueChange?.Invoke(thing);
        }
    }

    public void Clear()
    {
        Value.Clear();
        onValueChange?.Invoke(Value);
    }
}

[Serializable]
public class GameObjectListReference : ReferenceList<GameObject>
{
    public GameObject Instance
    {
        get { return constantValue.Count > 0 ? constantValue[0] : null; }
    }

    public void InstantiateAll(Transform transform)
    {
        foreach (var item in constantValue)
        {
            UnityEngine.Object.Instantiate(item, transform);
        }
    }

    public void DestroyAll()
    {
        foreach (var item in constantValue)
        {
            onListValueChange?.Invoke(item);
            UnityEngine.Object.Destroy(item);
        }
        constantValue.Clear();
    }
}

[Serializable]
public class StringListReference : ReferenceList<string> { }

[Serializable]
public class IntListReference : ReferenceList<int> { }

[Serializable]
public class BoolListReference : ReferenceList<bool> { }

[Serializable]
public class SpriteReference : Reference<Sprite> { }

[Serializable]
public class SpriteListReference : ReferenceList<Sprite> { }

[Serializable]
public class ColorReference : Reference<Color> { }
#endregion