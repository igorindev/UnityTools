using System;
using System.Collections.Generic;
using UnityEngine;

public class Recycler : MonoBehaviour
{
    public string[] recyclableTags; 

    public static Dictionary<string, List<GameObject>> recyclables = new Dictionary<string, List<GameObject>>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    void OnValidate()
    {
        recyclables.Clear();

        for (int i = 0; i < recyclableTags.Length; i++)
        {
            recyclables.Add(recyclableTags[i], new List<GameObject>());
        }
    }

    public static void AddToRecycler(string key, GameObject recycledObject)
    {
        recyclables[key].Add(recycledObject);
    }

    public static GameObject Recycle(string key)
    {
        GameObject recycled = null;
        if (recyclables[key].Count > 0)
        {
            recycled = recyclables[key][0];
            recyclables[key].RemoveAt(0);
        }
        else
        {
            //Create
        }

        return recycled;
    }
}
