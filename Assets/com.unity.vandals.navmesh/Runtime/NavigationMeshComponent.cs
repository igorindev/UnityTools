using System;
using System.Collections.Generic;
using UnityEngine;

//https://gamedev.stackexchange.com/questions/38721/how-can-i-generate-a-navigation-mesh-for-a-tile-grid
//https://medium.com/@mscansian/a-with-navigation-meshes-246fd9e72424
//need to apply funnel algorithm for navmesh
public class NavigationMeshComponent : MonoBehaviour
{
    [SerializeField] NavigationMeshPoint[] navigationMeshPoints;

    public Transform origin;
    public Transform end;

    NavigationMeshPoint currentPoint;

    public List<NavigationMeshPoint> points;
    public List<NavigationMeshPoint> path;

    NavigationMeshTriangle currentTriangle;
    NavigationMeshTriangle endTriangle;

    [ContextMenu("Is Inside")]
    public void Test()
    {
        CalculatePath(origin.position, end.position);
    }

    public void CalculatePath(Vector3 origin, Vector3 target)
    {
        currentTriangle = FindTriangle(origin);
        endTriangle = FindTriangle(target);

        points.Clear();
        path.Clear();

        if (currentTriangle != null)
            FindSmallestDistanceToTarget(currentTriangle.Vertex, target);
        else
            Debug.LogWarning("Not placed in the navmesh");

        //Optimize path;
    }

    NavigationMeshTriangle FindTriangle(Vector3 origin)
    {
        for (int i = 0; i < navigationMeshPoints.Length; i += 3)
        {
            NavigationMeshPoint index1 = navigationMeshPoints[i];
            NavigationMeshPoint index2 = navigationMeshPoints[i + 1];
            NavigationMeshPoint index3 = navigationMeshPoints[i + 2];

            NavigationMeshTriangle navigationMeshTriangle = new NavigationMeshTriangle(index1, index2, index3);
            if (navigationMeshTriangle.IsPointInTriangle(origin))
            {
                return navigationMeshTriangle;
            }
        }

        return null;
    }

    public void FindSmallestDistanceToTarget(NavigationMeshPoint[] inputArray, Vector3 target)
    {
        if (inputArray == null || inputArray.Length == 0)
            throw new ArgumentException("Array must not be null or empty");

        float smallestValue = inputArray[0].CalculatePointCost(currentPoint.Position, target);
        NavigationMeshPoint smallest = inputArray[0];

        if (points.Contains(inputArray[0]) == false)
            points.Add(inputArray[0]);

        for (int i = 1; i < inputArray.Length; i++)
        {
            if (points.Contains(inputArray[i]) == false)
                points.Add(inputArray[i]);

            float cost = inputArray[i].CalculatePointCost(currentPoint.Position, target);
            if (cost < smallestValue)
            {
                smallest = inputArray[i];
                smallestValue = cost;
            }
        }

        path.Add(smallest);

        if (endTriangle.IsPointInTriangle(smallest.Position))
        {
            return;
        }
        else
        {
            FindSmallestDistanceToTarget(smallest.Neighbors, target);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < navigationMeshPoints.Length; i++)
        {
            Gizmos.DrawSphere(navigationMeshPoints[i].Position, 0.25f);
        }
    }
}

[Serializable]
public class NavigationMeshPoint
{
    [SerializeField] Vector3 position;
    [SerializeField] NavigationMeshPoint[] neighbors;

    public Vector3 Position { get => position; }
    public NavigationMeshPoint[] Neighbors { get => neighbors; }

    public float CalculatePointCost(Vector3 origin, Vector3 end)
    {
        //F = G + H
        return Vector3.Distance(origin, Position) + Vector3.Distance(origin, end);
    }
}

[Serializable]
public class NavigationMeshTriangle
{
    [SerializeField] NavigationMeshPoint[] vertex = new NavigationMeshPoint[3];

    public NavigationMeshPoint[] Vertex { get => vertex; }

    public NavigationMeshTriangle(params NavigationMeshPoint[] vertex)
    {
        this.vertex = vertex;
    }

    //Barycentric Technique https://blackpawn.com/texts/pointinpoly/default.html
    public bool IsPointInTriangle(Vector3 point) //  Vector3 vertexA, Vector3 vertexB, Vector3 vertexC
    {
        Vector3 v0 = Vertex[2].Position - Vertex[0].Position;
        Vector3 v1 = Vertex[1].Position - Vertex[0].Position;
        Vector3 v2 = point - Vertex[0].Position;

        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        float invDenom = 1f / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        return (u >= 0) && (v >= 0) && (u + v < 1);
    }
}