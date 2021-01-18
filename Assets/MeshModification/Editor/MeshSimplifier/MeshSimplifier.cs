using UnityEditor;
using UnityEngine;

public class MeshSimplifier
{
    [SerializeField] Mesh sourceMesh = null;
    [Range(0, 1)] 
    [SerializeField] float Quality = 0.5f;

    [ContextMenu("Simplify")]
    public void Simpliy()
    {
        float quality = Quality;
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(sourceMesh);
        meshSimplifier.SimplifyMesh(quality);
        Mesh destMesh = meshSimplifier.ToMesh();

        AssetDatabase.CreateAsset(destMesh, "Assets/" + sourceMesh.name + " Simplified.asset");
        AssetDatabase.SaveAssets();
    }
}
