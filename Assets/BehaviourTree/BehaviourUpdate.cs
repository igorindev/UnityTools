using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class BehaviourUpdate : MonoBehaviour
{
    [SerializeField] BehaviourTreeRunner[] a;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < a.Length; i++)
        {
            //a[i].CallUpdate();
        }
    }
}
