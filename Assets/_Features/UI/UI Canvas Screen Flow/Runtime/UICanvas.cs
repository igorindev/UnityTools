using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CanvasSubsystem
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    public class UICanvas : MonoBehaviour
    {
        [Header("Condition")]
        [SerializeField] bool _useExitConfirmation = false;
        [SerializeField] bool _showCursor = false;

        [Space(15)]
        [SerializeField] UnityEvent _onEnableCanvas;
        [SerializeField] UnityEvent _onDisableCanvas;

        [Space(15)]
        [SerializeField] Button _closeButton;
        [SerializeField] UICanvasExitConfirmation _closeCanvasConfirmation;

        private Canvas _canvas;

        public bool IsEnabled => _canvas.enabled;

        internal void Setup()
        {
            _canvas = GetComponent<Canvas>();
            _closeButton.onClick.AddListener(CloseCanvas);
            _closeCanvasConfirmation.Setup(Close, null, "True", "Yes", "No");
        }

        public void CloseCanvas()
        {
            if (_useExitConfirmation)
            {
                _closeCanvasConfirmation.Show();
                return;
            }

            Close(true);
        }

        internal void EnableCanvas()
        {
            SetCanvasEnable(true);
        }

        internal void DisableCanvas()
        {
            SetCanvasEnable(false);
        }

        private void SetCanvasEnable(bool value)
        {
            _canvas.enabled = value;

            if (value)
            {
                SetCursor(true);
                _onEnableCanvas?.Invoke();
            }
            else
            {
                SetCursor(false);
                _onDisableCanvas?.Invoke();
            }
        }

        private void SetCursor(bool active)
        {
            if (_showCursor == false)
            {
                return;
            }

            Cursor.visible = active;
            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
        }

        private void Close(bool value)
        {
            if (value)
            {
                UICanvasSubsystem.CloseActiveCanvas();
            }
        }
    }
}
