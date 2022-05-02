using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectReflection : EditorWindow
{
    [SerializeField] Transform reflectionPoint;

    SerializedObject so;
    SerializedProperty reflectionPointProperty;

    [MenuItem("Tools/Object Reflection")]
    static void Init()
    {
        GetWindow<ObjectReflection>("Object Reflection");
    }

    void OnEnable()
    {
        so = new SerializedObject(this);

        reflectionPointProperty = so.FindProperty(nameof(reflectionPoint));
    }

    void OnGUI()
    {
        so.Update();
        EditorGUILayout.PropertyField(reflectionPointProperty);

        if (GUILayout.Button("Reflect"))
        {
            List<GameObject> toReflect = new List<GameObject>();

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                if (Selection.gameObjects[i].scene.IsValid())
                {
                    toReflect.Add(Selection.gameObjects[i]);
                }
            }

            if (toReflect.Count > 0)
                Reflect(toReflect.ToArray());
        }

        if (GUILayout.Button("Create Reflection Link"))
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                if (Selection.gameObjects[i].scene.IsValid())
                {
                    Debug.Log(Selection.gameObjects[i].name);
                }
            }

        }

        so.ApplyModifiedProperties();
    }

    [ContextMenu("Reflect")]
    void Reflect(GameObject[] toReflect)
    {
        for (int i = 0; i < toReflect.Length; i++)
        {
            Vector3 pos = ReflectPosition(toReflect[i].transform.position, reflectionPoint.position);

            Quaternion rot = ReflectRotation(toReflect[i].transform.rotation, reflectionPoint.forward);

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