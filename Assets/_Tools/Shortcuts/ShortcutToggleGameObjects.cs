using UnityEditor;

public class ShortcutToggleGameObjects
{
    //CTRL + W
    [MenuItem("Tools/Toggle Object %w")]
    public static void ToggleObjects()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            Selection.gameObjects[i].SetActive(!Selection.gameObjects[i].activeSelf);
        }
    }
}
