using UnityEngine;

public class ObjectReflection : MonoBehaviour
{
    public Transform reflectionPoint;
    public Transform[] toReflect;

    [ContextMenu("Reflect")]
    void Reflect()
    {
        for (int i = 0; i < toReflect.Length; i++)
        {
            Vector3 pos = ReflectPosition(toReflect[i].position, reflectionPoint.position);

            Quaternion rot = ReflectRotation(toReflect[i].rotation, reflectionPoint.forward);

            Instantiate(toReflect[i], pos, rot);
        }

    }

    Vector3 ReflectPosition(Vector3 sourcePosition, Vector3 mirror)
    {
        Vector3 reflectionDir = sourcePosition - mirror;
        return reflectionPoint.position + Vector3.Reflect(reflectionDir, reflectionPoint.forward);
    }

    Quaternion ReflectRotation(Quaternion sourceRotation, Vector3 mirrorNormal)
    {
        return Quaternion.LookRotation(Vector3.Reflect(sourceRotation * Vector3.forward, mirrorNormal), Vector3.Reflect(sourceRotation * Vector3.up, mirrorNormal));
    }
}