using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CanvasSubsystem
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    public class UICanvas : MonoBehaviour
    {
        [Header("Default Buttons")]
        [SerializeField] Button backButton;
        [SerializeField] Button continueButton;

        [Header("Condition")]
        [SerializeField] bool isHud = false;
        [SerializeField] bool openOnStart = false;
        [SerializeField] bool executeOnStart = false;
        [SerializeField] bool showCursor = false;

        [Space(15)]
        [SerializeField] UnityEvent onEnableCanvas;
        [SerializeField] UnityEvent onDisableCanvas;

        Canvas canvas;

        public bool IsEnabled => canvas.enabled;

        void Start()
        {
            canvas = GetComponent<Canvas>();

            if (openOnStart)
            {
                OpenCanvas();
            }
            else if (executeOnStart)
            {
                EnableCanvas(canvas.enabled);
            }
        }

        void OnDestroy()
        {

        }

        [ContextMenu("Open")]
        public void OpenCanvas()
        {
            UICanvasSubsystem.OpenCanvas(this);
            EnableCanvas(true);
        }

        [ContextMenu("Close")]
        public void CloseCurrentCanvas()
        {
            UICanvasSubsystem.CloseCanvas();
        }

        public void CloseAllCanvas()
        {
            UICanvasSubsystem.CloseAllCanvas();
        }

        public void EnableCanvas(bool value)
        {
            canvas.enabled = value;

            if (value)
            {
                SetCursor(true);
                onEnableCanvas?.Invoke();
            }
            else
            {
                SetCursor(false);
                onDisableCanvas?.Invoke();
            }
        }

        public void SetCursor(bool active)
        {
            if (showCursor == false) return;

            Cursor.visible = active;
            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}