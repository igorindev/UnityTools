using UnityEditor;
using UnityEngine;

public class MeshSmootherWizard : ScriptableWizard
{
    public Mesh sourceMesh;
    [Tooltip("Divide meshes in submeshes to generate more triangles")]
    [Range(0, 10)]
    public int subdivisionLevel = 1;

    [Tooltip("Repeat the process this many times")]
    [Range(0, 10)]
    public int timesToSubdivide = 1;

    public string whereToSave = "Assets/";

    public void Smooth()
    {
        var meshSmoother = new MeshSmoother();
        Mesh newMesh = meshSmoother.Use(sourceMesh, subdivisionLevel, timesToSubdivide);

        AssetDatabase.CreateAsset(newMesh, whereToSave + newMesh.name + " Smooth.asset");
        AssetDatabase.SaveAssets();
    }

    void OnWizardCreate()
    {
        if (sourceMesh != null)
        {
            Smooth();
        }
        else
        {
            Debug.LogError("You need to assign a mesh");
        }
    }

    [MenuItem("Tools/Mesh/Subdivide")]
    static void MeshSmoother()
    {
        DisplayWizard<MeshSmootherWizard>("Mesh Smoother");
    }
}
