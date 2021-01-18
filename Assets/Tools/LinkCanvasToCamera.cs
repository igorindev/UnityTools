using UnityEngine;

public class LinkCanvasToCamera : MonoBehaviour
{
    public Canvas canvas;
    private void OnEnable()
    {
        if (canvas != null)
        {
            canvas.worldCamera = CameraManager.instance.cameraPlayer;
            canvas.planeDistance = 1;
        }
    }
}
