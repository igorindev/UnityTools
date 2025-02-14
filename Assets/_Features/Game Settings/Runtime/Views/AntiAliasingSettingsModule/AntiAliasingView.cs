using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public class AntiAliasingView : SettingsModuleView
{
    [SerializeField] LocalizeDropdownEvent antiAliasingDropdown;
    [SerializeField] LocalizeDropdownEvent antiAliasingSMAAQualityDropdown;
    [SerializeField] LocalizeDropdownEvent antiAliasingTAAQualityDropdown;
    [SerializeField] SliderField antiAliasingTAASharpen;

    public override void Initialize()
    {
        List<KeyValuePair<string, string>> antiAliasingLocalizations = new()
        {
            new("VideoSettings", "none"),
            new("VideoSettings", "fxaa"),
            new("VideoSettings", "smaa"),
            new("VideoSettings", "taa"),
        };

        List<KeyValuePair<string, string>> antiAliasingTAAQualityLocalizations = new()
        {
            new("VideoSettings", "verylow"),
            new("VideoSettings", "low"),
            new("VideoSettings", "medium"),
            new("VideoSettings", "high"),
            new("VideoSettings", "veryhigh"),
        };

        List<KeyValuePair<string, string>> antiAliasingSMAAQualityLocalizations = new()
        {
            new("VideoSettings", "low"),
            new("VideoSettings", "medium"),
            new("VideoSettings", "high"),
        };

        antiAliasingDropdown.AddLocalizationValues(antiAliasingLocalizations);
        antiAliasingSMAAQualityDropdown.AddLocalizationValues(antiAliasingSMAAQualityLocalizations);
        antiAliasingTAAQualityDropdown.AddLocalizationValues(antiAliasingTAAQualityLocalizations);

        antiAliasingDropdown.TmpDropdown.onValueChanged.AddListener(SelectAAMode);
        antiAliasingSMAAQualityDropdown.TmpDropdown.onValueChanged.AddListener(SelectSMAAQuality);
        antiAliasingTAAQualityDropdown.TmpDropdown.onValueChanged.AddListener(SelectTAAQuality);

        antiAliasingTAASharpen.Setup(1, 0f, 1f, false, 0.1f);
    }

    public override void Dispose()
    {
        antiAliasingDropdown.TmpDropdown.onValueChanged.RemoveListener(SelectAAMode);
        antiAliasingSMAAQualityDropdown.TmpDropdown.onValueChanged.RemoveListener(SelectSMAAQuality);
        antiAliasingTAAQualityDropdown.TmpDropdown.onValueChanged.AddListener(SelectTAAQuality);
    }

    public void SelectAAMode(int antiAliasingIndex)
    {
        var currentAA = (AntialiasingMode)antiAliasingIndex;
    }

    public void SelectSMAAQuality(int quality)
    {
        var currentAAQuality = (AntialiasingQuality)quality;
    }

    public void SelectTAAQuality(int taaQuality)
    {
        var currentTAAQuality = (TemporalAAQuality)taaQuality;
    }

    public void SelectTAASharpen(int sharpen)
    {
        var sharpeness = sharpen;
    }
}
