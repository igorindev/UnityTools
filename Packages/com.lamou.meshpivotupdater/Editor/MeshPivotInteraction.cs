using UnityEditor;
using UnityEngine;

namespace MeshPivotUpdater
{
    public static class MeshPivotInteraction
    {
        private static Mesh mesh;
        private static bool selected;

        [MenuItem("Assets/Edit Mesh Pivot...")]
        public static void Edit()
        {
            if (mesh == null)
            {
                return;
            }

            Open(mesh);
        }

        [MenuItem("Assets/Edit Mesh Pivot...", true)]
        public static bool CheckIfMesh()
        {
            var context = Selection.activeObject;

            selected = false;
            if (context is Mesh m)
            {
                mesh = m;
                selected = true;
            }

            return selected;
        }

        private static void Open(Mesh mesh)
        {
            MeshPivotInspectorEditor.OpenMeshAsset(mesh);
        }
    }
}