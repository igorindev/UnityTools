using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;


//TODO
// - Individual versions
// - Play individual build

public class Builder : EditorWindow, IPostprocessBuildWithReport
{
    [SerializeField] string commonPath = "C:/Projects/ElvantisDev/Builds/";
    [SerializeField] string currentVersion = "v0.0.0.0";
    [SerializeField] bool updateMajorVersion = false;
    [SerializeField] bool updateMinorVersion = false;
    [SerializeField] bool updatePatchVersion = false;
    [SerializeField] bool updateTestVersion = true;
    [SerializeField] string extraInfo = "";

    [SerializeField] string buildVersion;
    [SerializeField] bool updateVersionOnBuild;
    [SerializeField] bool individualVersions;

    [SerializeField] BuilderPresetsSO buildPlayerOptionsSO;
    [SerializeField] BuildPlayerOptionsInspector[] builds = new BuildPlayerOptionsInspector[0];
    [SerializeField] Vector2 scroll;
    [SerializeField] Vector2 scrollHistory;

    SerializedObject so;
    SerializedProperty build;
    SerializedProperty stringsProperty;
    SerializedProperty commonPathProperty;
    SerializedProperty updateMajorVersionProperty;
    SerializedProperty updateMinorVersionProperty;
    SerializedProperty updatePatchVersionProperty;
    SerializedProperty updateTestVersionProperty;
    SerializedProperty extraInfoProperty;

    static bool updateMajorVersionStatic;
    static bool updateMinorVersionStatic;
    static bool updatePatchVersionStatic;
    static bool updateTestVersionStatic;

    static string commonPathStatic;

    static BuildPlayerOptionsInspector[] buildsStatic;
    static BuildTargetGroup currentBuildTargetGroup = BuildTargetGroup.Standalone;
    static BuildTarget currentBuildTarget = BuildTarget.StandaloneWindows;

    static int buildsMade;
    static int buildsCount;

    const string initialVersion = "0.0.0.0";

    static string scriptableObjectPath;
    static string buildName;

    static string backupVersion;

    bool oldSettings;

    GUIStyle iconButtonStyle;

    const string MissingWindowsModuleError = "Please install the build module via UnityHub first. See: https://docs.unity3d.com/Manual/GettingStartedAddingEditorComponents.html";

    [MenuItem("Tools/Builder")]
    static void Init()
    {
        Builder window = CreateInstance<Builder>();
        window.titleContent = new GUIContent("Builder", EditorGUIUtility.Load("d_Package Manager") as Texture2D);
        window.Show();
    }

    void OnFocus()
    {
        currentVersion = PlayerSettings.bundleVersion;
        updateMajorVersionStatic = updateMajorVersion;
        updateMinorVersionStatic = updateMinorVersion;
        updatePatchVersionStatic = updatePatchVersion;
        updateTestVersionStatic = updateTestVersion;
    }

    void OnValidate()
    {
        currentVersion = PlayerSettings.bundleVersion;
        updateMajorVersionStatic = updateMajorVersion;
        updateMinorVersionStatic = updateMinorVersion;
        updatePatchVersionStatic = updatePatchVersion;
        updateTestVersionStatic = updateTestVersion;
    }

    void OnEnable()
    {
        currentVersion = PlayerSettings.bundleVersion;

        so = new SerializedObject(this);
        build = so.FindProperty(nameof(buildPlayerOptionsSO));
        stringsProperty = so.FindProperty(nameof(builds));

        updateMajorVersionProperty = so.FindProperty(nameof(updateMajorVersion));
        updateMinorVersionProperty = so.FindProperty(nameof(updateMinorVersion));
        updatePatchVersionProperty = so.FindProperty(nameof(updatePatchVersion));
        updateTestVersionProperty = so.FindProperty(nameof(updateTestVersion));
        extraInfoProperty = so.FindProperty(nameof(extraInfo));

        commonPathProperty = so.FindProperty(nameof(commonPath));

        so.ApplyModifiedProperties();
    }

    void LoadPreset()
    {
        builds = (BuildPlayerOptionsInspector[])buildPlayerOptionsSO.builds.Clone();
        GUI.FocusControl(null);
        so.ApplyModifiedProperties();
        so.Update();
    }
    void CreatePreset()
    {
        BuilderPresetsSO preset = CreateInstance<BuilderPresetsSO>();
        // path has to start at "Assets"
        string path = "Assets/Tools/Builder/Editor/Presets/BuilderPresetsSO.asset";
        string uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(preset, uniquePath);
        Selection.activeObject = preset;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();

        buildPlayerOptionsSO = preset;
        buildPlayerOptionsSO.builds = (BuildPlayerOptionsInspector[])builds.Clone();

        EditorUtility.SetDirty(buildPlayerOptionsSO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        so.Update();

        LoadPreset();
    }

    void KillProcessOfRunningGames(string name)
    {
        string[] v = name.Split('/');
        string filePathWithoutExt = Path.ChangeExtension(v[v.Length - 1], null);
        Process[] processes = Process.GetProcessesByName(filePathWithoutExt);
        foreach (var process in processes)
        {
            process.Kill();
        }
    }

    void OnGUI()
    {
        so.Update();
        GUILayout.Label("");

        using (new EditorGUI.ChangeCheckScope())
        {
            EditorGUILayout.PropertyField(build, new GUIContent("Add Build Preset"));
            if (buildPlayerOptionsSO == null)
            {
                if (GUILayout.Button("Create Preset", GUILayout.Width(100), GUILayout.Height(25)))
                {
                    CreatePreset();
                }
                so.ApplyModifiedProperties();
                return;
            }
            if (oldSettings)
            {
                oldSettings = false;
                LoadPreset();
            }
        }

        if (GUI.changed)
        {
            oldSettings = true;
            LoadPreset();
        }

        scriptableObjectPath = AssetDatabase.GetAssetPath(buildPlayerOptionsSO);
        so.ApplyModifiedProperties();

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load Preset", GUILayout.Width(100), GUILayout.Height(25)))
            {
                LoadPreset();
            }
            if (GUILayout.Button("Create Preset", GUILayout.Width(100), GUILayout.Height(25)))
            {
                CreatePreset();
            }

            GUI.color = Color.yellow;
            if (GUILayout.Button("Save Preset", GUILayout.Width(100), GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("Overwrite Builder Preset",
                                                "Are you sure you want to Overwrite the current build preset?",
                                                "Overwrite",
                                                "Cancel"))
                {
                    buildPlayerOptionsSO.builds = (BuildPlayerOptionsInspector[])builds.Clone();

                    EditorUtility.SetDirty(buildPlayerOptionsSO);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    so.Update();
                }
            }
            GUI.color = Color.white;
        }

        GUILayout.Space(15);

        EditorGUILayout.LabelField("Build Path", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(commonPathProperty);
        commonPathStatic = commonPathProperty.stringValue;

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Build Version Config", EditorStyles.boldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = false;
            EditorGUILayout.TextField("Build Version", PlayerSettings.bundleVersion);
            GUI.enabled = true;
            GUIContent content = new GUIContent(EditorGUIUtility.Load("icons/d__Popup.png") as Texture2D);
            if (iconButtonStyle == null)
            {
                iconButtonStyle = new GUIStyle(GUI.skin.FindStyle("Button")) ?? new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("Button"));
            }
            iconButtonStyle.alignment = TextAnchor.MiddleCenter;
            iconButtonStyle.imagePosition = ImagePosition.ImageAbove;
            iconButtonStyle.padding = new RectOffset(0, 0, 0, 0);

            if (GUILayout.Button(content, GUILayout.Width(40), GUILayout.Height(18)))
            {
                SettingsService.OpenProjectSettings("Project/Player");
            }
        }
        GUILayout.Space(5);
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Update Version", GUILayout.Width(149));

            updateMajorVersionProperty.boolValue = GUILayout.Toggle(updateMajorVersionProperty.boolValue, "Major");
            updateMajorVersionStatic = updateMajorVersionProperty.boolValue;
            GUILayout.Space(10);
            updateMinorVersionProperty.boolValue = GUILayout.Toggle(updateMinorVersionProperty.boolValue, "Minor");
            updateMinorVersionStatic = updateMinorVersionProperty.boolValue;
            GUILayout.Space(10);
            updatePatchVersionProperty.boolValue = GUILayout.Toggle(updatePatchVersionProperty.boolValue, "Patch");
            updatePatchVersionStatic = updatePatchVersionProperty.boolValue;
            GUILayout.Space(10);
            updateTestVersionProperty.boolValue = GUILayout.Toggle(updateTestVersionProperty.boolValue, "Test");
            updateTestVersionStatic = updateTestVersionProperty.boolValue;
            GUILayout.Space(10);
            GUILayout.FlexibleSpace();
        }

        GUILayout.Space(15);
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Builds Configuration", EditorStyles.boldLabel);
            using (var scope = new EditorGUILayout.ScrollViewScope(scroll))
            {
                scroll = scope.scrollPosition;

                EditorGUILayout.PropertyField(stringsProperty, true);
            }
        }
        GUILayout.Space(2);
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Height(123)))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Builds History", EditorStyles.boldLabel);
                if (GUILayout.Button("Clear", GUILayout.Width(50), GUILayout.Height(18)))
                {
                    buildPlayerOptionsSO.history.Clear();
                }

                if (GUILayout.Button("Debug", GUILayout.Width(50), GUILayout.Height(18)))
                {
                    buildPlayerOptionsSO.history.Add("AAA");
                    for (int i = 0; i < buildPlayerOptionsSO.history.Count; i++)
                    {
                        Debug.Log(buildPlayerOptionsSO.history[i]);
                    }
                }
            }

            using (var scope = new EditorGUILayout.ScrollViewScope(scrollHistory))
            {
                scrollHistory = scope.scrollPosition;

                if (buildPlayerOptionsSO)
                {
                    for (int i = buildPlayerOptionsSO.history.Count - 1; i >= 0; i--)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Space(15);
                            if (i == buildPlayerOptionsSO.history.Count - 1) { EditorGUILayout.LabelField("Last", GUILayout.Width(50)); } else { EditorGUILayout.LabelField((i).ToString(), GUILayout.Width(50)); }
                            EditorGUILayout.LabelField(buildPlayerOptionsSO.history[i]);
                        }
                    }
                }
            }
        }
        GUILayout.Space(2);

        bool callBuild = false;
        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUI.color = Color.green;
                if (GUILayout.Button("Create Builds (x" + builds.Length + ")", GUILayout.Height(30)))
                {
                    buildsStatic = builds;
                    buildsCount = builds.Length;
                    buildsMade = 0;
                    GUI.FocusControl(null);

                    currentBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                    currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
                    backupVersion = PlayerSettings.bundleVersion;

                    onBuildEnd = null;
                    onBuildEnd += OnBuildFail;
                    callBuild = true;
                }
                GUI.color = Color.white;
            }

            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox, GUILayout.MaxWidth(200)))
            {
                if (GUILayout.Button("Run All Builds", GUILayout.Height(30), GUILayout.Width(100)))
                {
                    for (int i = 0; i < builds.Length; i++)
                    {
                        string path = commonPath + builds[i].locationPathName;
                        if (File.Exists(path))
                        {
                            Process.Start(commonPath + builds[i].locationPathName);
                        }
                        else
                        {
                            Debug.LogWarning($"<color=#EE2121><b>Launch Failed:</b></color> Build at path {path} do not exist.");
                        }
                    }
                }
                if (GUILayout.Button("Close All Builds", GUILayout.Height(30), GUILayout.Width(100)))
                {
                    for (int i = 0; i < builds.Length; i++)
                    {
                        KillProcessOfRunningGames(builds[i].locationPathName);
                    }
                }
            }
        }

        so.ApplyModifiedProperties();

        if (callBuild)
            Build();
    }
    async void Build()
    {
        Debug.Log("<b><color=cyan>NEW BUILD STARTED</color></b>");
        if (buildsMade != 0)
        {
            await Task.Delay(100);
        }

        UpdateVersion(FindCurrentVersion());

        string name = buildsStatic[buildsMade].locationPathName;
        if (buildsStatic[buildsMade].buildVersion.addVersionToName)
        {
            string[] n = buildsStatic[buildsMade].locationPathName.Split('.');
            name = n[0] + " " + PlayerSettings.bundleVersion;
            name = name.Replace('.', '-');
            name = name.Replace(':', '-');
            name = name.Replace('<', '(');
            name = name.Replace('>', ')');
            name += "." + n[1];
        }

        KillProcessOfRunningGames(name);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            locationPathName = commonPathStatic + name,
            assetBundleManifestPath = buildsStatic[buildsMade].assetBundleManifestPath,

            target = buildsStatic[buildsMade].target,
            targetGroup = buildsStatic[buildsMade].targetGroup,

#if UNITY_2021_2_OR_NEWER
            subtarget = (int)buildsStatic[buildsMade].subtarget,
#endif
            options = buildsStatic[buildsMade].options,

            scenes = buildsStatic[buildsMade].scenes,
            extraScriptingDefines = buildsStatic[buildsMade].defines
        };
        BuildTargetGroup tg = buildsStatic[buildsMade].targetGroup;
        ScriptingImplementation backup = PlayerSettings.GetScriptingBackend(buildPlayerOptions.targetGroup);
        ScriptingImplementation toUse = buildsStatic[buildsMade].scriptingBackend == BuildPlayerOptionsInspector.SCRIPTING_BACKEND.Mono ? ScriptingImplementation.Mono2x : ScriptingImplementation.IL2CPP;
        PlayerSettings.SetScriptingBackend(tg, toUse);

        buildsMade += 1;

        SwitchPlataform(buildPlayerOptions.targetGroup, buildPlayerOptions.target); 

        BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

        onBuildEnd?.Invoke(buildReport);
    }
    async void WaitForBuildCompletion(BuildReport report)
    {
        while (report.summary.result == BuildResult.Unknown || BuildPipeline.isBuildingPlayer)
        {
            await Task.Delay(1000);
        }

        Build();
    }

    string FindCurrentVersion()
    {
        string[] currentVersion = PlayerSettings.bundleVersion.Split('v', ' ');
        return currentVersion.Length == 1 ? initialVersion : currentVersion[1];
    }
    void UpdateVersion(string version)
    {
        if (!updateMajorVersionStatic && !updateMinorVersionStatic && !updatePatchVersionStatic && !updateTestVersionStatic) return;

        string[] split = version.Split('.');
        if (split.Length == 4 && int.TryParse(split[0], out int major) && int.TryParse(split[1], out int minor) && int.TryParse(split[2], out int patch) && int.TryParse(split[3], out int test))
        {
            int majorVersion = major + (updateMajorVersionStatic && buildsMade == 0 ? 1 : 0);
            int minorVersion = minor + (updateMinorVersionStatic && buildsMade == 0 ? 1 : 0);
            int patchVersion = patch + (updatePatchVersionStatic && buildsMade == 0 ? 1 : 0);
            int testVersion = test + (updateTestVersionStatic && buildsMade == 0 ? 1 : 0);

            string dateTime = "";

            if (buildsStatic[buildsMade].buildVersion.addDate && buildsStatic[buildsMade].buildVersion.addHour)
            {
                dateTime = $" ({DateTime.Now:MMM dd yyyy} {DateTime.Now:hh:mm:ss tt})";
            }
            else if (buildsStatic[buildsMade].buildVersion.addDate)
            {
                dateTime = ($" ({DateTime.Now:MMM dd yyyy})");
            }
            else if (buildsStatic[buildsMade].buildVersion.addHour)
            {
                dateTime = ($" ({DateTime.Now:hh:mm:ss tt})");
            }

            PlayerSettings.bundleVersion = $"v{majorVersion}.{minorVersion}.{patchVersion}.{testVersion}"
                                            + dateTime
                                            + (buildsStatic[buildsMade].buildVersion.addExtra.Length > 0 ? $" ({buildsStatic[buildsMade].buildVersion.addExtra})" : "");
        }
    }

    public static Action<BuildReport> onBuildEnd;
    public int callbackOrder { get { return 0; } }
    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"<b><color=green>BUILD COMPLETED</color></b>: <b>{buildsMade}</b> of <b>{buildsCount}</b> ({Application.version})");
            Debug.Log("Build size: " + (report.summary.totalSize / 1e+6) + " MB, for target " + report.summary.platform + " at path " + report.summary.outputPath);
            if (buildPlayerOptionsSO == null)
            {
                buildPlayerOptionsSO = AssetDatabase.LoadAssetAtPath<BuilderPresetsSO>(scriptableObjectPath);
            }
            buildPlayerOptionsSO.history.Add(name + " " + EditorUserBuildSettings.activeBuildTarget + " " + Application.version);
            currentVersion = PlayerSettings.bundleVersion;
            Debug.Log("c: " + currentVersion + " | P: " + PlayerSettings.bundleVersion);
            EditorUtility.SetDirty(buildPlayerOptionsSO);
            AssetDatabase.SaveAssets();
        }

        //FINISH
        if (buildsMade == buildsCount)
        {
            Debug.Log($"<b><color=cyan>ALL BUILDS COMPLETED</color></b>: <b>{buildsMade}</b> of <b>{buildsCount}</b> ({Application.version})");

            SwitchPlataform(currentBuildTargetGroup, currentBuildTarget);
            return;
        }

        WaitForBuildCompletion(report);
    }

    void OnBuildFail(BuildReport report)
    {
        if (report.summary.result == BuildResult.Failed)
        {
            Debug.Log($"<b><color=red>BUILD {buildsMade} FAILED</color></b>: <b>{buildsMade - 1}</b> of <b>{buildsCount}</b> completed.");
            PlayerSettings.bundleVersion = backupVersion;
        }
        else if (report.summary.result == BuildResult.Cancelled)
        {
            Debug.Log($"<b><color=magenta>BUILD {buildsMade} CANCELED</color></b>: <b>{buildsMade - 1}</b> of <b>{buildsCount}</b> completed.");
            PlayerSettings.bundleVersion = backupVersion;
        }
        else if (report.summary.result == BuildResult.Unknown)
        {
            Debug.Log($"<b><color=red>BUILD {buildsMade} FAILED</color></b>: <b>{buildsMade - 1}</b> of <b>{buildsCount}</b> completed.");
            PlayerSettings.bundleVersion = backupVersion;
        }

        SwitchPlataform(currentBuildTargetGroup, currentBuildTarget);
    }
    static void SwitchPlataform(BuildTargetGroup group, BuildTarget target)
    {
        if (!BuildPipeline.IsBuildTargetSupported(group, target))
        {
            Debug.LogError(MissingWindowsModuleError);
            return;
        }

        if (target != EditorUserBuildSettings.activeBuildTarget || group != EditorUserBuildSettings.selectedBuildTargetGroup)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(group, target);
            Debug.Log($"Build has been successfully configured for {target}.");
        }
        else
        {
            Debug.Log($"Build was already configured for {target}.");
        }

    }
}

[Serializable]
public struct BuildPlayerOptionsInspector
{
    public string locationPathName;
    public BuildVersionOptions buildVersion;
    public string assetBundleManifestPath;
    public BuildTargetGroup targetGroup;
    public BuildTarget target;

#if UNITY_2021_2_OR_NEWER
    public StandaloneBuildSubtarget subtarget;
#endif

    public BuildOptions options;
    public SCRIPTING_BACKEND scriptingBackend;


    [Scene] public string[] scenes;
    public string[] defines;
    
    [Serializable]
    public struct BuildVersionOptions
    {
       public bool addDate;
       public bool addHour;
       public string addExtra;
       public bool addVersionToName;
    }
    [Serializable]
    public enum SCRIPTING_BACKEND
    {
        Mono, IL2CPP
    }
}