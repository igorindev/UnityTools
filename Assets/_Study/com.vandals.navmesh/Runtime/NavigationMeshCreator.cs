using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

//https://gamedev.stackexchange.com/questions/38721/how-can-i-generate-a-navigation-mesh-for-a-tile-grid
//https://medium.com/@mscansian/a-with-navigation-meshes-246fd9e72424
//https://www.gamedev.net/tutorials/programming/artificial-intelligence/generating-2d-navmeshes-r3393/
public class NavigationMeshCreator
{
    static int[][] vertexOffsets = new int[][] {
        new int[] { 0, 0 },
        new int[] { 1, 0 },
        new int[] { 1, 1 },
        new int[] { 0, 1 }
    };

    static int[] edgeTable = new int[] {
        0b0000, 0b0001, 0b0010, 0b1000,
        0b0011, 0b1010, 0b1100, 0b0101,
        0b1111, 0b0111, 0b1011, 0b1101,
        0b1110, 0b0110, 0b0010, 0b1001
    };

    static List<Vector2> MarchingSquares(float[,] data, float isovalue)
    {
        List<Vector2> vertices = new List<Vector2>();

        for (int y = 0; y < data.GetLength(0) - 1; y++)
        {
            for (int x = 0; x < data.GetLength(1) - 1; x++)
            {
                int cellIndex = 0;

                for (int i = 0; i < 4; i++)
                {
                    if (data[y + vertexOffsets[i][1], x + vertexOffsets[i][0]] >= isovalue)
                    {
                        cellIndex |= 1 << i;
                    }
                }

                if (cellIndex == 0 || cellIndex == 15)
                {
                    continue;
                }

                int edgeMask = edgeTable[cellIndex];

                for (int i = 0; i < 4; i++)
                {
                    if ((edgeMask & (1 << i)) != 0)
                    {
                        int v1Idx = i;
                        int v2Idx = (i + 1) % 4;

                        Vector2 point1 = new Vector2(x + vertexOffsets[v1Idx][0], y + vertexOffsets[v1Idx][1]);
                        Vector2 point2 = new Vector2(x + vertexOffsets[v2Idx][0], y + vertexOffsets[v2Idx][1]);

                        float t = (isovalue - data[(int)point1.y, (int)point1.x]) / (data[(int)point2.y, (int)point2.x] - data[(int)point1.y, (int)point1.x]);
                        Vector2 vertex = Vector2.Lerp(point1, point2, t);

                        vertices.Add(vertex);
                    }
                }
            }
        }

        return vertices;
    }

    static List<Vector2> DouglasPeucker(List<Vector2> points, double epsilon)
    {
        if (points == null || points.Count < 3)
        {
            return points;
        }

        int firstIndex = 0;
        int lastIndex = points.Count - 1;
        int index = -1;
        double dmax = 0.0;

        for (int i = 1; i < lastIndex; i++)
        {
            double d = PerpendicularDistance(points[i], points[firstIndex], points[lastIndex]);
            if (d > dmax)
            {
                index = i;
                dmax = d;
            }
        }

        if (dmax > epsilon)
        {
            List<Vector2> results1 = DouglasPeucker(points.GetRange(firstIndex, index), epsilon);
            List<Vector2> results2 = DouglasPeucker(points.GetRange(index, lastIndex - index + 1), epsilon);

            results1.RemoveAt(results1.Count - 1); // Remove the duplicated point
            return results1.Concat(results2).ToList();
        }
        else
        {
            return new List<Vector2> { points[firstIndex], points[lastIndex] };
        }
    }

    static double PerpendicularDistance(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        Vector2 diff = point - lineStart;
        Vector2 lineVector = lineEnd - lineStart;

        float t = Vector2.Dot(diff, lineVector) / lineVector.sqrMagnitude;
        t = Math.Max(0, Math.Min(1, t));

        Vector2 closestPoint = lineStart + t * lineVector;

        return Vector2.Distance(point, closestPoint);
    }

    public static void Create()
    {
        float[,] data = {
            { 1, 2, 2, 1 },
            { 1, 4, 4, 1 },
            { 1, 3, 2, 1 },
            { 1, 1, 1, 1 }
        };

        float isovalue = 2.5f;
        List<Vector2> vertices = MarchingSquares(data, isovalue);
        float epsilon = 0.5f; // Adjust this value to control RDP simplification

        List<Vector2> simplifiedVertices = DouglasPeucker(vertices, epsilon);

        Console.WriteLine("Simplified vertices:");
        foreach (var vertex in simplifiedVertices)
        {
            Console.WriteLine($"({vertex.x}, {vertex.y})");
        }

        //Triangulate 

        //// Create a list of points in Triangle.NET format
        //List<Point> triangleNetPoints = new List<Point>();
        //foreach (var vertex in vertices)
        //{
        //    triangleNetPoints.Add(new Point(simplifiedVertices.X, simplifiedVertices.Y));
        //}
        //
        //// Create a polygon from the points
        //Polygon polygon = new Polygon();
        //polygon.Add(triangleNetPoints);
        //
        //// Perform Delaunay triangulation
        //TriangleNet.Mesh mesh = new TriangleNet.Mesh();
        //mesh.Triangulate(polygon);
        //
        //// Access the triangles
        //foreach (var triangle in mesh.Triangles)
        //{
        //    // You can access the vertices and perform further operations
        //    Console.WriteLine($"Triangle vertices: ({triangle.GetVertex(0).X}, {triangle.GetVertex(0).Y}), " +
        //                      $"({triangle.GetVertex(1).X}, {triangle.GetVertex(1).Y}), " +
        //                      $"({triangle.GetVertex(2).X}, {triangle.GetVertex(2).Y})");
        //}
    }
}
