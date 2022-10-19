using System;
using UnityEngine;
using UnityEngine.Audio;

public static class VideoSettings
{
    public static Resolution[] resolutions;
    public static string[] resolutionsNames;

    public static int currentResolutionIndex;
    static int lastResolutionIndex;

    public static int screenModeIndex;
    static int lastScreenModeIndex;

    public static void Initalize()
    {
        int cache = Screen.resolutions.Length;
        resolutionsNames = new string[cache];
        resolutions = Screen.resolutions; //only store one version
        for (int i = 0; i < cache; i++)
        {
            resolutionsNames[i] = $"{resolutions[i].width}x{resolutions[i].height} @ {resolutions[i].refreshRate} Hz";
        }

        ReverseArray(resolutions);
        ReverseArray(resolutionsNames);

        screenModeIndex = PlayerPrefs.GetInt("ScreenMode", screenModeIndex);
        currentResolutionIndex = PlayerPrefs.GetInt("CurrentResolution", currentResolutionIndex);

        ApplyScreen();
    }
    static T[] ReverseArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (i >= (float)array.Length / 2)
                break;

            (array[i], array[array.Length - 1 - i]) = (array[array.Length - 1 - i], array[i]);
        }
        return array;
    }

    public static void Save()
    {
        PlayerPrefs.SetInt("ScreenMode", screenModeIndex);
        PlayerPrefs.SetInt("CurrentResolution", currentResolutionIndex);
    }

    public static void SetResolution(int value)
    {
        lastResolutionIndex = currentResolutionIndex;
        currentResolutionIndex = value;
    }
    public static void SetScreenMode(int value)
    {
        lastScreenModeIndex = screenModeIndex;
        screenModeIndex = value;
    }

    public static void CancelVideoChanges()
    {
        currentResolutionIndex = lastResolutionIndex;
        screenModeIndex = lastScreenModeIndex;
    }

    public static void ApplyScreen()
    {
        Resolution currentResolution = resolutions[currentResolutionIndex];

        lastResolutionIndex = currentResolutionIndex;
        lastScreenModeIndex = screenModeIndex;
        Screen.SetResolution(currentResolution.width, currentResolution.height, GetScreenMode(), currentResolution.refreshRate);
        Debug.Log("Setted to: " + currentResolutionIndex + ": " + currentResolution + " | " + screenModeIndex + ": " + GetScreenMode());
    }

    public static void SetNewResolutionAndScreenModeIndex(Resolution current, FullScreenMode fullScreenMode)
    {
        int currentNew = 0;
        //for (int i = 0; i < resolutions.Length; i++)
        //{
        //    if (CompareResolutions(current, resolutions[i]))
        //    {
        //        currentNew = i;
        //        break;
        //    }
        //}
        //
        //currentResolutionIndex = currentNew;

        currentNew = fullScreenMode switch
        {
            //Borderless
            FullScreenMode.FullScreenWindow => 1,
            //Windowed
            FullScreenMode.Windowed => 2,
            _ => 0,
        };

        screenModeIndex = currentNew;
        ApplyScreen();
    }

    public static void SetFrameRate(int value)
    {
        if (value < 30) value = 30;
        Application.targetFrameRate = value;
    }
    public static void SetVsync(bool value) => QualitySettings.vSyncCount = value ? 1 : 0;

    public static void SetQuality(int value) => QualitySettings.SetQualityLevel(value);

    public static Resolution GetCurrentSettedResolution() => resolutions[currentResolutionIndex];
    public static int GetCurrentResolutionIndex() => currentResolutionIndex;

    public static FullScreenMode GetScreenMode()
    {
        return screenModeIndex switch
        {
            //Borderless
            1 => FullScreenMode.FullScreenWindow,
            //Windowed
            2 => FullScreenMode.Windowed,
#if UNITY_STANDALONE_WIN
            _ => FullScreenMode.ExclusiveFullScreen,
#else
            _ => FullScreenMode.MaximizedWindow,
#endif
        };
    }
    public static int GetScreenModeIndex(FullScreenMode mode)
    {
        return mode switch
        {
            //Borderless
            FullScreenMode.FullScreenWindow => 1,
            //Windowed
            FullScreenMode.Windowed => 2,
            _ => 0,
        };
    }

    public static int GetFrameRate() => Application.targetFrameRate;
    public static bool GetVsync() => QualitySettings.vSyncCount == 1;
    public static int GetQuality() => QualitySettings.GetQualityLevel();

    public static bool CompareResolutions(Resolution a, Resolution b)
    {
        return a.width == b.width && a.height == b.height && a.refreshRate == b.refreshRate;
    }

    internal static bool HasChanges()
    {
        return lastScreenModeIndex != screenModeIndex || lastResolutionIndex != currentResolutionIndex;
    }
}

public static class AudioSettings
{
    public const string MAIN_VOLUME = "Main";
    public const string MUSIC_VOLUME = "Music";
    public const string EFFECT_VOLUME = "Effect";

    public static AudioMixer activeAudioMixer;

    public static void Initalize(AudioMixer audioMixer)
    {
        //Load values
        activeAudioMixer = audioMixer;

        SetMusicVolume(PlayerPrefs.GetFloat("Music", GetMusicVolume()));
        SetEffectVolume(PlayerPrefs.GetFloat("Effect", GetEffectVolume()));
    }
    public static void Save()
    {
        PlayerPrefs.SetFloat("Music", GetMusicVolume());
        PlayerPrefs.SetFloat("Effect", GetEffectVolume());
    }

    //0.00001 -> 1
    public static void SetMainVolume(float value)
    {
        activeAudioMixer.SetFloat(MAIN_VOLUME, Mathf.Log10(value) * 20);
    }
    public static void SetMusicVolume(float value)
    {
        activeAudioMixer.SetFloat(MUSIC_VOLUME, Mathf.Log10(value) * 20);
    }
    public static void SetEffectVolume(float value)
    {
        activeAudioMixer.SetFloat(EFFECT_VOLUME, Mathf.Log10(value) * 20);
    }
    public static void SetMute(bool value)
    {
        AudioListener.volume = value ? 0 : 1;
    }

    public static float GetMainVolume() { activeAudioMixer.GetFloat(MAIN_VOLUME, out float value); return Mathf.Pow(10, value / 20); }
    public static float GetMusicVolume() { activeAudioMixer.GetFloat(MUSIC_VOLUME, out float value); return Mathf.Pow(10, value / 20); }
    public static float GetEffectVolume() { activeAudioMixer.GetFloat(EFFECT_VOLUME, out float value); return Mathf.Pow(10, value / 20); }
    public static bool GetMute() => AudioListener.volume == 0;
}