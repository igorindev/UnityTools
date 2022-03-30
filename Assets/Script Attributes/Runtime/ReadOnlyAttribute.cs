using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute 
{
    public bool Hide { get; set;} = false;
}