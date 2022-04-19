using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

[CreateAssetMenu(fileName ="ClearMeshRenderes", menuName = "ScriptableObject/Clear Mesh")]
public class ClearMeshesOnBuild : ScriptableObject, IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    [SerializeField] bool active;
    [SerializeField, Scene] string[] scenes;

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (active == false) { return; }

        for (int i = 0; i < scenes.Length; i++)
        {
            EditorSceneManager.LoadScene(scenes[i]);
            MeshRenderer[] meshRenderers = FindObjectsOfType<MeshRenderer>();
            for (int j = 0; j < meshRenderers.Length; j++)
            {
                meshRenderers[j].enabled = false;
            }
        }
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        if (active == false) { return; }

        for (int i = 0; i < scenes.Length; i++)
        {
            EditorSceneManager.LoadScene(scenes[i]);
            MeshRenderer[] meshRenderers = FindObjectsOfType<MeshRenderer>();
            for (int j = 0; j < meshRenderers.Length; j++)
            {
                meshRenderers[j].enabled = true;
            }
        }
    }
}
