using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class VideoSettings
{
    public enum QualityLevel
    {
        Low = 0,
        Medium,
        High,
        Ultra
    }

    /// <summary>
    /// More Lower the value more stable FPS and more Higher the value, faster the response (but some frames get discarted). 
    /// </summary>
    public enum VSync
    {
        DontSync = 0,
        DoubleBuffer,
        TrippleBuffer,
        QuadrupleBuffer,
        QuintupleBuffer,
    }

    private struct ScreenMode
    {
        public string name;
        public FullScreenMode fullScreenMode;

        public ScreenMode(string name, FullScreenMode fullScreenMode)
        {
            this.name = name;
            this.fullScreenMode = fullScreenMode;
        }
    }

    private const string ScreenModeKey = "ScreenMode";
    private const string CurrentResolutionKey = "CurrentResolution";

    private static List<Resolution> resolutions;
    private static List<string> resolutionsNames;

    private static List<FullScreenMode> availableScreenModes;

    private static List<RefreshRate> availableRefreshRates;
    private static List<string> availableRefreshRatesNames;

    private static List<DisplayInfo> availableDisplayInfos = new();

    private static VideoSettingsSaveData videoSettingsSave;
    private static VideoSettingsData currentVideoSettingsSaveData;
    private static VideoSettingsData tempVideoSettingsSaveData;

    public static event Action<IReadOnlyList<Resolution>> OnResolutionListUpdated;

    public static IReadOnlyList<string> GetCurrentResolutionsNames() => resolutionsNames;
    public static IReadOnlyList<string> GetCurrentRefreshRateNames() => availableRefreshRatesNames;
    public static IReadOnlyList<DisplayInfo> GetCurrentDisplayOptions() => availableDisplayInfos;

    public static void SetupScreenSettings()
    {
        videoSettingsSave = new();

        videoSettingsSave.Load(out VideoSettingsData loadedVideoSettings);

        SetupScreenModes(loadedVideoSettings.screenMode);

        SetupResolutions();

        SetupDisplayOptions(loadedVideoSettings.displayWindowIndex);

        ValidateResolution(loadedVideoSettings.resolutionWidth,
            loadedVideoSettings.resolutionHeight,
            loadedVideoSettings.resolutionRefreshRateNumerator,
            loadedVideoSettings.resolutionRefreshRateDenominator);

        tempVideoSettingsSaveData = currentVideoSettingsSaveData = loadedVideoSettings;

        //TODO: Save only if changes happened
        videoSettingsSave.Save(currentVideoSettingsSaveData);

        //ApplyScreen();
    }

    private static void SetupScreenModes(int screenMode)
    {
        if ((FullScreenMode)screenMode != Screen.fullScreenMode)
        {
            currentVideoSettingsSaveData.screenMode = (int)Screen.fullScreenMode;
            videoSettingsSave.Save(currentVideoSettingsSaveData);
        }
    }

    private static void SetupResolutions()
    {
        resolutions = new(ReverseArray(Screen.resolutions));
        resolutionsNames = new(resolutions.Count);
        availableRefreshRates = new();
        availableRefreshRatesNames = new();

        for (int i = 0; i < resolutions.Count; i++)
        {
            resolutionsNames.Add($"{resolutions[i].width}x{resolutions[i].height}");

            _AddUnique(availableRefreshRates, resolutions[i].refreshRateRatio);
            _AddUnique(availableRefreshRatesNames, resolutions[i].refreshRateRatio.ToString());
        }

        OnResolutionListUpdated?.Invoke(resolutions);

        static void _AddUnique<T>(List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
    }

    private static void ValidateResolution(int width, int height, uint refreshRateNumerator, uint refreshRateDenominator)
    {
        Resolution savedResolution = new()
        {
            width = width,
            height = height,
            refreshRateRatio = new RefreshRate()
            {
                numerator = refreshRateNumerator,
                denominator = refreshRateDenominator
            },
        };

        Resolution realActiveResolution = Screen.currentResolution;

        if (!AreResolutionsEqual(savedResolution, realActiveResolution))
        {
            currentVideoSettingsSaveData.resolutionWidth = realActiveResolution.width;
            currentVideoSettingsSaveData.resolutionHeight = realActiveResolution.height;
            currentVideoSettingsSaveData.resolutionRefreshRateNumerator = realActiveResolution.refreshRateRatio.numerator;
            currentVideoSettingsSaveData.resolutionRefreshRateDenominator = realActiveResolution.refreshRateRatio.denominator;
            currentVideoSettingsSaveData.resolutionIndex = resolutions.IndexOf(realActiveResolution);
        }
    }

    private static void SetupDisplayOptions(int windowIndex)
    {
        Screen.GetDisplayLayout(availableDisplayInfos);

        if (Screen.mainWindowDisplayInfo.Equals(availableDisplayInfos[windowIndex]))
        {
            return;
        }

        currentVideoSettingsSaveData.displayWindowIndex = availableDisplayInfos.IndexOf(Screen.mainWindowDisplayInfo);
    }

    public static bool IsCachedVideoValuesCorrect()
    {
#if UNITY_EDITOR
        return true;
#endif
        Resolution cachedResolution = GetSelectedResolution();
        Resolution activeResolution = Screen.currentResolution;

        if (AreResolutionsEqual(cachedResolution, activeResolution))
        {
            UpdateScreenResolutionAndScreenMode(activeResolution, GetSelectedScreenMode());
            return false;
        }

        FullScreenMode cachedScreenMode = GetSelectedScreenMode();
        FullScreenMode activeScreenMode = Screen.fullScreenMode;

        if (cachedScreenMode != activeScreenMode)
        {
            UpdateScreenResolutionAndScreenMode(activeResolution, activeScreenMode);
            return false;
        }

        return true;
    }

    public static void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];

        tempVideoSettingsSaveData.resolutionWidth = resolution.width;
        tempVideoSettingsSaveData.resolutionHeight = resolution.height;
        tempVideoSettingsSaveData.resolutionRefreshRateNumerator = resolution.refreshRateRatio.numerator;
        tempVideoSettingsSaveData.resolutionRefreshRateDenominator = resolution.refreshRateRatio.denominator;
    }

    public static void SetScreenMode(FullScreenMode screenMode)
    {
        tempVideoSettingsSaveData.screenMode = (int)screenMode;
    }

    public static void SetFrameRate(int frames)
    {
        tempVideoSettingsSaveData.frameRate = frames;
    }

    public static void SetVSync(int vSync)
    {
        tempVideoSettingsSaveData.vSync = vSync;
    }

    public static void SetActiveDisplay(int displayIndex)
    {
        tempVideoSettingsSaveData.displayWindowIndex = displayIndex;
    }

    public static void UpdateScreenResolutionAndScreenMode(Resolution resolution, FullScreenMode fullScreenMode)
    {
        SetResolution(resolutions.IndexOf(resolution));
        SetScreenMode(fullScreenMode);

        SaveChanges();
    }

    public static async void ApplyChanges(bool reverting = false)
    {
        Application.targetFrameRate = GetSelectedFrameRate();
        QualitySettings.vSyncCount = GetSelectedVSync();

        bool significantUpdate = TryUpdateResolutionOrScreenMode() || await TryUpdateDisplayWindow();

        if (reverting)
        {
            return;
        }

        if (!significantUpdate)
        {
            SaveChanges();
            return;
        }

        bool confirm = await WaitUserConfirmation();

        if (confirm)
        {
            SaveChanges();
        }
        else
        {
            CancelVideoChanges();
            ApplyChanges(true);
        }
    }

    private static bool TryUpdateResolutionOrScreenMode()
    {
        Resolution currentResolution = GetSelectedResolution();
        if (!AreResolutionsEqual(currentResolution, Screen.currentResolution))
        {
            Screen.SetResolution(currentResolution.width, currentResolution.height, GetSelectedScreenMode(), currentResolution.refreshRateRatio);
            return true;
        }

        return false;
    }

    private static async UniTask<bool> TryUpdateDisplayWindow()
    {
        DisplayInfo display = GetSelectedDisplayWindow();
        if (display.Equals(Screen.mainWindowDisplayInfo))
        {
            return false;
        }

        Vector2Int targetCoordinates = new Vector2Int(0, 0);
        if (Screen.fullScreenMode != FullScreenMode.Windowed)
        {
            // Target the center of the display. Doing it this way shows off
            // that MoveMainWindow snaps the window to the top left corner
            // of the display when running in fullscreen mode.
            targetCoordinates.x += display.width / 2;
            targetCoordinates.y += display.height / 2;
        }

        await Screen.MoveMainWindowTo(in display, targetCoordinates);

        SetupResolutions();

        return true;
    }

    private static void SaveChanges()
    {
        if (HasChanges())
        {
            videoSettingsSave.Save(tempVideoSettingsSaveData);
            currentVideoSettingsSaveData = tempVideoSettingsSaveData;
            return;
        }
    }

    public static void CancelVideoChanges()
    {
        tempVideoSettingsSaveData = currentVideoSettingsSaveData;
    }

    public static Resolution GetSelectedResolution() => resolutions[tempVideoSettingsSaveData.resolutionIndex];

    public static int GetSelectedResolutionIndex() => tempVideoSettingsSaveData.resolutionIndex;

    public static int GetSelectedFrameRate() => tempVideoSettingsSaveData.frameRate;

    public static int GetSelectedVSync() => tempVideoSettingsSaveData.vSync;

    public static DisplayInfo GetSelectedDisplayWindow() => availableDisplayInfos[tempVideoSettingsSaveData.displayWindowIndex];

    public static FullScreenMode GetSelectedScreenMode() => (FullScreenMode)tempVideoSettingsSaveData.screenMode;

    private static bool AreResolutionsEqual(Resolution a, Resolution b)
    {
        return a.width == b.width && a.height == b.height && a.refreshRateRatio.value == b.refreshRateRatio.value;
    }

    private static bool HasChanges()
    {
        return !tempVideoSettingsSaveData.Equals(currentVideoSettingsSaveData);
    }

    private static IReadOnlyList<T> ReverseArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (i >= array.Length * 0.5f)
            {
                break;
            }

            (array[i], array[array.Length - 1 - i]) = (array[array.Length - 1 - i], array[i]);
        }

        return array;
    }

    private static UniTask<bool> WaitUserConfirmation()
    {
        return UniTask.FromResult(true);
    }
}
