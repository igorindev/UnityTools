using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

public class DynamicGameView : Editor
{
    private static EditorWindow windowInstance;
    private static float scale = 1;
    private static Rect lastPos;
    private static Vector2 lastMousePosition;
    private static Vector2 lastSize;

    static EditorCoroutine editorCoroutine;

    static bool mouseDown;

    [MenuItem("Dynamic Game View/Show Popup")]
    static void ShowWindow()
    {
        System.Reflection.Assembly assembly = typeof(EditorWindow).Assembly;
        Type type = assembly.GetType("UnityEditor.GameView");
        windowInstance = CreateInstance(type) as EditorWindow;
        //windowInstance.position = new Rect();
        windowInstance.ShowUtility();
        lastPos = windowInstance.position;
        editorCoroutine = EditorCoroutineUtility.StartCoroutine(Update(), windowInstance);
    }

    static IEnumerator Update()
    {
        while (windowInstance)
        {
            Point pt = default;
            GetCursorPos(ref pt);
            
            if (GetLeftMousePressed())
            {
                if (mouseDown == false)
                {
                    lastMousePosition = new Vector2(pt.X, pt.Y);

                }

                mouseDown = true; 
            }
            else
            {
                mouseDown = false;
            }

            UpdatePos(new Vector2(pt.X, pt.Y));

            yield return null;

            //UpdateSize();
        }

        EditorCoroutineUtility.StopCoroutine(editorCoroutine);
    }

    static void UpdatePos(Vector2 newMousePos)
    {
        if (windowInstance.hasFocus)
        {
            if (mouseDown)
            {
                Vector2 mouseMovedDistance = newMousePos - lastMousePosition;
                lastMousePosition = newMousePos;

                Vector2 newWindowPos = windowInstance.position.position + mouseMovedDistance;

                lastPos = windowInstance.position = new Rect(newWindowPos, windowInstance.position.size);
            }
        }
    }

    static void UpdateSize()
    {
        Vector2 size = Handles.GetMainGameViewSize();
        Debug.Log(size);
        float originalX = size.x - (size.x * (4f / 100f));
        float originalY = size.y;
        float newX = windowInstance.position.size.x; // The new X value you want to scale to

        // Calculate the scaling factor
        float scaleFactor = (float)newX / originalX;

        // Calculate the scaled Y value
        int newY = (int)(originalY * scaleFactor);

        Vector2 result = new Vector2(newX, newY);

        windowInstance.position = new Rect(lastPos.position, result);
    }

    public static EditorWindow GetMainGameView()
    {
        System.Reflection.Assembly assembly = typeof(EditorWindow).Assembly;
        System.Type type = assembly.GetType("UnityEditor.GameView");
        return EditorWindow.GetWindow(type);
    }

    static Size GetScreenSize()
    {
        return new Size(GetSystemMetrics(0), GetSystemMetrics(1));
    }

    struct Size
    {
        public Size(int w, int h)
        {
            Width = w;
            Height = h;
        }
        public int Height { get; set; }
        public int Width { get; set; }
    }

    [DllImport("user32.dll")]
    static extern bool GetCursorPos(ref Point lpPoint);

    [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    public static extern int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int VirtualKeyPressed);

    public static bool GetLeftMousePressed()
    {
        if (GetAsyncKeyState(0x01) == 0)
            return false;
        else
            return true;
    }
}