using UnityEngine;
using UnityEngine.Events;

namespace CanvasManagement
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(UnityEngine.UI.CanvasScaler))]
    public class UICanvasController : MonoBehaviour
    {
        [Header("Conditions")]
        [SerializeField] bool executeOnStart = false;
        [SerializeField] bool doNotDisable = false;

        [SerializeField] UnityEvent onEnableCanvas;
        [SerializeField] UnityEvent onDisableCanvas;
        Canvas canvas;

        void Start()
        {
            canvas = GetComponent<Canvas>();

            if (executeOnStart)
                EnableCanvas(canvas.enabled);
        }

        public void EnableCanvas(bool value)
        {
            if (!doNotDisable || value)
            {
                canvas.enabled = value;
            }

            if (value)
            {
                if (UICanvasManager.Instance) UICanvasManager.Instance.AddCanvas(this);
                onEnableCanvas?.Invoke();
            }
            else
            {
                onDisableCanvas?.Invoke();
            }
        }
    }
}