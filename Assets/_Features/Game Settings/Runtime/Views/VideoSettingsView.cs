using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEngine.UI;

public class VideoSettingsView : MonoBehaviour, IDisposable
{
    [SerializeField] Button applyChanges;
    [SerializeField] Button revertChanges;

    [SerializeField] LocalizeDropdownEvent displayOptions;
    [SerializeField] LocalizeDropdownEvent screenModeDropdown;
    [SerializeField] LocalizeDropdownEvent resolutionDropdown;
    [SerializeField] LocalizeDropdownEvent refreshRateDropdown;
    [Space]
    [SerializeField] Slider frameRateSlider;
    [SerializeField] Toggle vSyncOptions;

    private void Start()
    {
        Setup();
        UpdateScreenVisuals();
    }

    private void Update()
    {
        ValidateVideoValues();
    }

    public void Dispose()
    {
        resolutionDropdown.TmpDropdown.onValueChanged.RemoveListener(SetResolution);
        screenModeDropdown.TmpDropdown.onValueChanged.RemoveListener(SetScreenMode);
        displayOptions.TmpDropdown.onValueChanged.RemoveListener(SetActiveDisplay);
        refreshRateDropdown.TmpDropdown.onValueChanged.RemoveListener(SetRefreshRate);

        frameRateSlider.onValueChanged.RemoveListener(SetFrameRate);
        vSyncOptions.onValueChanged.RemoveListener(SetVSync);

        applyChanges.onClick.RemoveListener(ApplyVideoChanges);
        revertChanges.onClick.RemoveListener(CancelVideoChanges);
    }

    public void Setup()
    {
        applyChanges.onClick.AddListener(ApplyVideoChanges);
        revertChanges.onClick.AddListener(CancelVideoChanges);

        ReloadVideoValues();

        SetupVSync();
        SetupFrameRate();
    }

    private void SetupResolutions()
    {
        resolutionDropdown.TmpDropdown.ClearOptions();

        List<string> resolutions = new();

        foreach (Resolution item in VideoSettings.GetAvailableResolutions())
        {
            resolutions.Add($"{item.width}x{item.height}");
        }

        resolutionDropdown.TmpDropdown.AddOptions(resolutions);

        resolutionDropdown.TmpDropdown.onValueChanged.AddListener(SetResolution);
    }

    private void SetupRefreshRate()
    {
        refreshRateDropdown.TmpDropdown.ClearOptions();

        List<string> refreshRates = new();

        foreach (RefreshRate item in VideoSettings.GetAvailableRefreshRates())
        {
            refreshRates.Add($"{item}Hz");
        }

        refreshRateDropdown.TmpDropdown.AddOptions(refreshRates);

        refreshRateDropdown.TmpDropdown.onValueChanged.AddListener(SetRefreshRate);
    }

    private void SetupDisplayMonitor()
    {
        displayOptions.TmpDropdown.ClearOptions();

        List<string> videoSettingsDropdownOptions = new();

        foreach (DisplayInfo displayInfo in VideoSettings.GetAvailableDisplayInfos())
        {
            videoSettingsDropdownOptions.Add(displayInfo.name);
        }

        displayOptions.TmpDropdown.AddOptions(videoSettingsDropdownOptions);

        displayOptions.TmpDropdown.onValueChanged.AddListener(SetActiveDisplay);
    }

    private void SetupScreenModeDropdown()
    {
        screenModeDropdown.TmpDropdown.ClearOptions();

        List<KeyValuePair<string, string>> screenModeLocalizations = new()
        {
#if UNITY_STANDALONE_WIN
            new("VideoSettings", "fullscreen"),
#elif UNITY_STANDALONE_OSX
            new("VideoSettings", "fullscreen"),
#endif
            new("VideoSettings", "borderless"),
            new("VideoSettings", "windowed"),
        };

        screenModeDropdown.AddLocalizationValues(screenModeLocalizations);

        screenModeDropdown.TmpDropdown.onValueChanged.AddListener(SetScreenMode);
    }

    private void SetupVSync()
    {
        vSyncOptions.onValueChanged.AddListener(SetVSync);
    }

    private void SetupFrameRate()
    {
        frameRateSlider.onValueChanged.AddListener(SetFrameRate);
    }

    private void ReloadVideoValues()
    {
        SetupResolutions();
        SetupScreenModeDropdown();
        SetupDisplayMonitor();
        SetupRefreshRate();
    }

    private void UpdateScreenVisuals()
    {
        screenModeDropdown.TmpDropdown.SetValueWithoutNotify(GetSelectedScreenMode(VideoSettings.GetTempSelectedScreenMode()));

        //TODO: create generic that gets from runtime index
        IList<Resolution> resolutions = VideoSettings.GetAvailableResolutions();
        Resolution resolution = VideoSettings.GetTempSelectedResolution();
        int resolutionIndex = resolutions.IndexOf(resolution);
        resolutionDropdown.TmpDropdown.SetValueWithoutNotify(resolutionIndex);

        IList<DisplayInfo> displayInfos = VideoSettings.GetAvailableDisplayInfos();
        DisplayInfo display = VideoSettings.GetTempSelectedDisplayInfo();
        int displayIndex = displayInfos.IndexOf(display);
        displayOptions.TmpDropdown.SetValueWithoutNotify(displayIndex);

        IList<RefreshRate> rrs = VideoSettings.GetAvailableRefreshRates();
        RefreshRate rr = VideoSettings.GetTempSelectedRefreshRate();
        int refreshRateindex = rrs.IndexOf(rr);
        refreshRateDropdown.TmpDropdown.SetValueWithoutNotify(refreshRateindex);


        //TODO: Add to slider
        //frameRateDropdown.TmpDropdown.SetValueWithoutNotify(VideoSettings.GetSelectedFrameRate());

        vSyncOptions.SetIsOnWithoutNotify(VideoSettings.GetTempSelectedVSync() != 0);
    }

    private void ValidateVideoValues()
    {
        if (VideoSettings.IsCachedVideoValuesCorrect())
        {
            return;
        }

        UpdateScreenVisuals();
    }

    public void SetResolution(int value)
    {
        VideoSettings.SetResolution(value);
    }

    public void SetScreenMode(int value)
    {
        VideoSettings.SetScreenMode(GetSelectedScreenMode(value));
    }

    public void SetVSync(bool value)
    {
        VideoSettings.SetVSync(value ? 1 : 0);
    }

    public void SetActiveDisplay(int value)
    {
        VideoSettings.SetActiveDisplay(value);
    }

    public void SetRefreshRate(int value)
    {
        VideoSettings.SetRefreshRate(value);
    }

    public void SetFrameRate(float value)
    {
        VideoSettings.SetFrameRate((int)value);
    }

    public void ApplyVideoChanges()
    {
        VideoSettings.ApplyChanges();
    }

    public void CancelVideoChanges()
    {
        VideoSettings.CancelVideoChanges();
    }

    public static FullScreenMode GetSelectedScreenMode(int selection)
    {
        return selection switch
        {
            1 => FullScreenMode.FullScreenWindow, //Borderless
            2 => FullScreenMode.Windowed, //Windowed
#if UNITY_STANDALONE_WIN
            0 => FullScreenMode.ExclusiveFullScreen,
#elif UNITY_STANDALONE_OSX
            0 => FullScreenMode.MaximizedWindow,
#endif
            _ => FullScreenMode.FullScreenWindow
        };
    }

    public static int GetSelectedScreenMode(FullScreenMode screenMode)
    {
        return screenMode switch
        {
            FullScreenMode.FullScreenWindow => 1, //Borderless
            FullScreenMode.Windowed => 2, //Windowed
#if UNITY_STANDALONE_WIN
            FullScreenMode.ExclusiveFullScreen => 0,
#elif UNITY_STANDALONE_OSX
            FullScreenMode.MaximizedWindow => 0,
#endif
            _ => 1,
        };
    }
}
