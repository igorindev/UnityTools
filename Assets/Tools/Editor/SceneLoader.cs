using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneLoader : EditorWindow
{
    [MenuItem("Tools/Scene Loader/Enable")]
    public static void Enable()
    {
        SceneView.duringSceneGui += OnScene;
        Debug.Log("Scene Loader : Enabled");
    }

    [MenuItem("Tools/Scene Loader/Disable")]
    public static void Disable()
    {
        SceneView.duringSceneGui -= OnScene;
        Debug.Log("Scene Loader : Disabled");
    }

    private static void OnScene(SceneView sceneview)
    {
        Handles.BeginGUI();

        GUIStyle style = new GUIStyle("button")
        {
            alignment = TextAnchor.UpperLeft
        };
        if (GUILayout.Button("Scenes", GUILayout.Width(80), GUILayout.Height(25)))
        {
            EditorBuildSettingsScene[] allScenes = EditorBuildSettings.scenes;
            if (allScenes.Length <= 0)
            {
                Debug.LogError("There is no scene in Build Settings");
                return;
            }

            string path;
            
            GenericMenu genericMenu = new GenericMenu();

            for (int i = 0; i < allScenes.Length; i++)
            {
                int id = i;
                path = Path.GetFileNameWithoutExtension(allScenes[i].path);
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                genericMenu.AddItem(new GUIContent(path), false, str => EditorSceneManager.OpenScene((string)str), allScenes[id].path);
            }

            genericMenu.AddSeparator("");

            for (int i = 0; i < allScenes.Length; i++)
            {
                int id = i;
                path = Path.GetFileNameWithoutExtension(allScenes[i].path);
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                genericMenu.AddItem(new GUIContent(path + " - Additive"), false, str => EditorSceneManager.OpenScene((string)str, OpenSceneMode.Additive), allScenes[id].path);
            }

            genericMenu.ShowAsContext();
        }

        Handles.EndGUI();
    }
}
