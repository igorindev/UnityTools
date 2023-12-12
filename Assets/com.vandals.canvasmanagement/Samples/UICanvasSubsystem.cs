using System;
using System.Collections.Generic;
using UnityEngine;

namespace CanvasSubsystem
{
    public static class UICanvasSubsystem
    {
        readonly static List<UICanvas> canvasHUD = new List<UICanvas>();
        readonly static ListStack<UICanvas> canvasMenuStack = new ListStack<UICanvas>();

        public static UICanvas CurrentCanvas;

        private static UICanvasDatabase database;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Initialize()
        {
            database = Resources.Load<UICanvasDatabase>("Assets/com.vandals.canvasmanagement/Samples/UICanvasDatabase.asset");
        }

        public static void CloseHUDs()
        {
            for (int i = 0; i < canvasHUD.Count; i++)
            {
                canvasHUD[i].EnableCanvas(false);
            }
        }

        public static void OpenHUDs()
        {
            for (int i = 0; i < canvasHUD.Count; i++)
            {
                canvasHUD[i].EnableCanvas(true);
            }
        }

        public static void CloseAllAndOpenHUDs()
        {
            CloseAllCanvas();

            OpenHUDs();
        }

        //Menus
        public static void OpenCanvas(UICanvas canvasController)
        {
            if (canvasMenuStack.Contains(canvasController)) { return; }

            canvasMenuStack.Push(canvasController);

            if (CurrentCanvas == null)
            {
                CloseHUDs();
            }
            else
            {
                CurrentCanvas.EnableCanvas(false);
            }

            CurrentCanvas = canvasController;
        }

        public static void OpenFreshCanvas(UICanvas canvasController)
        {
            CloseAllCanvas();
            CloseHUDs();

            canvasController.EnableCanvas(true);
            canvasMenuStack.Push(canvasController);

            CurrentCanvas = canvasController;
        }

        public static void RemoveCanvasFromStack(UICanvas canvasController)
        {
            canvasMenuStack.Remove(canvasController);
            if (CurrentCanvas == canvasController)
            {
                CurrentCanvas = null;
            }
        }

        public static void CloseCanvas()
        {
            if (!HasCanvasOpen()) { return; }

            if (HasCanvasOpen())
            {
                UICanvas temp = canvasMenuStack.Pop();
                if (temp)
                    temp.EnableCanvas(false);
                if (HasCanvasOpen())
                {
                    CurrentCanvas = canvasMenuStack.Peek();
                    if (CurrentCanvas)
                        CurrentCanvas.EnableCanvas(true);
                }
                else
                {
                    CurrentCanvas = null;
                    OpenHUDs();
                }
            }
            else
            {
                if (CurrentCanvas)
                {
                    CurrentCanvas = null;
                    OpenHUDs();
                }
            }
        }

        public static void CloseCanvasWithConfirmation()
        {
            if (HasCanvasOpen()) { return; }

            canvasMenuStack.Pop().EnableCanvas(false);
            CurrentCanvas = canvasMenuStack.Peek();

            if (HasCanvasOpen())
            {
                UICanvas temp = canvasMenuStack.Pop();
                if (temp)
                    temp.EnableCanvas(false);
                if (HasCanvasOpen())
                {
                    CurrentCanvas = canvasMenuStack.Peek();
                }
                else
                {
                    CurrentCanvas = null;
                }
            }
            else
            {
                if (CurrentCanvas)
                {
                    CurrentCanvas = null;
                }
            }
        }

        public static void CloseAllCanvas()
        {
            int count = canvasMenuStack.Count;
            for (int i = 0; i < count; i++)
            {
                CloseCanvas();
            }
        }

        public static UICanvas CurrentCanvasOnTop()
        {
            return canvasMenuStack.Peek();
        }

        public static bool HasCanvasOpen()
        {
            return canvasMenuStack.Count > 0;
        }
    }

    public class ListStack<T>
    {
        readonly List<T> items = new List<T>();

        public int Count { get => items.Count; }

        /// <summary>
        /// Add an element to the top of the stack 
        /// </summary>
        public void Push(T item)
        {
            items.Add(item);
        }

        /// <summary>
        /// Remove the element at the top of the stack 
        /// </summary>
        public T Pop()
        {
            if (items.Count > 0)
            {
                T temp = items[items.Count - 1];
                items.RemoveAt(items.Count - 1);
                return temp;
            }
            else
                return default;
        }

        /// <summary>
        /// Return who is at the top of the stack
        /// </summary>
        public T Peek()
        {
            if (items.Count > 0)
            {
                T temp = items[items.Count - 1];
                return temp;
            }
            else
                return default;
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public void RemoveAt(int itemAtPosition)
        {
            items.RemoveAt(itemAtPosition);
        }

        public void Remove(T item)
        {
            items.Remove(item);
        }
    }
}