using System;
using System.Collections.Generic;
using UnityEngine;

public class Recycler : MonoBehaviour
{
    public GameObject[] recyclableTags; 

    public static Dictionary<string, List<GameObject>> recyclables = new Dictionary<string, List<GameObject>>();

    void OnValidate()
    {
        recyclables.Clear();

        for (int i = 0; i < recyclableTags.Length; i++)
        {
            recyclables.Add(recyclableTags[i].name, new List<GameObject>());
        }
    }

    public static void AddToRecycler(string key, GameObject recycledObject)
    { 
        recyclables[key].Add(recycledObject);
        Debug.Log(recyclables[key].Count);
    }

    public static GameObject Recycle(string key)
    {
        GameObject recycled = null;
        Debug.Log(recyclables[key].Count);
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
