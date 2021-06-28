using UnityEngine;

public class LookToCamera : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Transform target;
    [SerializeField] Camera cameraTarget;
    [SerializeField] bool look = true;

    Transform t;
    void Start()
    {
        if (target == null)
        {
            t = cameraTarget.transform;
        }
        else
        {
            t = target;
        }
        if (canvas != null)
        {
            canvas.worldCamera = cameraTarget;
        }
        if (look == false)
        {
            canvas.planeDistance = 1;
            enabled = false;
        }
    }
    void LateUpdate()
    {
        transform.LookAt(t);
        transform.Rotate(0, 180, 0);
    }
}
