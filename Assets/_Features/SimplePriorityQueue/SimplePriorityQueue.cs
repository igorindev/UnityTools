using System;
using System.Collections.Generic;

public struct SimplePriorityQueue<T> where T : IComparable<T>
{
    private List<T> list;

    private Comparer<T> reverseComparer;

    public readonly SimplePriorityQueue<T> Create()
    {
        return new SimplePriorityQueue<T>(new List<T>(), Comparer<T>.Create((x, y) => y.CompareTo(x)));
    }

    private SimplePriorityQueue(List<T> list, Comparer<T> reverseComparer)
    {
        this.list = list;

        // reverse-order
        this.reverseComparer = reverseComparer;
    }

    public readonly void Enqueue(T item)
    {
        // Binary search for insertion point
        int index = list.BinarySearch(item, reverseComparer);
        if (index < 0) index = ~index;
        list.Insert(index, item);
    }

    public readonly T Peek()
    {
        return list[^1];
    }

    public readonly T Dequeue()
    {
        T item = list[^1];
        list.RemoveAt(list.Count - 1); // Remove from last is performant than remove first
        return item;
    }

    public readonly IReadOnlyList<T> GetAllValues()
    {
        return list;
    }
}
