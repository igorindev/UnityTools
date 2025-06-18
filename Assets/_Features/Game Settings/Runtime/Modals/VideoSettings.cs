using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Modules;
using UnityEngine;

public class VideoSettings : Modular<VideoSettingsModuleCollection>
{
    public enum QualityLevel
    {
        Low = 0,
        Medium,
        High,
        Ultra
    }

    public static event Action<IReadOnlyList<Resolution>> OnScreenSettingsUpdated;

    private static List<Resolution> resolutions = new();
    private static List<FullScreenMode> availableScreenModes = new();
    private static List<RefreshRate> availableRefreshRates = new();
    private static List<DisplayInfo> availableDisplayInfos = new();

    private static VideoSettingsSaveData videoSettingsSaveHandler;
    private static VideoSettingsData currentSaveData;
    private static VideoSettingsData tempSaveData;

    //A UNITY ja guarda algumas informacoes de video, talvez eu nao queira mais guardar esses.
    //Caso queria, tneh oque garantir o fluxo:
    //Abrir o jogo
    //Unity seta os valores em cache
    //Unity realiza fallback para melhores opções se nao acha anterior
    //Substitui valores no save
    //Pega valores do save

    public VideoSettings() : base()
    {
        videoSettingsSaveHandler = new VideoSettingsSaveData(out currentSaveData);

        LoadVideoModule(currentSaveData);

        if (true) //Validate if should initialize the save file
        {
            SetupScreenModes(currentSaveData.screenMode);

            SetupResolutionsAndRefreshRates(currentSaveData.resolutionWidth,
                currentSaveData.resolutionHeight,
                currentSaveData.resolutionRefreshRateNumerator,
                currentSaveData.resolutionRefreshRateDenominator);

            SetupDisplayOptions(currentSaveData.displayWindow);
        }

        videoSettingsSaveHandler.Save(currentSaveData);
        tempSaveData = currentSaveData;

        ApplyChanges();
    }

    private void LoadVideoModule(VideoSettingsData videoSettingsData)
    {
        foreach (Module modules in _modules)
        {
            if (!videoSettingsData.settingsSaveModule.TryGetValue(modules.Type, out SettingsSaveModule settingsSave))
            {
                videoSettingsData.settingsSaveModule.Add(modules.Type, modules.GetSaveInstance() as SettingsSaveModule);
            }
        }
    }

    private static void SetupScreenModes(int screenMode)
    {
        //if ((FullScreenMode)screenMode != Screen.fullScreenMode)
        //{
        //    currentVideoSettingsSaveData.screenMode = (int)Screen.fullScreenMode;
        //}
    }

    private static void SetupResolutionsAndRefreshRates(int width, int height, uint refreshRateNumerator, uint refreshRateDenominator)
    {
        resolutions = new(ReverseArray(Screen.resolutions));
        availableRefreshRates = new();

        for (int i = 0; i < resolutions.Count; i++)
        {
            AddUniqueToList(availableRefreshRates, resolutions[i].refreshRateRatio);
        }

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

        //Fallback
        if (!resolutions.Contains(savedResolution))
        {
            currentSaveData.resolutionWidth = resolutions[0].width;
            currentSaveData.resolutionHeight = resolutions[0].height;
            currentSaveData.resolutionRefreshRateNumerator = resolutions[0].refreshRateRatio.numerator;
            currentSaveData.resolutionRefreshRateDenominator = resolutions[0].refreshRateRatio.denominator;

            return;
        }
    }

    private static void SetupDisplayOptions(string displayInfoName)
    {
        Screen.GetDisplayLayout(availableDisplayInfos);

        DisplayInfo display = availableDisplayInfos.FirstOrDefault(x => x.name == displayInfoName);

        //Fallback
        if (display.Equals(default))
        {
            currentSaveData.displayWindow = Screen.mainWindowDisplayInfo.name;
        }
    }

    public static bool IsCachedVideoValuesCorrect()
    {
#if UNITY_EDITOR
        return true;
#endif
        bool correct = true;

        Resolution cachedResolution = GetTempSelectedResolution();
        Resolution activeResolution = Screen.currentResolution;

        FullScreenMode cachedScreenMode = GetTempSelectedScreenMode();
        FullScreenMode activeScreenMode = Screen.fullScreenMode;

        if (AreResolutionsEqual(cachedResolution, activeResolution) || cachedScreenMode != activeScreenMode)
        {
            UpdateScreenResolutionAndScreenMode(activeResolution, activeScreenMode);
            correct = false;
        }

        DisplayInfo displayWindow = GetTempSelectedDisplayInfo();
        DisplayInfo activeDisplayInfo = Screen.mainWindowDisplayInfo;

        if (!displayWindow.Equals(activeDisplayInfo))
        {
            currentSaveData.displayWindow = Screen.mainWindowDisplayInfo.name;
            correct = false;
        }

        return correct;
    }

    public static void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];

        tempSaveData.resolutionWidth = resolution.width;
        tempSaveData.resolutionHeight = resolution.height;
        tempSaveData.resolutionRefreshRateNumerator = resolution.refreshRateRatio.numerator;
        tempSaveData.resolutionRefreshRateDenominator = resolution.refreshRateRatio.denominator;
    }

    public static void SetScreenMode(FullScreenMode screenMode)
    {
        tempSaveData.screenMode = (int)screenMode;
    }

    public static void SetRefreshRate(int value)
    {
        RefreshRate refreshRate = availableRefreshRates[value];

        tempSaveData.resolutionRefreshRateNumerator = refreshRate.numerator;
        tempSaveData.resolutionRefreshRateDenominator = refreshRate.denominator;
    }

    public static void SetFrameRate(int frames)
    {
        tempSaveData.frameRate = frames;
    }

    public static void SetVSync(int vSync)
    {
        tempSaveData.vSync = vSync;
    }

    public static void SetActiveDisplay(int displayIndex)
    {
        tempSaveData.displayWindow = availableDisplayInfos[displayIndex].name;
    }

    public static void UpdateScreenResolutionAndScreenMode(Resolution resolution, FullScreenMode fullScreenMode)
    {
        SetResolution(resolutions.IndexOf(resolution));
        SetScreenMode(fullScreenMode);

        SaveChanges();
    }

    public static async void ApplyChanges(bool reverting = false)
    {
        //TODO: block all interaction until everything is applied

        Application.targetFrameRate = GetTempSelectedFrameRate();
        QualitySettings.vSyncCount = GetTempSelectedVSync();

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
        Resolution currentResolution = GetTempSelectedResolution();
        RefreshRate currentRefreshRate = GetTempSelectedRefreshRate();
        if (!AreResolutionsEqual(currentResolution, Screen.currentResolution))
        {
            Screen.SetResolution(currentResolution.width, currentResolution.height, GetTempSelectedScreenMode(), currentRefreshRate);
            return true;
        }

        return false;
    }

    private static async UniTask<bool> TryUpdateDisplayWindow()
    {
        DisplayInfo display = GetTempSelectedDisplayInfo();
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

        //SetupResolutionsAndRefreshRates();

        return true;
    }

    private static void SaveChanges()
    {
        if (HasChanges())
        {
            currentSaveData = tempSaveData;
            videoSettingsSaveHandler.Save(currentSaveData);
            return;
        }
    }

    public static void CancelVideoChanges()
    {
        tempSaveData = currentSaveData;
    }

    public static IList<Resolution> GetAvailableResolutions() => resolutions;

    public static IList<RefreshRate> GetAvailableRefreshRates() => availableRefreshRates;

    public static IList<DisplayInfo> GetAvailableDisplayInfos() => availableDisplayInfos;

    public static Resolution GetTempSelectedResolution()
    {
        return new()
        {
            width = tempSaveData.resolutionWidth,
            height = tempSaveData.resolutionHeight,
            refreshRateRatio = GetTempSelectedRefreshRate()
        };
    }

    public static RefreshRate GetTempSelectedRefreshRate() => new()
    {
        numerator = tempSaveData.resolutionRefreshRateNumerator,
        denominator = tempSaveData.resolutionRefreshRateDenominator
    };

    public static int GetTempSelectedFrameRate() => tempSaveData.frameRate;

    public static int GetTempSelectedVSync() => tempSaveData.vSync;

    public static DisplayInfo GetTempSelectedDisplayInfo() => availableDisplayInfos.FirstOrDefault(displayInfo => displayInfo.name == tempSaveData.displayWindow);

    public static FullScreenMode GetTempSelectedScreenMode() => (FullScreenMode)tempSaveData.screenMode;

    private static bool AreResolutionsEqual(Resolution a, Resolution b)
    {
        return a.width == b.width && a.height == b.height && a.refreshRateRatio.value == b.refreshRateRatio.value;
    }

    private static bool HasChanges()
    {
        return !tempSaveData.Equals(currentSaveData);
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

    private static void AddUniqueToList<T>(List<T> list, T item)
    {
        if (!list.Contains(item))
        {
            list.Add(item);
        }
    }
}
