
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "VolumeProfile", menuName = "ScriptableObjects/Volume Profile")]
public class PostProcessOnPlatform : ScriptableObject, IProcessSceneWithReport, IPreprocessBuildWithReport
{
    public const string path = @"Assets\Post Processing\Editor\Volume Profile.asset";

    [Header("Default")]
    public VolumeProfile[] defaultVolumes;
    public static VolumeProfile[] staticDefaultVolumes;

    [Serializable]
    public struct VolumesPlatform
    {
        public BuildTarget[] platforms;
        public VolumeProfile[] platformVolumes;
    }
    public VolumesPlatform[] volumesOnPlatforms;
    public static VolumesPlatform[] staticVolumesPlatforms;

    public static int plataform = -1;
    public int callbackOrder => 0;

    static PostProcessOnPlatform postProcessOnPlatform;

    [ContextMenu("Force Validate")]
    void OnValidate()
    {
        Debug.Log("Volume Profile Validated");
        staticDefaultVolumes = defaultVolumes;
        staticVolumesPlatforms = volumesOnPlatforms;
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Preprocess");

        if (postProcessOnPlatform == null)
            postProcessOnPlatform = AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObject)) as PostProcessOnPlatform;

        Debug.Log(postProcessOnPlatform);
        if (postProcessOnPlatform == null)
        {
            if (EditorUtility.DisplayDialog("PostProcess On Platform (Switch the PP based on the build target)", 
                "The PostProcess on Platform was not founded in the search path. You want to keep building without it?", 
                "Continue Build", "Cancel") == false)
            {
                throw new BuildFailedException("Cancelling build in preprocess of the Volume Profiler.");
            }
        }
        
        plataform = -1;
        for (int i = 0; i < staticVolumesPlatforms.Length; i++)
        {
            for (int j = 0; j < staticVolumesPlatforms[i].platforms.Length; j++)
            {
                if (EditorUserBuildSettings.activeBuildTarget == staticVolumesPlatforms[i].platforms[j])
                {
                    plataform = i;
                    break;
                }
            }
        }
        Debug.Log("Plataform: " + plataform);

        
    }

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        if (BuildPipeline.isBuildingPlayer == false) { return; }
        if (plataform == -1) { return; }

        Volume[] v = FindObjectsOfType<Volume>();
        GetPlatformVolume(v);
        Debug.Log($"Updated {v.Length} volumes in scene {scene.path}");
    }

    void GetPlatformVolume(Volume[] item)
    {
        for (int i = 0; i < item.Length; i++)
        {
            item[i].sharedProfile = staticVolumesPlatforms[plataform].platformVolumes[i];
        }
    }

    ////LOGIC TO CHANGE TEMPORARY
    [ContextMenu("Restore Post Process")]
    async void RestorePostProcess()
    {
        while (BuildPipeline.isBuildingPlayer) { await Task.Delay(100); }

        string currentPath = EditorSceneManager.GetActiveScene().path;
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                Scene s = EditorSceneManager.OpenScene(scene.path);
                while (s.isLoaded == false)
                {
                    await Task.Delay(50);
                }
                Volume[] v = FindObjectsOfType<Volume>();
                for (int i = 0; i < v.Length; i++)
                {
                    Debug.Log(v[i]);
                    Debug.Log(v[i].sharedProfile);
                    Debug.Log(defaultVolumes[i]);
                    v[i].sharedProfile = defaultVolumes[i];
                }

                Debug.Log($"Updated {v.Length} scene {scene.path} volumes");

                EditorSceneManager.SaveScene(s);
            }
        }

        Debug.Log("Changed all scenes volumes");

        EditorSceneManager.OpenScene(currentPath);
    }
}
