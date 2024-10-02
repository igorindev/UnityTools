using UnityEditor;
using UnityEngine;

namespace MeshPivotUpdater
{
    public static class MeshSaverEditor
    {
        public static Mesh SaveMeshNewInstance(Mesh mesh)
        {
            return SaveMesh(mesh, true);
        }

        private static Mesh SaveMesh(Mesh mesh, bool optimizeMesh)
        {
            string path = EditorUtility.SaveFilePanel("Save Mesh", Application.dataPath, mesh.name, "asset");
            if (string.IsNullOrEmpty(path))
            {
                return mesh;
            }

            path = FileUtil.GetProjectRelativePath(path);

            Mesh meshToSave = Object.Instantiate(mesh);

            if (optimizeMesh)
            {
                MeshUtility.Optimize(meshToSave);
            }

            AssetDatabase.CreateAsset(meshToSave, path);
            AssetDatabase.SaveAssets();

            return meshToSave;
        }
    }
}