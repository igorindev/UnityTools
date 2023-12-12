using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SampleCode : MonoBehaviour
{
    public Vector3 target;
    public SampleTreeNode result;

    int[][] ar;

    public class SampleTreeNode : IKdTreePosition
    {
        public Vector3 Position { get; private set; }

        public SampleTreeNode(int x, int y, int z)
        {
            Position = new Vector3(x, y, z);
        }
    }

    public class SampleTreeNode2 : SampleTreeNode
    {
        public SampleTreeNode2(int x, int y, int z) : base(x, y, z) { }
    }

    KdTree<SampleTreeNode> tree;
    private void Start()
    {
        tree = new KdTree<SampleTreeNode>();

        ar = new int[][]
{
            new int[] {21, 74, 35},
            new int[] {87, 11, 63},
            new int[] {46, 71, 56},
            new int[] {9, 38, 72},
            new int[] {31, 92, 13},
            new int[] {77, 49, 41},
            new int[] {20, 76, 98},
            new int[] {63, 10, 92},
            new int[] {72, 98, 44},
            new int[] {2, 70, 68},
            new int[] {1, 16, 56},
            new int[] {34, 32, 94},
            new int[] {96, 85, 77},
            new int[] {75, 58, 85},
            new int[] {14, 55, 5},
            new int[] {31, 48, 12},
            new int[] {99, 20, 46},
            new int[] {8, 80, 65},
            new int[] {95, 29, 97},
            new int[] {44, 26, 25},
            new int[] {93, 4, 37},
            new int[] {9, 9, 6},
            new int[] {42, 95, 37},
            new int[] {58, 44, 8},
            new int[] {44, 35, 55},
            new int[] {40, 97, 95},
};

        foreach (int[] pos in ar)
        {
            tree.Add(new SampleTreeNode(pos[0], pos[1], pos[2]));
        }
    }

    [ContextMenu("Tree")]
    void CreateTree()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        result = tree.FindClosest(target);
        // Log the elapsed time in milliseconds
        UnityEngine.Debug.Log("FULL TREE loop Execution Time: " + stopwatch.Elapsed.TotalMilliseconds + " ms");
        // Stop the stopwatch
        stopwatch.Stop();

        Debug.Log("Tree = " + tree.ToString());
        Debug.Log(result.ToString());
        Debug.Log(result.Position);
    }

    void OnDrawGizmos()
    {
        if (ar == null) return;

        foreach (int[] item in ar)
        {
            Gizmos.color = Color.red;
            var v = new Vector3(item[0], item[1], item[2]);
            Gizmos.DrawSphere(v, 0.5f);
        }

        if (result != null)
        {
            Gizmos.color = Color.green;
            var v = new Vector3(result.Position[0], result.Position[1], result.Position[2]);
            Gizmos.DrawSphere(v, 0.8f);
        }

        if (target != null)
        {
            Gizmos.color = Color.blue;
            var v = target;
            Gizmos.DrawSphere(v, 0.8f);
        }
    }

    [ContextMenu("List")]
    void Main()
    {
        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();
        Vector3[] ar = new Vector3[]
            {
            new Vector3(21, 74, 35),
            new Vector3(87, 11, 63),
            new Vector3(46, 71, 56),
            new Vector3(9, 38, 72),
            new Vector3(31, 92, 13),
            new Vector3(77, 49, 41),
            new Vector3(20, 76, 98),
            new Vector3(63, 10, 92),
            new Vector3(72, 98, 44),
            new Vector3(2, 70, 68),
            new Vector3(1, 16, 56),
            new Vector3(34, 32, 94),
            new Vector3(96, 85, 77),
            new Vector3(75, 58, 85),
            new Vector3(14, 55, 5),
            new Vector3(31, 48, 12),
            new Vector3(99, 20, 46),
            new Vector3(8, 80, 65),
            new Vector3(95, 29, 97),
            new Vector3(44, 26, 25),
            new Vector3(93, 4, 37),
            new Vector3(9, 9, 6),
            new Vector3(42, 95, 37),
            new Vector3(58, 44, 8),
            new Vector3(44, 35, 55),
            new Vector3(40, 97, 95),
        };

        // Target point
        Vector3 targetPoint = target;
        
        // Find the closest point
        Vector3 closestPoint = FindClosestPoint(ar, targetPoint);

        // Log the elapsed time in milliseconds
        UnityEngine.Debug.Log("FindClosestPoint loop Execution Time: " + stopwatch.Elapsed.TotalMilliseconds + " ms");
        // Stop the stopwatch
        stopwatch.Stop();

        // Output the result
        Debug.Log($"The closest point to {targetPoint} is {closestPoint} //Count: " + ar.Length);
    }

    static Vector3 FindClosestPoint(Vector3[] points, Vector3 target)
    {
        // If the array is empty, handle it as needed (throw an exception, return a default value, etc.)
        if (points.Length == 0)
        {
            throw new InvalidOperationException("The array is empty.");
        }

        Vector3 closestPoint = points[0];
        float minDistance = Vector3.Distance(target, points[0]);

        // Iterate through the array to find the closest point
        for (int i = 1; i < points.Length; i++)
        {
            float currentDistance = Vector3.Distance(target, points[i]);

            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                closestPoint = points[i];
            }
        }

        return closestPoint;
    }
}