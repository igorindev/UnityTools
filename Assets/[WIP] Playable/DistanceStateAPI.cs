using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceStateAPI : MonoBehaviour
{
    public List<DisanceObject> disanceObjects = new List<DisanceObject>();

    public void Register()
    {

    }

    public void Deregister()
    {

    }

    public void Update()
    {
        foreach (DisanceObject item in disanceObjects)
        {
            item.Tick();
        }
    }
}

public class DisanceObject
{
    public void Tick()
    {

    }
}
