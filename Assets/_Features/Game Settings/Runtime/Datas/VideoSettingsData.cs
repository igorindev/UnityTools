using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public struct VideoSettingsData : ISettingsSaveData
{
    public int resolutionWidth;
    public int resolutionHeight;

    public uint resolutionRefreshRateNumerator;
    public uint resolutionRefreshRateDenominator;

    public int screenMode;

    public string displayWindow;

    public int vSync;
    public int frameRate;

    public int qualityLevelGraphic;
    public int qualityLevelShadow;

    public int antiAliasingMode;
    public int antiAliasingQuality;
    public int antiAliasingTAAQuality;
    public int antiAliasingTAASharpening;

    public int antiAliasingHardwareMode;

    public List<SettingsSaveModule> settingsSaveModule;

    public void BuildData(SettingsSaveModule[] settingsSaveModules)
    {
        settingsSaveModule = settingsSaveModules.ToList();
    }
}

[Serializable]
public class SettingsSaveModule
{

}

[Serializable]

public class AntiAliasingSaveModule : SettingsSaveModule
{
    public int antiAliasingMode = 0;
    public int antiAliasingQuality = 1;
    public int antiAliasingTAAQuality = 3;
    public float antiAliasingTAASharpening = 0.3f;

    public int antiAliasingHardwareMode;
}

[Serializable]

public class Test : SettingsSaveModule
{
    public int sdasdadas = 231321;
}
