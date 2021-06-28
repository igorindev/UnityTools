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
        var gameview = EditorWindow.GetWindow(T);

        if (IsProjectBriwserOpen)
        {
            IsProjectBriwserOpen = false;
            gameview.Close();
        }
        else
        {
            IsProjectBriwserOpen = true;
            gameview.Show();
        }
    }
}
