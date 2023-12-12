using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
public class KDTree<T> : IEnumerable<T> where T : IKDTreeNode
{
    private readonly List<KDNode> _nodesCollection = new();

    public int Count => _nodesCollection.Count;

    KDNode root = null;
    readonly int numOfDimensions;

    [Serializable]
    public class KDNode
    {
        public KDNode left = null;
        public KDNode right = null;
        public int numOfDimensions;

        public T Value;

        // This is the data. Each node has K different properties
        public KDPoint point;

        public KDNode(T props, int numOfDimensions)
        {
            Value = props;
            this.numOfDimensions = numOfDimensions;
            point = new KDPoint(props.Position, numOfDimensions);
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
        public Vector3 position;
        private int numOfDimensions;

        public KDPoint(Vector3 props, int numOfDimensions)
        {
            this.position = props;
            this.numOfDimensions = numOfDimensions;
        }

        public float Get(int depth)
        {
            return position[depth % numOfDimensions];
        }

        public override string ToString()
        {
            string result = "(";
            for (int i = 0; i < numOfDimensions; i++)
            {
                float item = position[i];
                result += item.ToString() + (i == numOfDimensions - 1 ? "" : ", ");
            }

            return result + ") ";
        }
    }

    public KDTree(int numOfDimensions)
    {
        this.numOfDimensions = numOfDimensions;
    }

    public void Add(T point)
    {
        KDNode n = new KDNode(point, numOfDimensions);
        _nodesCollection.Add(n);
        if (root == null)
        {
            root = n;
        }
        else
        {
            root.Add(n);
        }
    }

    public void Remove(T point)
    {
        KDPoint kDPoint = new KDPoint(point.Position, numOfDimensions);
        KDNode result = NearestNeighbor(root, kDPoint, 0);
        if (result.point.position == point.Position)
        {
            _nodesCollection.Remove(result);
            UpdatePositions();
        }
    }

    public void UpdatePositions()
    {
        root = null;
        foreach (KDNode node in _nodesCollection)
        {
            if (root == null)
            {
                root = node;
            }
            else
            {
                root.Add(node);
            }
        }
    }

    //Traverses the tree at most twice in search of a nearest point
    public T NearestNeighbor(Vector3 target)
    {
        KDPoint kDPoint = new KDPoint(target, numOfDimensions);
        T result = NearestNeighbor(root, kDPoint, 0).Value;

        return result;
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

        float radiusSquared = DistanceSquared(target, best.point);

        /*
	     * We may need to check the other side of the tree. If the other side is closer than the radius,
	     * then we must recurse to the other side as well. 'dist' is either a horizontal or a vertical line
	     * that goes to an imaginary line that is splitting the plane by the root point.
	     */
        float dist = target.Get(depth) - root.point.Get(depth);

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

        float d1 = DistanceSquared(n0.point, target);
        float d2 = DistanceSquared(n1.point, target);

        if (d1 < d2)
            return n0;
        else
            return n1;
    }

    float DistanceSquared(KDPoint p0, KDPoint p1)
    {
        float total = 0;
        int numDims = numOfDimensions;

        for (int i = 0; i < numDims; i++)
        {
            float diff = Mathf.Abs(p0.Get(i) - p1.Get(i));
            total += Mathf.Pow(diff, 2);
        }
        return total;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _nodesCollection.Select(node => node.Value).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
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
        sb.Append("//Count: " + _nodesCollection.Count);
        return sb.ToString();
    }
}
