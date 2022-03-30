using UnityEngine;
using UnityEngine.Events;

public class UICanvasController : MonoBehaviour
{
    [SerializeField] UnityEvent onEnable;
    [SerializeField] UnityEvent onDisable;

    Canvas canvas;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    public void EnableCanvas(bool enable)
    {
        canvas.enabled = enable;

        if (enable)
        {
            onEnable?.Invoke();
        }
        else
        {
            onDisable?.Invoke();
        }
    }
}
