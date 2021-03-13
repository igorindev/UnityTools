using UnityEditor;
using UnityEngine;

public class MeshSimplifierWizard : ScriptableWizard
{
    public Mesh sourceMesh;
    [Range(0,1)] public float Quality = 0.5f;
    public string whereToSave = "Assets/";

    public void Simpliy()
    {
        float quality = Quality;
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(sourceMesh);
        meshSimplifier.SimplifyMesh(quality);
        Mesh destMesh = meshSimplifier.ToMesh();

        AssetDatabase.CreateAsset(destMesh, whereToSave + sourceMesh.name + " Simplified.asset");
        AssetDatabase.SaveAssets();
    }

    void OnWizardCreate()
    {
        if (sourceMesh != null)
        {
            Simpliy();
        }
        else
        {
            Debug.LogError("You need to assign a mesh");
        }
    }

    [MenuItem("Tools/Mesh/Decimate")]
    static void MeshSimplifier()
    {
        DisplayWizard<MeshSimplifierWizard>("Mesh Simplifier");
    }
}
