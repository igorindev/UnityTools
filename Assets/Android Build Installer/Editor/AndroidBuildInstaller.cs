#if UNITY_EDITOR
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
        string aabLocation = PlayerPrefs.GetString("AAB location"); 
        string adbLocation = PlayerPrefs.GetString("ADB location");
        string bundletoolLocation = PlayerPrefs.GetString("Bundletool");

        if (string.IsNullOrEmpty(aabLocation) || !File.Exists(aabLocation))
            aabLocation = EditorUtility.OpenFilePanel("Find AAB", Environment.CurrentDirectory, "aab");
        if (string.IsNullOrEmpty(aabLocation) || !File.Exists(aabLocation))
        {
            Debug.LogError("Cannot find .aab file.");
            return;
        }
        PlayerPrefs.SetString("AAB location", aabLocation);

        adbLocation = Path.GetFullPath(Path.Combine(EditorApplication.applicationPath, "../../")) + "Editor/Data/PlaybackEngines/AndroidPlayer/SDK";
        adbLocation = adbLocation.Replace('\\', '/');

        if (string.IsNullOrEmpty(aabLocation) || !File.Exists(bundletoolLocation))
            bundletoolLocation = EditorUtility.OpenFilePanel("Find Bundletool", Environment.CurrentDirectory, "jar");
        if (string.IsNullOrEmpty(aabLocation) || !File.Exists(bundletoolLocation))
        {
            Debug.LogError("Cannot find bundletool.jar.");
            return;
        }
        PlayerPrefs.SetString("Bundletool", bundletoolLocation);
        
        string strCmdText = "/C "
                            + "echo Creating apks from aab...&"
                            + $"java -jar {bundletoolLocation} build-apks --bundle=C:/Users/igor/Desktop/dan_hextech419_test.aab --output=C:/Users/igor/Desktop/dan_hextech419_Test.apks&"
                            + "echo Getting the apks size...&"
                            + $"java -jar {bundletoolLocation} get-size total --apks=C:/Users/igor/Desktop/dan_hextech419_Test.apks&"
                            + "echo Setting ANDROID_HOME...&"
                            + $"set ANDROID_HOME={adbLocation}&"
                            + "echo Installing apks on phone...&"
                            + $"java -jar {bundletoolLocation} install-apks --apks=C:/Users/igor/Desktop/dan_hextech419_Test.apks&"
                            + "echo Completed.&"
                            + "set /p num1=";

        Debug.Log(aabLocation);
        Debug.Log(adbLocation);
        Debug.Log(bundletoolLocation);

        ProcessStartInfo info = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = strCmdText,
            WorkingDirectory = Path.GetDirectoryName(adbLocation),
        };

        Process process = Process.Start(info);
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.OutputDataReceived += (sender, e) => { Debug.Log(e.Data); };
        process.ErrorDataReceived += (sender, e) => { Debug.LogError(e.Data); };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
    }

    [MenuItem("Android/Install APK")]
    public static void PushAPKToAndroid()
    {
        string apkLocation = PlayerPrefs.GetString("APK location");
        if (string.IsNullOrEmpty(apkLocation) || !File.Exists(apkLocation))
            apkLocation = EditorUtility.OpenFilePanel("Find APK", Environment.CurrentDirectory, "apk");
        if (string.IsNullOrEmpty(apkLocation) || !File.Exists(apkLocation))
        {
            Debug.LogError("Cannot find .apk file.");
            return;
        }

        PlayerPrefs.SetString("APK location", apkLocation);

        string adbLocation = Path.GetFullPath(Path.Combine(EditorApplication.applicationPath, "../../")) + "Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb.exe";
        adbLocation = adbLocation.Replace('\\', '/');

        Debug.Log(apkLocation);
        Debug.Log(adbLocation);

        ProcessStartInfo info = new ProcessStartInfo
        {
            FileName = adbLocation,
            Arguments = "install -r " + apkLocation,
            WorkingDirectory = Path.GetDirectoryName(adbLocation),
        };

        Process.Start(info);
    }
}
#endif
