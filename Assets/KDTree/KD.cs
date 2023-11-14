using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IKdTreePosition
{
    Vector3 Position { get; }
}

public class KdTree<T> : IEnumerable<T> where T : IKdTreePosition
{
    private readonly Stack<KdNode> _searchStack = new();
    private readonly List<KdNode> _nodesCollection = new();

    private KdNode _root;

    public int Count => _nodesCollection.Count;

    public T this[int key]
    {
        get
        {
            return _nodesCollection[key].Value;
        }
    }

    public void Add(T item)
    {
        KdNode kdNode = new KdNode() { Value = item };
        _nodesCollection.Add(kdNode);
        AttachToTree(kdNode);
    }

    public void RemoveAt(int i)
    {
        _nodesCollection.RemoveAt(i);
    }

    public void UpdatePositions()
    {
        _root = null;
        foreach (var node in _nodesCollection)
        {
            AttachToTree(node);
        }
    }

    public T FindClosest(Vector3 position)
    {
        return FindClosestInternal(position);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _nodesCollection.Select(node => node.Value).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void AttachToTree(KdNode newNode)
    {
        newNode.Left = null;
        newNode.Right = null;
        newNode.Level = 0;
        newNode.CachedPosition = newNode.Value.Position;

        var parent = FindParentInternal(newNode.CachedPosition);

        if (parent == null)
        {
            _root = newNode;
            return;
        }

        var splitParent = GetSplitValue(parent.Level, parent.CachedPosition);
        var splitNew = GetSplitValue(parent.Level, newNode.CachedPosition);

        newNode.Level = parent.Level + 1;

        if (splitNew < splitParent)
        {
            parent.Left = newNode;
        }
        else
        {
            parent.Right = newNode;
        }
    }

    private KdNode FindParentInternal(Vector3 position)
    {
        var current = _root;
        var parent = _root;
        while (current != null)
        {
            var splitCurrent = GetSplitValue(current.Level, current.CachedPosition);
            var splitTarget = GetSplitValue(current.Level, position);

            parent = current;
            if (splitTarget < splitCurrent)
            {
                current = current.Left;
            }
            else
            {
                current = current.Right;
            }
        }
        return parent;
    }

    private T FindClosestInternal(Vector3 position)
    {
        if (_root == null)
        {
            return default;
        }

        _searchStack.Clear();
        _searchStack.Push(_root);

        var nearestSqrDistance = float.MaxValue;
        KdNode nearestNode = _root;

        while (_searchStack.Count != 0)
        {
            var currentNode = _searchStack.Pop();

            var sqrDistance = Vector3.SqrMagnitude(position - currentNode.CachedPosition);
            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearestNode = currentNode;
            }

            var splitCurrent = GetSplitValue(currentNode.Level, currentNode.CachedPosition);
            var splitTarget = GetSplitValue(currentNode.Level, position);

            float splitDifference = splitCurrent - splitTarget;
            float splitSqrDifference = splitDifference * splitDifference;

            if (splitTarget < splitCurrent)
            {
                if (currentNode.Left != null)
                {
                    _searchStack.Push(currentNode.Left);
                }
                if (currentNode.Right != null && splitSqrDifference < nearestSqrDistance)
                {
                    _searchStack.Push(currentNode.Right);
                }
            }
            else
            {
                if (currentNode.Right != null)
                {
                    _searchStack.Push(currentNode.Right);
                }
                if (currentNode.Left != null && splitSqrDifference < nearestSqrDistance)
                {
                    _searchStack.Push(currentNode.Left);
                }
            }
        }

        return nearestNode.Value;
    }

    private float GetSplitValue(int level, Vector3 position)
    {
        switch (level % 3)
        {
            case 0: return position.x;
            case 1: return position.y;
            case 2: return position.z;
            default: throw new InvalidOperationException();
        }
    }

    private class KdNode
    {
        public T Value;
        public Vector3 CachedPosition;
        public int Level;
        public KdNode Left;
        public KdNode Right;
    }
}