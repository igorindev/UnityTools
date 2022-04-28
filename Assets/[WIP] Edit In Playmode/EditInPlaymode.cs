using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;


public class EditInPlaymode
{
    static Scene[] openScenes;
    static GameObject[][] gameObjects;

    [InitializeOnLoadMethod]
    static void Init()
    {
        EditorApplication.playModeStateChanged += ChangePlayState;
    }

    static void ChangePlayState(PlayModeStateChange playModeStateChange)
    {
        switch (playModeStateChange)
        {
            case PlayModeStateChange.EnteredPlayMode:
                break;
            case PlayModeStateChange.ExitingPlayMode:
                openScenes = GetAllOpenScenes();
                gameObjects = new GameObject[openScenes.Length][];
                for (int i = 0; i < openScenes.Length; i++)
                {
                    gameObjects[i] = openScenes[i].GetRootGameObjects();
                }
                break;
            case PlayModeStateChange.EnteredEditMode:
                for (int i = 0; i < openScenes.Length; i++)
                {
                    EditorSceneManager.LoadScene(openScenes[i].name);
                    for (int j = 0; j < gameObjects.Length; j++)
                    {
                        Object.Instantiate(gameObjects[i][j]);
                        Object.DestroyImmediate(gameObjects[i][j]);
                    }
                }
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
        }
    }

    static Scene[] GetAllOpenScenes()
    {
        int countLoaded = EditorSceneManager.sceneCount;
        Scene[] loadedScenes = new Scene[countLoaded];

        for (int i = 0; i < countLoaded; i++)
        {
            loadedScenes[i] = EditorSceneManager.GetSceneAt(i);
        }

        return loadedScenes;
    }
}
