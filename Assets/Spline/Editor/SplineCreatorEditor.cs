using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplineCreator))]
public class SplineCreatorEditor : Editor
{
    override public void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var myScript = target as SplineCreator;
        EditorGUILayout.Space(5);
        if (GUILayout.Button("Bake"))
        {
            myScript.Bake();
        }
    }
}