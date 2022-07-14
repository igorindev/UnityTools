using System.Collections.Generic;
using UnityEngine;

namespace CanvasManagement
{
    public class UICanvas : MonoBehaviour
    {
        public static ListStack<UICanvasController> canvasStack = new ListStack<UICanvasController>();
        public static Dictionary<string, UICanvasController> canvasControllers = new Dictionary<string, UICanvasController>();
        public static List<UICanvasController> canvasHUD = new List<UICanvasController>();

        public static UICanvasController currentOpenCanvas;

        internal static void AddHUD(UICanvasController canvasController)
        {
            canvasHUD.Add(canvasController);
        }
        internal static void RemoveHUD(UICanvasController canvasController)
        {
            canvasHUD.Remove(canvasController);
        }

        internal static void AddCanvas(UICanvasController canvasController)
        {
            if (string.IsNullOrEmpty(canvasController.id))
                Debug.LogError($"{canvasController.name}'s ID is empty. Please assing an id. Click to go to the GameObject", canvasController.gameObject);
            canvasControllers.Add(canvasController.id, canvasController);
        }
        internal static void RemoveCanvas(UICanvasController canvasController)
        {
            canvasControllers.Remove(canvasController.id);
        }

        internal static void RemoveCanvasFromStack(UICanvasController canvasController)
        {
            if (canvasStack.Contains(canvasController))
            {
                canvasStack.Remove(canvasController);
                if (currentOpenCanvas == canvasController)
                {
                    CloseCurrentCanvas();
                }
            }
        }

        //HUDs
        internal static void OpenHUDs()
        {
            for (int i = 0; i < canvasHUD.Count; i++)
                canvasHUD[i].EnableCanvas(true);
        }
        internal static void CloseHUDs()
        {
            for (int i = 0; i < canvasHUD.Count; i++)
                canvasHUD[i].EnableCanvas(false);
        }

        //Menus
        public static void OpenCanvas(string canvasController)
        {
            UICanvasController cc = canvasControllers[canvasController];
            if (canvasStack.Contains(cc)) { return; }
            canvasStack.Push(cc);

            if (currentOpenCanvas && currentOpenCanvas.extraCanvas != cc)
            {
                currentOpenCanvas.EnableCanvas(false);
            }
            else
                CloseHUDs();

            currentOpenCanvas = cc;
            currentOpenCanvas.EnableCanvas(true);
        }
        public static void OpenFreshCanvas(string canvasController)
        {
            UICanvasController cc = canvasControllers[canvasController];

            CloseAllCanvas();

            cc.EnableCanvas(true);
            canvasStack.Push(cc);

            currentOpenCanvas = cc;
            currentOpenCanvas.EnableCanvas(true);
        }
        public static void OpenCanvas(UICanvasController canvasController)
        {
            if (canvasStack.Contains(canvasController)) { return; }

            canvasStack.Push(canvasController);

            if (currentOpenCanvas && currentOpenCanvas.extraCanvas != canvasController)
                currentOpenCanvas.EnableCanvas(false);
            else
                CloseHUDs();

            currentOpenCanvas = canvasController;
            currentOpenCanvas.EnableCanvas(true);
        }
        public static void OpenFreshCanvas(UICanvasController canvasController)
        {
            CloseAllCanvas();

            canvasController.EnableCanvas(true);
            canvasStack.Push(canvasController);

            currentOpenCanvas = canvasController;
            currentOpenCanvas.EnableCanvas(true);
        }

        public static void CloseCurrentCanvas()
        {
            if (currentOpenCanvas)
            {
                if (currentOpenCanvas.extraCanvas)
                {
                    //Usefull for confirmation dialogs
                    OpenCanvas(currentOpenCanvas.extraCanvas);
                    return;
                }

                UICanvasController temp = canvasStack.Pop();
                if (temp) temp.EnableCanvas(false);

                if (HasCanvasInStack())
                {
                    //After close, open the last screen
                    currentOpenCanvas = canvasStack.Peek();
                    if (currentOpenCanvas) currentOpenCanvas.EnableCanvas(true);
                }
                else
                {
                    currentOpenCanvas = null;
                    OpenHUDs();
                }
            }
        }
        public static void CloseAllCanvas()
        {
            int cache = canvasStack.Count;
            for (int i = 0; i < cache; i++)
            {
                canvasStack[i].EnableCanvas(false);
            }
            canvasStack.Clear();
            currentOpenCanvas = null;
        }

        public static bool HasCanvasInStack() => canvasStack.Count > 0;
    }
}

public class ListStack<T>
{
    readonly List<T> items = new List<T>();

    public int Count { get => items.Count; }

    public T this[int index] => items[index];

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
    public void Clear()
    {
        items.Clear();
    }
}