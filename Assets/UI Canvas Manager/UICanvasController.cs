using UnityEngine;
using UnityEngine.Events;

namespace CanvasManagement
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(UnityEngine.UI.CanvasScaler))]
    public class UICanvasController : MonoBehaviour
    {
        [Header("Condition")]
        [SerializeField] bool executeOnStart = false;

        [Header("Confirm Screen")]
        public bool confirmScreenActive = true;
        public UICanvasController confirmScreen = null;

        [Space(15)]
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
            canvas.enabled = value;

            if (value)
            {
                onEnableCanvas?.Invoke();
            }
            else
            {
                onDisableCanvas?.Invoke();
            }
        }

        [ContextMenu("Open")]
        public void OpenCanvas()
        {
            if (UICanvasManager.Instance) UICanvasManager.Instance.AddCanvas(this);
        }
        [ContextMenu("Close")]
        public void CloseCanvas()
        {
            if (UICanvasManager.Instance) UICanvasManager.Instance.CloseCanvas();
        }
        [ContextMenu("Close Confirm Screen")]
        public void CloseConfirmScreen()
        {
            if (UICanvasManager.Instance) UICanvasManager.Instance.CloseCanvasWithConfirmation();
        }
    }
}