using UnityEditor;
using UnityEngine;

namespace MeshPivotUpdater
{
    public static class MeshSaverEditor
    {
        public static bool SaveMeshNewInstance(Mesh mesh, out Mesh result)
        {
            return SaveMesh(mesh, true, out result);
        }

        private static bool SaveMesh(Mesh mesh, bool optimizeMesh, out Mesh result)
        {
            string path = EditorUtility.SaveFilePanel("Save Mesh", Application.dataPath, mesh.name, "asset");
            if (string.IsNullOrEmpty(path))
            {
                result = mesh;
                return false;
            }

            path = FileUtil.GetProjectRelativePath(path);

            Mesh meshToSave = Object.Instantiate(mesh);

            if (optimizeMesh)
            {
                MeshUtility.Optimize(meshToSave);
            }

            AssetDatabase.CreateAsset(meshToSave, path);
            AssetDatabase.SaveAssets();

            result = meshToSave;
            return true;
        }
    }
}