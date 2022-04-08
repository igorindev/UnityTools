using System.Collections.Generic;
using UnityEngine;

namespace CanvasManagement
{
    public class UICanvasManager : Singleton<UICanvasManager>
    {
        Stack<UICanvasController> canvasStack = new Stack<UICanvasController>();

        public UICanvasController CurrentCanvas { get; private set; }

        public void AddCanvas(UICanvasController canvasController)
        {
            if (HasCanvasOpen())
            {
                //Show cursor
                SetCursor(true);
                //Stop player movement
            }
            canvasStack.Push(canvasController);
            CurrentCanvas = canvasController;
        }

        public void CloseCanvas()
        {
            RemoveCanvas();
        }

        UICanvasController RemoveCanvas()
        {
            UICanvasController closed = canvasStack.Pop();
            closed.EnableCanvas(false);

            if (HasCanvasOpen() == false)
            {
                //Hide cursor
                SetCursor(false);
                //Allow player movement
            }

            return closed;
        }
        public UICanvasController CurrentCanvasOnTop()
        {
            return canvasStack.Peek();
        }

        public bool HasCanvasOpen()
        {
            return canvasStack.Count > 0;
        }

        public void SetCursor(bool active)
        {
            Cursor.visible = active;
            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}