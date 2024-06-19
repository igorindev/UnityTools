using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Mesh), true)]
public class MeshEditorExtended : Editor
{
    Editor defaultEditor;
    Mesh mesh;

    void OnEnable()
    {
        defaultEditor = CreateEditor(targets, Type.GetType("UnityEditor.ModelInspector, UnityEditor"));
        mesh = target as Mesh;
    }

    void OnDisable()
    {
        MethodInfo disableMethod = defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        disableMethod?.Invoke(defaultEditor, null);
        //DestroyImmediate(defaultEditor);
    }

    public override void OnInspectorGUI()
    {
        GUI.enabled = true;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Edit Pivot"))
        {
            MeshPivotUpdater.OpenMeshAsset(mesh);
        }

        EditorGUILayout.EndHorizontal();
        defaultEditor.OnInspectorGUI();
    }

    public override void DrawPreview(Rect previewArea)
    {
        defaultEditor.DrawPreview(previewArea);
    }

    public override bool HasPreviewGUI()
    {
        return defaultEditor.HasPreviewGUI();
    }
}
