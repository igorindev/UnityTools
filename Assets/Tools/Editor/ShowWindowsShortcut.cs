using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class ShowWindowsShortcut : Editor
{
    public static bool IsProjectBriwserOpen = false;

    [Shortcut("Custom Shortcut/Open Project Browser", null, KeyCode.Tab)]
    public static void AddComponentToSelectedGameObject(ShortcutArguments shortcutArguments)
    {
        OpenProjectBrowser();
    }

    public static void OpenProjectBrowser()
    {
        System.Type T = System.Type.GetType("UnityEditor.ProjectBrowser, UnityEditor");
        EditorWindow projectWindow = EditorWindow.GetWindow(T);

        if (IsProjectBriwserOpen)
        {
            IsProjectBriwserOpen = false;
            projectWindow.Close();
        }
        else
        {
            System.Type C = System.Type.GetType("UnityEditor.SceneView, UnityEditor");
            EditorWindow sceneWIndow = EditorWindow.GetWindow(C, false, null, false);
            IsProjectBriwserOpen = true;
            projectWindow.position = new Rect(sceneWIndow.position.x, sceneWIndow.position.yMax - projectWindow.position.height - 10, sceneWIndow.position.width - 10, projectWindow.position.height);
        }
    }
}