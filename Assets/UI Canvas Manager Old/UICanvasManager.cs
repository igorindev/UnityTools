using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CanvasManagementOld
{
    public class UICanvasManager : Singleton<UICanvasManager>
    {
        readonly Stack<UICanvasController> canvasHudStack = new Stack<UICanvasController>();
        readonly Stack<UICanvasController> canvasInteractableStack = new Stack<UICanvasController>();

        [field:SerializeField, ReadOnly] public UICanvasController CurrentCanvas { get; private set; }

        [Space(15)]
        [SerializeField] UnityEvent onEnterCanvas;
        [SerializeField] UnityEvent onExitCanvas;

        protected override void Awake()
        {
            base.Awake();

            SetCursor(false);
        }

        public void AddCanvas(UICanvasController canvasController)
        {
            if (canvasInteractableStack.Contains(canvasController)) { return; }

            canvasController.EnableCanvas(true);
            canvasInteractableStack.Push(canvasController);
            if (CurrentCanvas == null)
            {
                onEnterCanvas?.Invoke();
                SetCursor(true);
            }

            CurrentCanvas = canvasController;
        }

        public void CloseCanvas()
        {
            if (HasCanvasOpen()) { return; }

            //Check if there is a popup that not allow close
            if (CurrentCanvas.confirmScreen && CurrentCanvas.confirmScreenActive)
            {
                CurrentCanvas.confirmScreen.EnableCanvas(true);
                CurrentCanvas.confirmScreen.OpenCanvas();
            }
            else
            {
                if (HasCanvasOpen())
                {
                    canvasInteractableStack.Pop().EnableCanvas(false); 
                    if (HasCanvasOpen()) 
                    { 
                        CurrentCanvas = canvasInteractableStack.Peek(); 
                    }
                    else 
                    {
                        SetCursor(false);
                        CurrentCanvas = null;
                        onExitCanvas?.Invoke();
                    }
                }
                else
                {
                    if (CurrentCanvas)
                    {
                        SetCursor(false);
                        CurrentCanvas = null;
                        onExitCanvas?.Invoke();
                    }
                }
            }
        }
        public void CloseCanvasWithConfirmation()
        {
            if (HasCanvasOpen()) { return; }

            canvasInteractableStack.Pop().EnableCanvas(false);
            CurrentCanvas = canvasInteractableStack.Peek();

            if (HasCanvasOpen())
            {
                canvasInteractableStack.Pop().EnableCanvas(false);
                if (HasCanvasOpen())
                {
                    CurrentCanvas = canvasInteractableStack.Peek();
                }
                else
                {
                    SetCursor(false);
                    CurrentCanvas = null;
                    onExitCanvas?.Invoke();
                }
            }
            else
            {
                if (CurrentCanvas)
                {
                    SetCursor(false);
                    CurrentCanvas = null;
                    onExitCanvas?.Invoke();
                }
            }
        }

        public UICanvasController CurrentCanvasOnTop()
        {
            return canvasInteractableStack.Peek();
        }

        public bool HasCanvasOpen()
        {
            return canvasInteractableStack.Count > 0;
        }

        public void SetCursor(bool active)
        {
            Cursor.visible = active;
            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}