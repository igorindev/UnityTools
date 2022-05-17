using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleAttributeScript : MonoBehaviour
{
    [Button]
    [SerializeField] bool sa;
    [SerializeField] int a;
    [SerializeField] GameObject adsa;
    void Start()
    {
        
    }

    [Button]
    void Update()
    {
        Debug.Log("Aaa");
    }
}
