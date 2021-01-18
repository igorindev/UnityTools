using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookToCamera : MonoBehaviour
{
    public Canvas canvas;
    public Transform target;
    public bool look = true;

    Transform t;
    private void Start()
    {
        if (target == null)
        {
            t = CameraManager.instance.transform;
        }
        else
        {
            t = target;
        }
        if (canvas != null)
        {
            canvas.worldCamera = CameraManager.instance.cameraPlayer;
        }
        if (look == false)
        {
            canvas.planeDistance = 1;
            enabled = false;
        }
    }
    private void LateUpdate()
    {
        transform.LookAt(t);
        transform.Rotate(0, 180, 0);
    }
}
