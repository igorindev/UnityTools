using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlataformSwitcher : IPreprocessBuildWithReport
{

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        ChangeAssetsOnPlataform(EditorUserBuildSettings.activeBuildTarget);

        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes, for target " + report.summary.platform + " at path " + report.summary.outputPath);
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed: Errors (" + summary.totalErrors + ")");
        }
    }

    [MenuItem("Plataform/Switch")]
    public static void ChangeActivePlataform()
    {
        ChangeAssetsOnPlataform(EditorUserBuildSettings.activeBuildTarget);
    }

    public static async void ChangeAssetsOnPlataform(BuildTarget target)
    {
        EditorSceneManager.SaveOpenScenes();
        string[] actives = GetAllOpenScenes();
        string[] scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        for (int i = 0; i < scenes.Length; i++)
        {
            PlataformActiveSwitcher[] plataformActiveSwitchers = GameObject.FindObjectsOfType<PlataformActiveSwitcher>();
            for (int j = 0; j < plataformActiveSwitchers.Length; j++)
            {
                plataformActiveSwitchers[j].Switch(target);
            }

            EditorSceneManager.MarkAllScenesDirty();
            await Task.Delay(100);

            EditorSceneManager.SaveOpenScenes();
        }

        EditorSceneManager.OpenScene(actives[0], OpenSceneMode.Single);
        for (int i = 1; i < actives.Length; i++)
        {
            EditorSceneManager.OpenScene(actives[i], OpenSceneMode.Additive);
        }
    }

    public static string[] GetAllOpenScenes()
    {
        string[] loadedScenes = new string[SceneManager.loadedSceneCount];

        for (int i = 0; i < loadedScenes.Length; i++)
        {
            loadedScenes[i] = EditorSceneManager.GetSceneAt(i).path;
        }

        return loadedScenes;
    }
}
