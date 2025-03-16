using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AntiAliasingView : SettingsModuleView
{
    [SerializeField] LocalizeDropdownEvent antiAliasingDropdown;
    [SerializeField] LocalizeDropdownEvent antiAliasingSMAAQualityDropdown;
    [SerializeField] LocalizeDropdownEvent antiAliasingTAAQualityDropdown;
    [SerializeField] SliderField antiAliasingTAASharpen;

    private AntiAliasing aa;

    public override void Initialize()
    {
        aa = VideoSettings.Get<AntiAliasing>();

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
        antiAliasingTAASharpen.onValueChangedFloat.AddListener(SelectTAASharpen);

        antiAliasingTAASharpen.Setup(1, 0f, 1f, false, 0.1f);
    }

    public override void Dispose()
    {
        antiAliasingDropdown.TmpDropdown.onValueChanged.RemoveListener(SelectAAMode);
        antiAliasingSMAAQualityDropdown.TmpDropdown.onValueChanged.RemoveListener(SelectSMAAQuality);
        antiAliasingTAAQualityDropdown.TmpDropdown.onValueChanged.RemoveListener(SelectTAAQuality);
        antiAliasingTAASharpen.onValueChangedFloat.RemoveListener(SelectTAASharpen);
    }

    public void SelectAAMode(int antiAliasingIndex)
    {
        aa.SetAA(antiAliasingIndex);
    }

    public void SelectSMAAQuality(int quality)
    {
        aa.SetSMAAQuality(quality);
    }

    public void SelectTAAQuality(int taaQuality)
    {
        aa.SetTAAQuality(taaQuality);
    }

    public void SelectTAASharpen(float sharpen)
    {
        aa.SetTAASharpening(sharpen);
    }

    internal void UpdateVisuals()
    {
        antiAliasingDropdown.TmpDropdown.SetValueWithoutNotify((int)aa.GetAAMode());
        antiAliasingDropdown.TmpDropdown.onValueChanged.Invoke(antiAliasingDropdown.TmpDropdown.value);

        antiAliasingSMAAQualityDropdown.TmpDropdown.SetValueWithoutNotify((int)aa.GetSMAAQuality());
        antiAliasingSMAAQualityDropdown.TmpDropdown.onValueChanged.Invoke(antiAliasingDropdown.TmpDropdown.value);

        antiAliasingTAAQualityDropdown.TmpDropdown.SetValueWithoutNotify((int)aa.GetTAAQuality());
        antiAliasingTAAQualityDropdown.TmpDropdown.onValueChanged.Invoke(antiAliasingDropdown.TmpDropdown.value);

        antiAliasingTAASharpen.SetValue(aa.GetTAASharpening());
        antiAliasingTAASharpen.ForceNotify();
    }
}
