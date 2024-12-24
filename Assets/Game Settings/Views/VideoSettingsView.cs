using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class VideoSettingsView : MonoBehaviour
{
    [SerializeField] LocalizeDropdownEvent displayOptions;
    [SerializeField] LocalizeDropdownEvent screenModeDropdown;
    [SerializeField] LocalizeDropdownEvent resolutionDropdown;
    [SerializeField] LocalizeDropdownEvent refreshRateDropdown;
    [SerializeField] LocalizeDropdownEvent vSyncOptions;

    private List<TMP_Dropdown.OptionData> _videoSettingsDropdownOptions = new();

    private void Start()
    {
        VideoSettings.SetupScreenSettings();

        Setup();
        UpdateScreenVisuals();
    }

    private void Update()
    {
        ValidateVideoValues();
    }

    public void Setup()
    {
        resolutionDropdown.AddOptions(VideoSettings.GetCurrentResolutionsNames().ToList());

        SetupDisplayMonitor();
        SetupScreenModeDropdown();
        SetupVSync();

        refreshRateDropdown.AddOptions(VideoSettings.GetCurrentRefreshRateNames().ToList());
    }

    private void UpdateScreenVisuals()
    {
        resolutionDropdown.SetValueWithoutNotify(VideoSettings.GetSelectedResolutionIndex());
        resolutionDropdown.RefreshShownValue();

        screenModeDropdown.SetValueWithoutNotify(GetSelectedScreenMode(VideoSettings.GetSelectedScreenMode()));
        screenModeDropdown.RefreshShownValue();
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

    public void ApplyChanges()
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
            0 => FullScreenMode.FullScreenWindow, //Borderless
            1 => FullScreenMode.Windowed, //Windowed
#if UNITY_STANDALONE_WIN
            2 => FullScreenMode.ExclusiveFullScreen,
#elif UNITY_STANDALONE_OSX
            2 => FullScreenMode.MaximizedWindow,
#endif
            _ => FullScreenMode.FullScreenWindow
        };
    }

    public static int GetSelectedScreenMode(FullScreenMode screenMode)
    {
        return screenMode switch
        {
            FullScreenMode.FullScreenWindow => 0, //Borderless
            FullScreenMode.Windowed => 1, //Windowed
#if UNITY_STANDALONE_WIN
            FullScreenMode.ExclusiveFullScreen => 2,
#elif UNITY_STANDALONE_OSX
            FullScreenMode.MaximizedWindow => 2,
#endif
            _ => 0,
        };
    }

    private void SetupDisplayMonitor()
    {
        foreach (DisplayInfo displayInfo in VideoSettings.GetCurrentDisplayOptions())
        {
            _videoSettingsDropdownOptions.Add(new LocalizedDropdownOption(displayInfo.name));
        }

        displayOptions.AddOptions(_videoSettingsDropdownOptions);
    }

    private void SetupScreenModeDropdown()
    {
        List<TMP_Dropdown.OptionData> availableScreenModesNames = new()
        {
            new LocalizedDropdownOption("Borderless"),
            new LocalizedDropdownOption("Windowed"),
#if UNITY_STANDALONE_WIN
            new LocalizedDropdownOption("FullScreen"),
#elif UNITY_STANDALONE_OSX
            new LocalizedDropdownOption("Maximized"),
#endif
        };

        screenModeDropdown.AddOptions(availableScreenModesNames);
    }

    private void SetupVSync()
    {
        vSyncOptions.AddOptions(new List<TMP_Dropdown.OptionData>()
        {
            new LocalizedDropdownOption("Test", "f1"),
            new LocalizedDropdownOption("VSync"),
            new LocalizedDropdownOption("Tripple Buffer"),
        });
    }
}
