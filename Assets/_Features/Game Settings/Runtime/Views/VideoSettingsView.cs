using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettingsView : MonoBehaviour, IDisposable
{
    [SerializeField] Button applyChanges;
    [SerializeField] Button revertChanges;

    [SerializeField] LocalizeDropdownEvent displayOptions;
    [SerializeField] LocalizeDropdownEvent screenModeDropdown;
    [SerializeField] LocalizeDropdownEvent resolutionDropdown;
    [SerializeField] LocalizeDropdownEvent refreshRateDropdown;
    [SerializeField] AntiAliasingView _antiAliasing;
    [Space]
    [SerializeField] SliderField frameRateSlider;
    [SerializeField] Toggle vSyncOptions;

    private void Start()
    {
        Setup();
        UpdateScreenVisuals();
        RegisterListeners();
    }

    private void Update()
    {
        ValidateVideoValues();
    }

    private void OnDestroy()
    {
        Dispose();
    }

    public void Setup()
    {
        applyChanges.onClick.AddListener(ApplyVideoChanges);
        revertChanges.onClick.AddListener(CancelVideoChanges);

        _antiAliasing.Initialize();

        InitializeVisualOptions();
    }

    private void RegisterListeners()
    {
        vSyncOptions.onValueChanged.AddListener(SetVSync);
        screenModeDropdown.TmpDropdown.onValueChanged.AddListener(SetScreenMode);
        frameRateSlider.onValueChangedFloat.AddListener(SetFrameRate);
        displayOptions.TmpDropdown.onValueChanged.AddListener(SetActiveDisplay);
        resolutionDropdown.TmpDropdown.onValueChanged.AddListener(SetResolution);
        refreshRateDropdown.TmpDropdown.onValueChanged.AddListener(SetRefreshRate);
    }

    public void Dispose()
    {
        resolutionDropdown.TmpDropdown.onValueChanged.RemoveListener(SetResolution);
        screenModeDropdown.TmpDropdown.onValueChanged.RemoveListener(SetScreenMode);
        displayOptions.TmpDropdown.onValueChanged.RemoveListener(SetActiveDisplay);
        refreshRateDropdown.TmpDropdown.onValueChanged.RemoveListener(SetRefreshRate);

        frameRateSlider.onValueChangedFloat.RemoveListener(SetFrameRate);
        vSyncOptions.onValueChanged.RemoveListener(SetVSync);

        applyChanges.onClick.RemoveListener(ApplyVideoChanges);
        revertChanges.onClick.RemoveListener(CancelVideoChanges);
    }

    private void InitializeVisualOptions()
    {
        LocalizeResolutions();
        LocalizeScreenModeDropdown();
        LocalizeDisplayMonitor();
        LocalizeRefreshRate();
    }

    private void ValidateVideoValues()
    {
        if (VideoSettings.IsCachedVideoValuesCorrect())
        {
            return;
        }

        UpdateScreenVisuals();
    }

    private void LocalizeResolutions()
    {
        resolutionDropdown.TmpDropdown.ClearOptions();

        List<string> resolutions = new();

        foreach (Resolution item in VideoSettings.GetAvailableResolutions())
        {
            resolutions.Add($"{item.width}x{item.height}");
        }

        resolutionDropdown.TmpDropdown.AddOptions(resolutions);
    }

    private void LocalizeRefreshRate()
    {
        refreshRateDropdown.TmpDropdown.ClearOptions();

        List<string> refreshRates = new();

        foreach (RefreshRate item in VideoSettings.GetAvailableRefreshRates())
        {
            refreshRates.Add($"{item}Hz");
        }

        refreshRateDropdown.TmpDropdown.AddOptions(refreshRates);
    }

    private void LocalizeDisplayMonitor()
    {
        displayOptions.TmpDropdown.ClearOptions();

        List<string> videoSettingsDropdownOptions = new();

        foreach (DisplayInfo displayInfo in VideoSettings.GetAvailableDisplayInfos())
        {
            videoSettingsDropdownOptions.Add(displayInfo.name);
        }

        displayOptions.TmpDropdown.AddOptions(videoSettingsDropdownOptions);
    }

    private void LocalizeScreenModeDropdown()
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
    }

    private void UpdateScreenVisuals()
    {
        screenModeDropdown.TmpDropdown.SetValueWithoutNotify(GetSelectedScreenMode(VideoSettings.GetTempSelectedScreenMode()));
        screenModeDropdown.TmpDropdown.onValueChanged.Invoke(screenModeDropdown.TmpDropdown.value); //Force notify

        //TODO: create generic that gets from runtime index
        IList<Resolution> resolutions = VideoSettings.GetAvailableResolutions();
        Resolution resolution = VideoSettings.GetTempSelectedResolution();
        int resolutionIndex = resolutions.IndexOf(resolution);
        resolutionDropdown.TmpDropdown.SetValueWithoutNotify(resolutionIndex);
        resolutionDropdown.TmpDropdown.onValueChanged.Invoke(resolutionDropdown.TmpDropdown.value);

        IList<DisplayInfo> displayInfos = VideoSettings.GetAvailableDisplayInfos();
        DisplayInfo display = VideoSettings.GetTempSelectedDisplayInfo();
        int displayIndex = displayInfos.IndexOf(display);
        displayOptions.TmpDropdown.SetValueWithoutNotify(displayIndex);
        displayOptions.TmpDropdown.onValueChanged.Invoke(displayOptions.TmpDropdown.value);

        IList<RefreshRate> rrs = VideoSettings.GetAvailableRefreshRates();
        RefreshRate rr = VideoSettings.GetTempSelectedRefreshRate();
        int refreshRateindex = rrs.IndexOf(rr);
        refreshRateDropdown.TmpDropdown.SetValueWithoutNotify(refreshRateindex);
        refreshRateDropdown.TmpDropdown.onValueChanged.Invoke(refreshRateDropdown.TmpDropdown.value);

        frameRateSlider.SetValue(VideoSettings.GetTempSelectedFrameRate());
        frameRateSlider.ForceNotify();

        vSyncOptions.SetIsOnWithoutNotify(VideoSettings.GetTempSelectedVSync() != 0);
        vSyncOptions.onValueChanged.Invoke(vSyncOptions.isOn);

        _antiAliasing.UpdateVisuals();
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
