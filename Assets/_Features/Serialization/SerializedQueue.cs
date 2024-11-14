using System;
using System.Collections.Generic;
using UnityEngine;

namespace Serialization
{
    [Serializable]
    public class SerializedQueue<T> : Queue<T>, ISerializationCallbackReceiver
    {
        [SerializeField] List<T> queue = new List<T>();
        public void OnBeforeSerialize()
        {
            queue.Clear();

            foreach (T pair in this)
            {
                queue.Add(pair);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            foreach (T pair in queue)
            {
                Enqueue(pair);
            }
        }
    }
}