using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Compilation;

public class FastEnterPlaymode : MonoBehaviour
{
    public static int count = 0;

    [MenuItem("Play Mode/No Domain", true)]
    static bool IsInPlayMode()
    {
        return EditorApplication.isPlaying == false;
    }
    
    [MenuItem("Play Mode/No Domain")]
    static void EnterPlayMode()
    {
        //Subscribe
        EditorApplication.playModeStateChanged += OnExitPlayMode;
        //Save Scene
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSettings.enterPlayModeOptionsEnabled = true;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload;

            EditorApplication.EnterPlaymode();
        }
        else
        {
            EditorApplication.playModeStateChanged -= OnExitPlayMode;
        }

        count++;
        Debug.Log($"<color=cyan>Static Verifier: {count}</color> | The value should always be 1, so domain reload is working.");
    }

    static void OnExitPlayMode(PlayModeStateChange action)
    {
        if (action == PlayModeStateChange.EnteredEditMode)
        {

            EditorSettings.enterPlayModeOptionsEnabled = false;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.None;

            //Unsubscribe
            EditorApplication.playModeStateChanged -= OnExitPlayMode;

            //CompilationPipeline.RequestScriptCompilation();
            EditorUtility.RequestScriptReload();
        }
    }
}
