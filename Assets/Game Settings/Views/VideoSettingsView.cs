using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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
        resolutionDropdown.TmpDropdown.AddOptions(VideoSettings.GetCurrentResolutionsNames().ToList());

        SetupDisplayMonitor();
        SetupScreenModeDropdown();
        SetupVSync();

        refreshRateDropdown.TmpDropdown.AddOptions(VideoSettings.GetCurrentRefreshRateNames().ToList());
    }

    private void UpdateScreenVisuals()
    {
        resolutionDropdown.TmpDropdown.SetValueWithoutNotify(VideoSettings.GetSelectedResolutionIndex());
        resolutionDropdown.TmpDropdown.RefreshShownValue();

        screenModeDropdown.TmpDropdown.SetValueWithoutNotify(GetSelectedScreenMode(VideoSettings.GetSelectedScreenMode()));
        screenModeDropdown.TmpDropdown.RefreshShownValue();
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

    private void SetupDisplayMonitor()
    {
        foreach (DisplayInfo displayInfo in VideoSettings.GetCurrentDisplayOptions())
        {
            _videoSettingsDropdownOptions.Add(new TMP_Dropdown.OptionData(displayInfo.name));
        }

        displayOptions.TmpDropdown.AddOptions(_videoSettingsDropdownOptions);
    }

    private void SetupScreenModeDropdown()
    {
        List<TMP_Dropdown.OptionData> availableScreenModesNames = new()
        {
            new TMP_Dropdown.OptionData("Borderless"),
            new TMP_Dropdown.OptionData("Windowed"),
#if UNITY_STANDALONE_WIN
            new TMP_Dropdown.OptionData("FullScreen"),
#elif UNITY_STANDALONE_OSX
            new TMP_Dropdown.OptionData("Maximized"),
#endif
        };

        List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>
        {
#if UNITY_STANDALONE_WIN
            new KeyValuePair<string, string>("VideoSettings", "fullscreen"),
#elif UNITY_STANDALONE_OSX
            new KeyValuePair<string, string>("VideoSettings", "fullscreen"),
#endif
            new KeyValuePair<string, string>("VideoSettings", "borderless"),
            new KeyValuePair<string, string>("VideoSettings", "windowed"),
        };

        screenModeDropdown.AddLocalizationValues(list);
    }

    private void SetupVSync()
    {
        vSyncOptions.TmpDropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
        {
            new TMP_Dropdown.OptionData("Test"),
            new TMP_Dropdown.OptionData("VSync"),
            new TMP_Dropdown.OptionData("Tripple Buffer"),
        });
    }
}
