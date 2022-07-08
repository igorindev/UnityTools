using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutHole : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                DeleteTri(hit.triangleIndex);
            }
        }
    }

    void DeleteTri(int triangleIndex)
    {
        Destroy(gameObject.GetComponent<MeshCollider>());
        Mesh mesh = transform.GetComponent<MeshFilter>().mesh;

        int[] oldTriangles = mesh.triangles;
        //Um triangulo tem 3 vertices, então se tiro um triangulo, tiro 3 vertices
        int[] newTriangles = new int[mesh.triangles.Length - 3];

        int i = 0;
        int j = 0;

        while (j < mesh.triangles.Length)
        {
            //Se chega aos trianguloos ignorados, pula eles
            if (j != triangleIndex * 3)
            {
                newTriangles[i++] = oldTriangles[j++];
                newTriangles[i++] = oldTriangles[j++];
                newTriangles[i++] = oldTriangles[j++];
            }
            else
            {
                j += 3;
            }
        }
        transform.GetComponent<MeshFilter>().mesh.triangles = newTriangles;
        gameObject.AddComponent<MeshCollider>();
    }
}
