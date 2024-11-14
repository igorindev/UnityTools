using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CanvasSubsystem
{
    public static class UICanvasSubsystem
    {
        private const string DefaultCanvasGroup = "default";

        private static readonly Dictionary<string, ListStack<UICanvas>> canvasGroups = new();

        private static readonly ListStack<UICanvas> openCanvases = new ListStack<UICanvas>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Initialize()
        {
            canvasGroups.Add(DefaultCanvasGroup, new ListStack<UICanvas>());
        }

        public static UICanvas OpenCanvas(UICanvas canvas, string group = DefaultCanvasGroup)
        {
            ListStack<UICanvas> canvasStack = canvasGroups[group];
            if (canvasStack.Contains(canvas))
            {
                return null;
            }

            canvasStack.Push(canvas);
            canvas.EnableCanvas();

            return canvas;
        }

        public static async UniTask<T> OpenCanvas<T>(string canvasId, string group = DefaultCanvasGroup) where T : UICanvas
        {
            UICanvas canvas = await UICanvasProvider.Get(canvasId);

            if (!canvasGroups.TryGetValue(group, out ListStack<UICanvas> canvasStack))
            {
                canvasStack = new ListStack<UICanvas>();
                canvasGroups.Add(group, canvasStack);
            }

            canvasStack.Push(canvas);
            canvas.Setup();
            canvas.EnableCanvas();

            return (T)canvas;
        }

        public static void CloseActiveCanvas(string group = DefaultCanvasGroup)
        {
            if (HasCanvasOpen(group))
            {
                UICanvas currentCanvas = canvasGroups[group].Pop();
                currentCanvas.DisableCanvas();

                if (HasCanvasOpen(group))
                {
                    UICanvas toOpen = canvasGroups[group].Peek();
                    toOpen.EnableCanvas();
                }
            }
        }

        public static void CloseAllCanvas(string group = DefaultCanvasGroup)
        {
            int count = canvasGroups[group].Count;
            for (int i = 0; i < count; i++)
            {
                CloseActiveCanvas(group);
            }
        }

        public static UICanvas CurrentCanvasOnTop(string group = DefaultCanvasGroup)
        {
            return canvasGroups[group].Peek();
        }

        public static bool HasCanvasOpen(string group = DefaultCanvasGroup)
        {
            return canvasGroups[group].Count > 0;
        }
    }
}
