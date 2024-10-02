using System;
using UnityEditor;
using UnityEngine;

namespace MeshPivotUpdater
{
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

        public override void OnInspectorGUI()
        {
            GUI.enabled = true;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Edit Pivot"))
            {
                MeshPivotInspectorEditor.OpenMeshAsset(mesh);
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
}