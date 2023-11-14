using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/**
 * Supporting code for YouTube tutorial ( https://www.youtube.com/watch?v=Glp7THUpGow ) on KD-Trees.  
 * 
 * Basic implementation of a KD-Tree for finding the nearest neighbor. KDNode is a typical tree node with 
 * left and right pointers, but instead of a single value, it stores a List of values, encapsulated by KDPoint class.
 * 
 * For more information, see https://en.wikipedia.org/wiki/K-d_tree
 * @author Andre Violentyev
 */

public interface IKDTreeNode
{
    Vector3 Position { get; }
}

[Serializable]
public class KDTree<T> where T : IKDTreeNode
{
    private readonly Stack<KDNode> _searchStack = new();
    private readonly List<KDNode> _nodesCollection = new();

    [Serializable]
    public class KDNode
    {
        public KDNode left = null;
        public KDNode right = null;
        public int numOfDimensions;

        public T Value;
        public Vector3 CachedPosition;

        // This is the data. Each node has K different properties
        public KDPoint point;

        public KDNode(List<int> props)
        {
            point = new KDPoint(props);
            numOfDimensions = props.Count;
        }

        public KDNode(KDPoint point)
        {
            this.point = point;
            numOfDimensions = point.props.Count;
        }

        public void Add(KDNode n)
        {
            Add(n, 0);
        }

        void Add(KDNode n, int k)
        {
            if (n.point.Get(k) < point.Get(k))
            {
                if (left == null)
                {
                    left = n;
                }
                else
                {
                    left.Add(n, k + 1);
                }
            }
            else
            {
                if (right == null)
                {
                    right = n;
                }
                else
                {
                    right.Add(n, k + 1);
                }
            }
        }

        public override string ToString()
        {
            return "Point: " + point.ToString();
        }
    }

    //This class represents a data point, with K different properties
    [Serializable]
    public class KDPoint
    {
        public List<int> props;

        public KDPoint(List<int> props)
        {
            this.props = props;
        }

        public int Get(int depth)
        {
            return props[depth % props.Count];
        }

        public int Size()
        {
            return props.Count;
        }

        public override string ToString()
        {
            string result = "(";
            for (int i = 0; i < props.Count; i++)
            {
                int item = props[i];
                result += item.ToString() + (i == props.Count - 1 ? "" : ", ");
            }

            return result + ") ";
        }
    }

    KDNode root = null;
    readonly int numOfDimensions;

    public KDTree(int numDims)
    {
        numOfDimensions = numDims;
    }

    public KDTree(KDNode root)
    {
        this.root = root;
        numOfDimensions = root.point.Size();
    }

    //note that all points must have exactly the same number of dimensions
    public KDTree(List<List<int>> points)
    {
        numOfDimensions = points[0].Count;
        root = new KDNode(points[0]);

        for (int i = 1, numPoints = points.Count; i < numPoints; i++)
        {
            List<int> point = points[i];
            KDNode n = new KDNode(point);
            root.Add(n);
        }
    }

    public void Add(List<int> point)
    {
        KDNode n = new KDNode(point);
        if (root == null)
        {
            root = n;
        }
        else
        {
            root.Add(n);
        }
    }

    //Traverses the tree at most twice in search of a nearest point
    public KDNode NearestNeighbor(KDPoint target)
    {
        return NearestNeighbor(root, target, 0);
    }

    KDNode NearestNeighbor(KDNode root, KDPoint target, int depth)
    {
        if (root == null) return null;
        KDNode nextBranch;
        KDNode otherBranch;
        // compare the property appropriate for the current depth
        if (target.Get(depth) < root.point.Get(depth))
        {
            nextBranch = root.left;
            otherBranch = root.right;
        }
        else
        {
            nextBranch = root.right;
            otherBranch = root.left;
        }

        // recurse down the branch that's best according to the current depth
        KDNode temp = NearestNeighbor(nextBranch, target, depth + 1);
        KDNode best = Closest(temp, root, target);

        long radiusSquared = DistanceSquared(target, best.point);

        /*
	     * We may need to check the other side of the tree. If the other side is closer than the radius,
	     * then we must recurse to the other side as well. 'dist' is either a horizontal or a vertical line
	     * that goes to an imaginary line that is splitting the plane by the root point.
	     */
        long dist = target.Get(depth) - root.point.Get(depth);

        if (radiusSquared >= dist * dist)
        {
            temp = NearestNeighbor(otherBranch, target, depth + 1);
            best = Closest(temp, best, target);
        }

        return best;
    }

    //Determines whether n0 or n1 is closer to the target. Does NOT recurse any deeper.
    KDNode Closest(KDNode n0, KDNode n1, KDPoint target)
    {
        if (n0 == null) return n1;

        if (n1 == null) return n0;

        long d1 = DistanceSquared(n0.point, target);
        long d2 = DistanceSquared(n1.point, target);

        if (d1 < d2)
            return n0;
        else
            return n1;
    }

    static long DistanceSquared(KDPoint p0, KDPoint p1)
    {
        long total = 0;
        int numDims = p0.props.Count;

        for (int i = 0; i < numDims; i++)
        {
            int diff = Mathf.Abs(p0.Get(i) - p1.Get(i));
            total += (int)Mathf.Pow(diff, 2);
        }
        return total;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        Queue<KDNode> q = new Queue<KDNode>();
        q.Enqueue(root);

        while (!(q.Count == 0))
        {
            int size = q.Count;
            for (int i = 0; i < size; i++)
            {
                KDNode n = q.Dequeue();
                if (n != null)
                {
                    sb.Append(n.point).Append(", ");
                    q.Enqueue(n.left);
                    q.Enqueue(n.right);
                }
                else
                {
                    sb.Append("null, ");
                }
            }
            sb.Append('\n');
        }
        return sb.ToString();
    }

    [ContextMenu("test")]
    public void CreateATest()
    {
        KDTree<IKDTreeNode> tree = new KDTree<IKDTreeNode>(2);

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
            new int[] {27, 13, 7},
            new int[] {18, 92, 62},
            new int[] {3, 59, 46},
            new int[] {69, 93, 78},
            new int[] {22, 31, 7},
            new int[] {8, 64, 2},
            new int[] {63, 95, 10},
            new int[] {56, 5, 70},
            new int[] {24, 94, 67},
            new int[] {13, 8, 98},
            new int[] {91, 13, 19},
            new int[] {84, 8, 54},
            new int[] {18, 19, 20},
            new int[] {22, 21, 68},
            new int[] {61, 41, 60},
            new int[] {6, 89, 9},
            new int[] {100, 8, 52},
            new int[] {50, 94, 7},
            new int[] {77, 51, 84},
            new int[] {10, 61, 76},
            new int[] {93, 2, 38},
            new int[] {43, 23, 71},
            new int[] {81, 70, 91},
            new int[] {42, 56, 89},
            new int[] {36, 19, 46},
            new int[] {57, 23, 48},
            new int[] {79, 39, 37},
            new int[] {23, 62, 14},
            new int[] {66, 98, 87},
            new int[] {31, 60, 32},
            new int[] {71, 38, 69},
            new int[] {39, 68, 26},
            new int[] {41, 59, 4},
            new int[] {63, 34, 54},
            new int[] {92, 31, 7},
            new int[] {74, 97, 68},
            new int[] {2, 72, 7},
            new int[] {56, 50, 91},
            new int[] {15, 70, 34},
            new int[] {60, 16, 91},
            new int[] {67, 42, 89},
            new int[] {59, 19, 43},
            new int[] {86, 9, 17},
            new int[] {67, 66, 100},
            new int[] {36, 36, 48},
            new int[] {2, 32, 56},
            new int[] {59, 43, 9},
            new int[] {51, 74, 94},
            new int[] {23, 99, 89},
            new int[] {64, 53, 50},
            new int[] {61, 67, 48},
            new int[] {99, 66, 84},
            new int[] {48, 45, 68},
            new int[] {54, 95, 98},
            new int[] {35, 49, 59},
            new int[] {40, 18, 29},
            new int[] {9, 39, 79},
            new int[] {59, 79, 6},
            new int[] {25, 99, 3},
            new int[] {9, 89, 52},
            new int[] {20, 37, 93},
            new int[] {69, 55, 79},
            new int[] {47, 66, 48},
            new int[] {13, 97, 14},
            new int[] {68, 52, 77},
            new int[] {25, 6, 38},
            new int[] {57, 2, 58},
            new int[] {17, 99, 77},
            new int[] {57, 76, 80},
            new int[] {12, 41, 59},
            new int[] {54, 5, 70},
            new int[] {72, 69, 66},
            new int[] {60, 95, 35},
            new int[] {53, 64, 89},
            new int[] {3, 60, 67},
            new int[] {3, 44, 46},
};

        foreach (int[] item in ar)
        {
            var r = new List<int>
            {
                item[0],
                item[1],
                item[2]
            };

            tree.Add(r);
        }

        DebugExtension.Log("Tree = " + tree.ToString());

        List<int> r2 = new List<int>
        {
            target.x,
            target.y,
            target.z
        };

        result = tree.NearestNeighbor(new KDTree<IKDTreeNode>.KDPoint(r2));
        DebugExtension.Log(result.ToString());
    }

    public Vector3Int target;
    public KDTree<IKDTreeNode>.KDNode result;
    public int[][] ar = new int[][]
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
            new int[] {27, 13, 7},
            new int[] {18, 92, 62},
            new int[] {3, 59, 46},
            new int[] {69, 93, 78},
            new int[] {22, 31, 7},
            new int[] {8, 64, 2},
            new int[] {63, 95, 10},
            new int[] {56, 5, 70},
            new int[] {24, 94, 67},
            new int[] {13, 8, 98},
            new int[] {91, 13, 19},
            new int[] {84, 8, 54},
            new int[] {18, 19, 20},
            new int[] {22, 21, 68},
            new int[] {61, 41, 60},
            new int[] {6, 89, 9},
            new int[] {100, 8, 52},
            new int[] {50, 94, 7},
            new int[] {77, 51, 84},
            new int[] {10, 61, 76},
            new int[] {93, 2, 38},
            new int[] {43, 23, 71},
            new int[] {81, 70, 91},
            new int[] {42, 56, 89},
            new int[] {36, 19, 46},
            new int[] {57, 23, 48},
            new int[] {79, 39, 37},
            new int[] {23, 62, 14},
            new int[] {66, 98, 87},
            new int[] {31, 60, 32},
            new int[] {71, 38, 69},
            new int[] {39, 68, 26},
            new int[] {41, 59, 4},
            new int[] {63, 34, 54},
            new int[] {92, 31, 7},
            new int[] {74, 97, 68},
            new int[] {2, 72, 7},
            new int[] {56, 50, 91},
            new int[] {15, 70, 34},
            new int[] {60, 16, 91},
            new int[] {67, 42, 89},
            new int[] {59, 19, 43},
            new int[] {86, 9, 17},
            new int[] {67, 66, 100},
            new int[] {36, 36, 48},
            new int[] {2, 32, 56},
            new int[] {59, 43, 9},
            new int[] {51, 74, 94},
            new int[] {23, 99, 89},
            new int[] {64, 53, 50},
            new int[] {61, 67, 48},
            new int[] {99, 66, 84},
            new int[] {48, 45, 68},
            new int[] {54, 95, 98},
            new int[] {35, 49, 59},
            new int[] {40, 18, 29},
            new int[] {9, 39, 79},
            new int[] {59, 79, 6},
            new int[] {25, 99, 3},
            new int[] {9, 89, 52},
            new int[] {20, 37, 93},
            new int[] {69, 55, 79},
            new int[] {47, 66, 48},
            new int[] {13, 97, 14},
            new int[] {68, 52, 77},
            new int[] {25, 6, 38},
            new int[] {57, 2, 58},
            new int[] {17, 99, 77},
            new int[] {57, 76, 80},
            new int[] {12, 41, 59},
            new int[] {54, 5, 70},
            new int[] {72, 69, 66},
            new int[] {60, 95, 35},
            new int[] {53, 64, 89},
            new int[] {3, 60, 67},
            new int[] {3, 44, 46},
};

    public void DrawGizmos()
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
            var v = new Vector3(result.point.props[0], result.point.props[1], result.point.props[2]);
            Gizmos.DrawSphere(v, 0.8f);
        }

        if (target != null)
        {
            Gizmos.color = Color.blue;
            var v = target;
            Gizmos.DrawSphere(v, 0.8f);
        }
    }
}

public class Cc : IKDTreeNode
{
    public Vector3 Position => Vector3.zero;
}

public class get
{
    void test()
    {
        KDTree<Cc> tree = new KDTree<Cc>(2);

        int[][] ar = new int[][]
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
            new int[] {27, 13, 7},
            new int[] {18, 92, 62},
            new int[] {3, 59, 46},
            new int[] {69, 93, 78},
            new int[] {22, 31, 7},
            new int[] {8, 64, 2},
            new int[] {63, 95, 10},
            new int[] {56, 5, 70},
            new int[] {24, 94, 67},
            new int[] {13, 8, 98},
            new int[] {91, 13, 19},
            new int[] {84, 8, 54},
            new int[] {18, 19, 20},
            new int[] {22, 21, 68},
            new int[] {61, 41, 60},
            new int[] {6, 89, 9},
            new int[] {100, 8, 52},
            new int[] {50, 94, 7},
            new int[] {77, 51, 84},
            new int[] {10, 61, 76},
            new int[] {93, 2, 38},
            new int[] {43, 23, 71},
            new int[] {81, 70, 91},
            new int[] {42, 56, 89},
            new int[] {36, 19, 46},
            new int[] {57, 23, 48},
            new int[] {79, 39, 37},
            new int[] {23, 62, 14},
            new int[] {66, 98, 87},
            new int[] {31, 60, 32},
            new int[] {71, 38, 69},
            new int[] {39, 68, 26},
            new int[] {41, 59, 4},
            new int[] {63, 34, 54},
            new int[] {92, 31, 7},
            new int[] {74, 97, 68},
            new int[] {2, 72, 7},
            new int[] {56, 50, 91},
            new int[] {15, 70, 34},
            new int[] {60, 16, 91},
            new int[] {67, 42, 89},
            new int[] {59, 19, 43},
            new int[] {86, 9, 17},
            new int[] {67, 66, 100},
            new int[] {36, 36, 48},
            new int[] {2, 32, 56},
            new int[] {59, 43, 9},
            new int[] {51, 74, 94},
            new int[] {23, 99, 89},
            new int[] {64, 53, 50},
            new int[] {61, 67, 48},
            new int[] {99, 66, 84},
            new int[] {48, 45, 68},
            new int[] {54, 95, 98},
            new int[] {35, 49, 59},
            new int[] {40, 18, 29},
            new int[] {9, 39, 79},
            new int[] {59, 79, 6},
            new int[] {25, 99, 3},
            new int[] {9, 89, 52},
            new int[] {20, 37, 93},
            new int[] {69, 55, 79},
            new int[] {47, 66, 48},
            new int[] {13, 97, 14},
            new int[] {68, 52, 77},
            new int[] {25, 6, 38},
            new int[] {57, 2, 58},
            new int[] {17, 99, 77},
            new int[] {57, 76, 80},
            new int[] {12, 41, 59},
            new int[] {54, 5, 70},
            new int[] {72, 69, 66},
            new int[] {60, 95, 35},
            new int[] {53, 64, 89},
            new int[] {3, 60, 67},
            new int[] {3, 44, 46},
};

        foreach (int[] item in ar)
        {
            var r = new List<int>
            {
                item[0],
                item[1],
                item[2]
            };

            tree.Add(r);
        }

        DebugExtension.Log("Tree = " + tree.ToString());

        List<int> r2 = new List<int>
        {
            1,2,3
        };

        KDTree<Cc>.KDNode nearestNeighbor = tree.NearestNeighbor(new KDTree<Cc>.KDPoint(r2));
        DebugExtension.Log(nearestNeighbor.ToString());
    }
}