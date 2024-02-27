using UnityEngine;

public class AssetNode : ScriptableObject
{
    public Object reference;
    public string guid;
    public Vector2 position;

    public virtual void OnDrawGizmos() { }
}
