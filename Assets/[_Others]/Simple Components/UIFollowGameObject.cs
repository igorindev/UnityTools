using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowGameObject : MonoBehaviour
{
    public Transform target;
    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if(target != null)
        {
        Vector3 screenPos = cam.WorldToScreenPoint(target.position + Vector3.up);
        transform.position = screenPos;
        }
    }
}
