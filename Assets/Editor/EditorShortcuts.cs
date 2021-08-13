using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class EditorShortcuts : Editor
{
    public static bool IsProjectBriwserOpen = false;

    [Shortcut("Custom Shortcut/Open Project Browser", null, KeyCode.Space, ShortcutModifiers.Action)]
    public static void AddComponentToSelectedGameObject(ShortcutArguments shortcutArguments)
    {
        OpenProjectBrowser();
    }

    // Toggle the active value of the selected GameObjects using CTRL + W
    [MenuItem("Tools/Toggle Object %w")]
    public static void ToggleObjects()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
            Selection.gameObjects[i].SetActive(!Selection.gameObjects[i].activeSelf);
    }

    public static void OpenProjectBrowser()
    {
        System.Type T = System.Type.GetType("UnityEditor.ProjectBrowser, UnityEditor");
        EditorWindow projectWindow = EditorWindow.GetWindow(T);

        projectWindow.Show();
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