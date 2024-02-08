using UnityEditor;
using UnityEngine;

public static class ContextMenuExtensions
{
    [MenuItem("CONTEXT/BoxCollider/Set to Child Bounds")]
    public static void SetBoxColliderToChildBounds(MenuCommand command)
    {
        BoxCollider collider = (BoxCollider) command.context;
        Bounds bounds = GetCombinedBounds(collider);

        collider.center = bounds.center;
        collider.size = bounds.size;
    }
    static Bounds GetCombinedBounds(Collider parent)
    {
        Bounds combinedBounds = new Bounds();
        combinedBounds.Encapsulate(parent.bounds);
        Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers) 
        {
            if (combinedBounds.size == Vector3.zero) combinedBounds = r.bounds;

            combinedBounds.Encapsulate(r.bounds);
        }

        return combinedBounds;
    }
}
