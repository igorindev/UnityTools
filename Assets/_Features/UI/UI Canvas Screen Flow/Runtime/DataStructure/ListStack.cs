using System.Collections.Generic;

namespace CanvasSubsystem
{
    public class ListStack<T>
    {
        readonly List<T> items = new List<T>();

        public int Count { get => items.Count; }

        /// <summary>
        /// Add an element to the top of the stack 
        /// </summary>
        public void Push(T item)
        {
            items.Add(item);
        }

        /// <summary>
        /// Remove the element at the top of the stack 
        /// </summary>
        public T Pop()
        {
            if (items.Count > 0)
            {
                T temp = items[items.Count - 1];
                items.RemoveAt(items.Count - 1);
                return temp;
            }
            else
                return default;
        }

        /// <summary>
        /// Return who is at the top of the stack
        /// </summary>
        public T Peek()
        {
            if (items.Count > 0)
            {
                T temp = items[items.Count - 1];
                return temp;
            }
            else
                return default;
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public void RemoveAt(int itemAtPosition)
        {
            items.RemoveAt(itemAtPosition);
        }

        public void Remove(T item)
        {
            items.Remove(item);
        }
    }
}
