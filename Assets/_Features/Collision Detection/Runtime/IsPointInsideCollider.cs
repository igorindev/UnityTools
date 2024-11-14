using UnityEngine;

public static partial class CollisionExtension
{
    public static bool IsPointInside(this Collider collider, Vector3 point)
    {
        Vector3 closest = collider.ClosestPoint(point);
        return closest == point;
    }
}
