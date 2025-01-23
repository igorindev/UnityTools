using UnityEditor;
using UnityEngine;

public static class TriggerCallerMenuEditor
{
    [MenuItem("GameObject/Trigger/Trigger Box", false, 0)]
    static void CreateTriggerBox()
    {
        GameObject _object = new GameObject("Trigger Box");
        _object.AddComponent<BoxCollider>();
        _object.AddComponent<CollisionTriggerAreaComponent>();
    }
    [MenuItem("GameObject/Trigger/Trigger Sphere", false, 0)]
    static void CreateTriggerSphere()
    {
        GameObject _object = new GameObject("Trigger Sphere");
        _object.AddComponent<SphereCollider>();
        _object.AddComponent<CollisionTriggerAreaComponent>();
    }
    [MenuItem("GameObject/Trigger/Trigger Capsule", false, 0)]
    static void CreateTriggerCapsule()
    {
        GameObject _object = new GameObject("Trigger Capsule");
        _object.AddComponent<CapsuleCollider>();
        _object.AddComponent<CollisionTriggerAreaComponent>();
    }

}
