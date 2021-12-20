using UnityEngine;

public class LinkCanvasToCamera : MonoBehaviour
{
    [SerializeField] Camera target;
    [SerializeField] Canvas canvas;
    [SerializeField] float planeDistance = 0.4f;
    private void OnEnable()
    {
        if (canvas != null)
        {
            canvas.worldCamera = target;
            canvas.planeDistance = planeDistance;
        }
    }
}
