using UnityEngine;  //This needs to be 'removed' to make 'universal', i.e. not tied to Unity3D
using System.Collections;
using System;

public class SpatialHash
{
    [Serializable]
    public class HashObject
    {
        public Vector3 insertPos;
        public Transform currentTransform;

        public HashObject(Transform item)
        {
            insertPos = item.position;
            currentTransform = item;
        }
    }

    private readonly Hashtable hashTable;
    private readonly int cellSize;

    public SpatialHash(int cellSize)
    {
        this.cellSize = cellSize;
        this.hashTable = new Hashtable();
    }

    public int Count
    {
        get { return hashTable.Count; }
    }

    public ICollection Cells
    {
        get { return hashTable.Keys; }
    }

    public void Insert(Vector3 v, object obj)
    {
        ArrayList cell;
        foreach (string key in Keys(v))
        {
            if (hashTable.Contains(key))
                cell = (ArrayList)hashTable[key];
            else
            {
                cell = new ArrayList();
                hashTable.Add(key, cell);
            }
            if (!cell.Contains(obj))
                cell.Add(obj);
        }
    }

    public void Remove(HashObject obj)
    {
        foreach (string key in Keys(obj.insertPos))
        {
            if (hashTable.Contains(key))
            {
                ArrayList cell = (ArrayList)hashTable[key];
                if (cell.Contains(obj))
                    cell.Remove(obj);
            }
        }
    }

    public void UpdatePosition(HashObject hash)
    {
        if (hash.insertPos != hash.currentTransform.position)
        {
            hash.insertPos = hash.currentTransform.position;
            Remove(hash);
            Insert(hash.currentTransform.position, hash);
        }
    }

    public ArrayList Query(Vector3 pos)
    {
        string key = Key(pos);
        if (hashTable.Contains(key))
            return (ArrayList)hashTable[key];
        return new ArrayList();
    }

    private ArrayList Keys(Vector3 pos, bool is2D = false)
    {
        int o = cellSize / 2;
        ArrayList keys;
        if (is2D)
        {
            keys = new ArrayList
            {
                Key(new Vector3(pos.x - o, pos.y - 0, pos.z - o)),
                Key(new Vector3(pos.x - o, pos.y - 0, pos.z - 0)),
                Key(new Vector3(pos.x - o, pos.y - 0, pos.z + o)),
                Key(new Vector3(pos.x - 0, pos.y - 0, pos.z - o)),
                Key(new Vector3(pos.x - 0, pos.y - 0, pos.z - 0)),
                Key(new Vector3(pos.x - 0, pos.y - 0, pos.z + o)),
                Key(new Vector3(pos.x + o, pos.y - 0, pos.z - o)),
                Key(new Vector3(pos.x + o, pos.y - 0, pos.z - o)),
                Key(new Vector3(pos.x + o, pos.y - 0, pos.z - 0)),
            };
        }
        else
        {
            keys = new ArrayList
            {
                Key(new Vector3(pos.x - o, pos.y - 0, pos.z - o)),
                Key(new Vector3(pos.x - o, pos.y - 0, pos.z - 0)),
                Key(new Vector3(pos.x - o, pos.y - 0, pos.z + o)),
                Key(new Vector3(pos.x - 0, pos.y - 0, pos.z - o)),
                Key(new Vector3(pos.x - 0, pos.y - 0, pos.z - 0)),
                Key(new Vector3(pos.x - 0, pos.y - 0, pos.z + o)),
                Key(new Vector3(pos.x + o, pos.y - 0, pos.z - o)),
                Key(new Vector3(pos.x + o, pos.y - 0, pos.z - o)),
                Key(new Vector3(pos.x + o, pos.y - 0, pos.z - 0)),

                Key(new Vector3(pos.x - o, pos.y - o, pos.z - o)),
                Key(new Vector3(pos.x - o, pos.y - o, pos.z - 0)),
                Key(new Vector3(pos.x - o, pos.y - o, pos.z + o)),
                Key(new Vector3(pos.x - 0, pos.y - o, pos.z - o)),
                Key(new Vector3(pos.x - 0, pos.y - o, pos.z - 0)),
                Key(new Vector3(pos.x - 0, pos.y - o, pos.z + o)),
                Key(new Vector3(pos.x + o, pos.y - o, pos.z - o)),
                Key(new Vector3(pos.x + o, pos.y - o, pos.z - o)),
                Key(new Vector3(pos.x + o, pos.y - o, pos.z - 0)),

                Key(new Vector3(pos.x - o, pos.y + o, pos.z - o)),
                Key(new Vector3(pos.x - o, pos.y + o, pos.z - 0)),
                Key(new Vector3(pos.x - o, pos.y + o, pos.z + o)),
                Key(new Vector3(pos.x - 0, pos.y + o, pos.z - o)),
                Key(new Vector3(pos.x - 0, pos.y + o, pos.z - 0)),
                Key(new Vector3(pos.x - 0, pos.y + o, pos.z + o)),
                Key(new Vector3(pos.x + o, pos.y + o, pos.z - o)),
                Key(new Vector3(pos.x + o, pos.y + o, pos.z - o)),
                Key(new Vector3(pos.x + o, pos.y + o, pos.z - 0))
            };
        }

        return keys;
    }

    private string Key(Vector3 v)
    {
        int x = (int)Mathf.Floor(v.x / cellSize) * cellSize;
        int y = (int)Mathf.Floor(v.y / cellSize) * cellSize;
        int z = (int)Mathf.Floor(v.z / cellSize) * cellSize;
        return x.ToString() + ":" + y.ToString() + ":" + z.ToString();
    }
}