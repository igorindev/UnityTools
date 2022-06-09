using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AndroidBuildInstaller
{
    [MenuItem("Android/Install AAB")]
    public static void PushAABToAndroid()
    {
        string bundletoolLocation = PlayerPrefs.GetString("Bundletool");
        if (string.IsNullOrEmpty(bundletoolLocation) || !File.Exists(bundletoolLocation))
            bundletoolLocation = EditorUtility.OpenFilePanel("Find Bundletool", Environment.CurrentDirectory, "jar");
        if (string.IsNullOrEmpty(bundletoolLocation) || !File.Exists(bundletoolLocation))
        {
            Debug.LogError("Cannot find bundletool.jar. ");
            return;
        }
        PlayerPrefs.SetString("Bundletool", bundletoolLocation);

        string aabLocation = EditorUtility.OpenFilePanel("Find AAB", Environment.CurrentDirectory, "aab");
        if (string.IsNullOrEmpty(aabLocation) || !File.Exists(aabLocation))
        {
            Debug.LogError("Cannot find .aab file.");
            return;
        }

        string adbLocation = Path.GetFullPath(Path.Combine(EditorApplication.applicationPath, "../../")) + "Editor/Data/PlaybackEngines/AndroidPlayer/SDK";
        adbLocation = adbLocation.Replace('\\', '/');

        string apkLocation = Path.GetFileNameWithoutExtension(aabLocation);
        string aabDirectory = Path.GetDirectoryName(aabLocation);

        string output = aabDirectory + "/" + apkLocation + ".apks";

        Debug.Log("AAB: " + aabLocation);
        Debug.Log("APKs: " + output);
        Debug.Log("ADB: " + adbLocation);
        Debug.Log("Bundletool: " + bundletoolLocation);
        
        string strCmdText = "/C "
                            + "echo Creating apks from aab...&"
                            + @$"java -jar ""{bundletoolLocation}"" build-apks --bundle=""{aabLocation}"" --output=""{output}""&"
                            + "echo Getting the apks size...&"
                            + @$"java -jar ""{bundletoolLocation}"" get-size total --apks=""{output}""&"
                            + "echo Setting ANDROID_HOME...&"
                            + @$"set ANDROID_HOME={adbLocation}&"
                            + "echo Installing apks on phone...&"
                            + @$"java -jar ""{bundletoolLocation}"" install-apks --apks=""{output}""&"
                            + "echo Completed.&"
                            + "set /p num1=";


        ProcessStartInfo info = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = strCmdText,
            WorkingDirectory = Path.GetDirectoryName(adbLocation),
        };

        Process process = Process.Start(info);
    }

    [MenuItem("Android/Install APK")]
    public static void PushAPKToAndroid()
    {
        string adbLocation = Path.GetFullPath(Path.Combine(EditorApplication.applicationPath, "../../")) + "Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/";
        adbLocation = adbLocation.Replace('/', '\\');

        string apkLocation = PlayerPrefs.GetString("APK location");
        if (string.IsNullOrEmpty(apkLocation) || !File.Exists(apkLocation))
            apkLocation = EditorUtility.OpenFilePanel("Find APK", Environment.CurrentDirectory, "apk");
        if (string.IsNullOrEmpty(apkLocation) || !File.Exists(apkLocation))
        {
            Debug.LogError("Cannot find .apk file.");
            return;
        }
        PlayerPrefs.SetString("APK location", apkLocation);


        Debug.Log("APK: " + apkLocation);
        Debug.Log("ADB: " + adbLocation);

        string strCmdText = "/C "
                    + "echo Starting APK installation...&"
                    + "echo Check if the APK has a keystore! Or the installation might fail.&"
                    + @$"cd ""{adbLocation}""&"
                    + $"adb devices&"
                    + @$"adb -d install -r ""{apkLocation}""&"
                    + "echo Completed.&"
                    + "set /p num1=";

        Process p = new Process();

        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.Arguments = strCmdText;
        p.Start();

    }
}