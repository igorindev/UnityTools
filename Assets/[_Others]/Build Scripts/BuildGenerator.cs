// Shady
// Build Generator
// For making iOS and Android builds automatically at default paths outside project directory
// Default Path for iOS build is /Users/${UserName}/Documents/_Builds/${Product Name}/iOS Build
// Default Path for Android build is ${UserName}/Documents/_Builds/${Product Name}/Android Build
// Only active scenes in build settings are included in build
// No need to create any folders all folders will be created automatically
// If iOS build already exists no need to delete, it will Append existing build for iOS
// If APK already exists with the same version it will be deleted and new build will be generated
// If build is completed succesfully respective folders are automatically opened
// Can be accesed from Menu Items
// Shortcut key for Generating iOS Build     => (Ctrl/Cmd) + Shift + I
// Shortcut key for Generating Android Build => (Ctrl/Cmd) + Shift + A
// Shortcut key opening Build Path           => (Ctrl/Cmd) + Shift + O

using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
#endif

public class BuildGenerator
{
    #if UNITY_EDITOR

    private static string GetBuildPath(BuildTarget target)
    {
        string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        //One check only for MacOSX because on MAC the Documents folder is inside the path returned by the above statement
        if(SystemInfo.operatingSystemFamily.Equals(OperatingSystemFamily.MacOSX))
            path += "/Documents";

        if(!Directory.Exists(path + "/_Builds"))
            Directory.CreateDirectory(path + "/_Builds");
        if(!Directory.Exists(path + "/_Builds/" + PlayerSettings.productName))
            Directory.CreateDirectory(path + "/_Builds/" + PlayerSettings.productName);
        path += "/_Builds/" + PlayerSettings.productName;

        switch(target)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneOSX:
            break;
            case BuildTarget.iOS:
                if(!Directory.Exists(path + "/iOS Build"))
                    Directory.CreateDirectory(path + "/iOS Build");
                path += "/iOS Build";
            break;
            case BuildTarget.Android:
                if(!Directory.Exists(path + "/Android Build"))
                    Directory.CreateDirectory(path + "/Android Build");
                path += "/Android Build";
            break;
        }//switch end
        return path;
    }//GetBuildPath() end

    private static string[] GetActiveScenes()
    {
        List<string> scenes = new List<string>();
        foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if(scene.enabled)
                scenes.Add(scene.path);
        }//loop end
        return scenes.ToArray();
    }//GetBuildScenes() end

    private static void OpenInExplorer(string path) => EditorUtility.RevealInFinder(path);

    [MenuItem("Build/Generate Android Build %#a")]
    private static void GenerateAndroidBuild()
    {
        if(UnityEditor.EditorUtility.DisplayDialog("BUILD GENERATOR", "Do you want to Generate Android Build?\n\nWARNING\nIf your project is not on Android platform it will be switched.", "Yes", "No"))
            MakeAndroidBuild();
    }//GenerateAppleBuild() end

    private static void MakeAndroidBuild()
    {
        string buildPath = GetBuildPath(BuildTarget.Android) + ($"/{PlayerSettings.productName} v{PlayerSettings.bundleVersion}({PlayerSettings.Android.bundleVersionCode}).apk");

        //if same version apk exists then delete it
        if(File.Exists(buildPath))
            File.Delete(buildPath);

        PlayerSettings.Android.minSdkVersion  = AndroidSdkVersions.AndroidApiLevel24;
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes             = GetActiveScenes();
        buildPlayerOptions.locationPathName   = buildPath;
        buildPlayerOptions.target             = BuildTarget.Android;
        buildPlayerOptions.options            = BuildOptions.None;

        BuildSummary summary = BuildPipeline.BuildPlayer(buildPlayerOptions).summary;

        if(summary.result == BuildResult.Succeeded)
        {
            OpenInExplorer(buildPath);
            Debug.Log("Build Generated Successfully for Android Platform to path\n" + buildPath);
        }//if end
    }//MakeAndroidBuild() end

    [MenuItem("Build/Generate iOS Build %#i")]
    private static void GenerateAppleBuild()
    {
        if(UnityEditor.EditorUtility.DisplayDialog("BUILD GENERATOR", "Do you want to Generate Apple Build?\n\nWARNING\nIf your project is not currently on iOS Platform it will be switched.", "Yes", "No"))
            MakeiOSBuild();
    }//GenerateAppleBuild() end

    private static void MakeiOSBuild()
    {
        string BuildPath = GetBuildPath(BuildTarget.iOS);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes             = GetActiveScenes();
        buildPlayerOptions.locationPathName   = BuildPath;
        buildPlayerOptions.target             = BuildTarget.iOS;
        //This will append build for iOS if already build exists
        if(File.Exists(Path.Combine(BuildPath, "Info.plist")))
            buildPlayerOptions.options = BuildOptions.AcceptExternalModificationsToPlayer;
        else
            buildPlayerOptions.options = BuildOptions.None;

        BuildReport report   = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if(summary.result == BuildResult.Succeeded)
        {
            OpenBuildPath();
            Debug.Log("Build Generated Successfully for iOS Platform to path\n" + BuildPath);
        }//if end
    }//GenerateAppleBuild() end

    //BuildPath will be opened based on the Platform you project currently is
    [MenuItem("Build/Open Build Path %#o")]
    private static void OpenBuildPath()
    {
        string buildPath = string.Empty;
        switch(EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.iOS:
                buildPath = GetBuildPath(BuildTarget.iOS);
                if(File.Exists(Path.Combine(buildPath, "Info.plist")))
                {
                    OpenInExplorer(buildPath + "/Unity-iPhone.xcodeproj");   //else if xcodeproj found then open to xcodeproj
                    return;
                }//if end
            break;
            case BuildTarget.Android:
                buildPath = GetBuildPath(BuildTarget.Android);
                if(File.Exists(buildPath + ($"/{PlayerSettings.productName} v{PlayerSettings.bundleVersion}({PlayerSettings.Android.bundleVersionCode}).apk")))
                {
                    OpenInExplorer(buildPath + ($"/{PlayerSettings.productName} v{PlayerSettings.bundleVersion}({PlayerSettings.Android.bundleVersionCode}).apk"));
                    return;
                }//if end
            break;
            default:
                buildPath = GetBuildPath(EditorUserBuildSettings.activeBuildTarget);
            break;
        }//switch end
        OpenInExplorer(buildPath); 
    }//OpenBuildPath() end

    #endif

}//class end