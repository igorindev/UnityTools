using System;

[Serializable]
public class Reference { }

[Serializable]
public class Reference<T> : Reference
{
    public bool useConstant = true;
    public T constantValue;
    public Action<T> onValueChange;

    public Variable<T> variable;

    public Reference() { }

    public Reference(T value)
    {
        useConstant = true;
        constantValue = value;
    }

    public virtual T Value
    {
        get { return useConstant ? constantValue : variable.GetValue(); }
    }

    public static implicit operator T(Reference<T> reference)
    {
        return reference.Value;
    }

    public string GetName
    {
        get { return useConstant ? "Local Variable" : variable.name; }
    }
}

public static class ReferenceExtension
{
    public static void Set<T>(this Reference<T> reference, T value)
    {
        if (reference.useConstant)
        {
            reference.constantValue = value;
        }
        else
        {
            Variable<T> variable = reference.variable as Variable<T>;
            variable.Set(value);
        }
        reference.onValueChange?.Invoke(value);
    }
}