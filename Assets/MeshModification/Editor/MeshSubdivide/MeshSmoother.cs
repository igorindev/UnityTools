using UnityEngine;

public class MeshSmoother
{
    Vector3[] vertices;
    int[] triangles;

    public string meshName;

    private int[] subdivision = new int[] { 0, 2, 3, 4, 6, 8, 9, 12, 16, 18, 24 };

    [ContextMenu("Apply")]
    public Mesh Use(Mesh sourceMesh, int subdivisionLevel, int timesToSubdivide)
    {
        Mesh newMesh = (Mesh)UnityEngine.Object.Instantiate(sourceMesh);
        vertices = newMesh.vertices;
        triangles = newMesh.triangles;

        for (int i = 0; i < timesToSubdivide; i++)
        {
            MeshHelper.Subdivide(newMesh, subdivision[subdivisionLevel]);
        }

        vertices = newMesh.vertices;
        return newMesh;
    }
}