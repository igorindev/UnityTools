// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class Builder : EditorWindow, IPostprocessBuildWithReport
{
    [SerializeField] BuildPlayerOptionsSO buildPlayerOptions;
    [SerializeField] BuildPlayerOptionsInspector[] builds = new BuildPlayerOptionsInspector[0];
    [SerializeField] Vector2 scroll;
    [SerializeField] static int buildsMade;
    [SerializeField] static int buildsCount;
    [SerializeField] static BuildPlayerOptionsInspector[] buildsStatic;

    [MenuItem("Tools/Auto Builder")]
    static void Init()
    {
        Builder window = CreateInstance<Builder>();
        window.titleContent = new GUIContent("Builder", EditorGUIUtility.IconContent("Collab.Build").image);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("");

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty build = so.FindProperty("buildPlayerOptions");
        SerializedProperty stringsProperty = so.FindProperty("builds");
        EditorGUILayout.PropertyField(build, new GUIContent("Add Build Options"), true);

        if (buildPlayerOptions == null) { so.ApplyModifiedProperties(); return; }

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load Builds", GUILayout.Width(100), GUILayout.Height(25)))
            {
                builds = (BuildPlayerOptionsInspector[])buildPlayerOptions.builds.Clone();
                GUI.FocusControl(null);
                so.Update();
            }

            if (GUILayout.Button("Save Builds", GUILayout.Width(100), GUILayout.Height(25)))
            {
                buildPlayerOptions.builds = (BuildPlayerOptionsInspector[])builds.Clone();

                EditorUtility.SetDirty(buildPlayerOptions);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                so.Update();
            }
        }

        using (var scope = new EditorGUILayout.ScrollViewScope(scroll))
        {
            scroll = scope.scrollPosition;
            
            EditorGUILayout.PropertyField(stringsProperty, true);
        }

        GUILayout.FlexibleSpace();
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Create Builds (x" + builds.Length + ")", GUILayout.Width(250), GUILayout.Height(30)))
                {
                    buildsStatic = builds;
                    buildsCount = builds.Length;
                    buildsMade = 0;
                    Build();
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.Space(50);
        }

        so.ApplyModifiedProperties();
    }
    async void Build()
    {
        await Task.Delay(100);
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        buildPlayerOptions.locationPathName = buildsStatic[buildsMade].locationPathName;
        buildPlayerOptions.assetBundleManifestPath = buildsStatic[buildsMade].assetBundleManifestPath;

        buildPlayerOptions.target = buildsStatic[buildsMade].target;
        buildPlayerOptions.targetGroup = buildsStatic[buildsMade].targetGroup;
        buildPlayerOptions.options = buildsStatic[buildsMade].options;

        buildPlayerOptions.scenes = buildsStatic[buildsMade].scenes;
        buildPlayerOptions.extraScriptingDefines = buildsStatic[buildsMade].extraScriptingDefines;

        buildsMade += 1;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
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

    public int callbackOrder { get { return 0; } }
    public void OnPostprocessBuild(BuildReport report)
    {
        Debug.Log("<b>Completed Builds (" + buildsMade + "/" + buildsCount + ")</b>");
        if (buildsMade == buildsCount)
        {
            return;
        }

        WaitForBuildCompletion(report);
    }
    async void WaitForBuildCompletion(BuildReport report)
    {
        while (report.summary.result == BuildResult.Unknown || BuildPipeline.isBuildingPlayer)
        {
            await Task.Delay(1000);
        }

        Build();
    }
}

[Serializable]
public struct BuildPlayerOptionsInspector
{
    public string locationPathName;
    public string assetBundleManifestPath;
    public BuildTargetGroup targetGroup;
    public BuildTarget target;
    public BuildOptions options;
    public string[] scenes;
    public string[] extraScriptingDefines;
}