using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class ShortcutProjectBrowserVisualizer
{
    public static bool IsProjectBrowserOpen = false;

    //CTRL + Space
    [Shortcut("Custom Shortcut/Open Project Browser", null, KeyCode.Space, ShortcutModifiers.Action)]
    public static void AddComponentToSelectedGameObject(ShortcutArguments shortcutArguments)
    {
        OpenProjectBrowser();
    }

    public static void OpenProjectBrowser()
    {
        System.Type T = System.Type.GetType("UnityEditor.ProjectBrowser, UnityEditor");
        EditorWindow projectWindow = EditorWindow.GetWindow(T);

        projectWindow.Show();
        if (IsProjectBrowserOpen)
        {
            IsProjectBrowserOpen = false;
            projectWindow.Close();
        }
        else
        {
            System.Type C = System.Type.GetType("UnityEditor.SceneView, UnityEditor");
            EditorWindow sceneWIndow = EditorWindow.GetWindow(C, false, null, false);
            IsProjectBrowserOpen = true;
            projectWindow.position = new Rect(sceneWIndow.position.x, sceneWIndow.position.yMax - projectWindow.position.height - 10, sceneWIndow.position.width - 10, projectWindow.position.height);
        }
    }
}
