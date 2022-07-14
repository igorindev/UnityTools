using UnityEngine;
using UnityEngine.Events;

namespace CanvasManagement
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(UnityEngine.UI.CanvasScaler))]
    public class UICanvasController : MonoBehaviour
    {
        [Help] public string note = "Mark as HUD if is an Interface canvas";

        [Space]
        public string id;

        [Header("Condition")]
        [SerializeField] bool isHUD = false;

        [Header("Confirm Screen")]
        public UICanvasController extraCanvas = null;

        [Header("Cursor")]
        [SerializeField] bool controlCursorState = true;
        [SerializeField] bool cursorStateOnOpen = true;

        [Space(15)]
        [SerializeField] UnityEvent onEnableCanvas;
        [SerializeField] UnityEvent onDisableCanvas;

        Canvas canvas;

        public bool IsEnabled
        {
            get
            {
                if (canvas)
                    return canvas.enabled;
                else
                    return false;
            }
        }

        void OnValidate()
        {

            if (string.IsNullOrEmpty(id))
                Debug.LogError($"{gameObject.name}'s ID is empty. Please assing an id. Click to go to the GameObject", gameObject);
            else
                gameObject.name = $"Canvas - {id}";
        }

        void Start()
        {
            canvas = GetComponent<Canvas>();

            if (isHUD)
                UICanvas.AddHUD(this);
            else
                UICanvas.AddCanvas(this);
        }
        void OnDestroy()
        {
            if (isHUD)
                UICanvas.RemoveHUD(this);
            else
            {
                UICanvas.RemoveCanvas(this);
                UICanvas.RemoveCanvasFromStack(this);
            }
        }

        internal void EnableCanvas(bool value)
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

            SetCursor();
        }

        internal void SetCursor()
        {
            if (controlCursorState)
            {
                Cursor.visible = cursorStateOnOpen;
                Cursor.lockState = cursorStateOnOpen ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
    }
}