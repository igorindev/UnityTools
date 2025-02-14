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

    public List<ISettingsSaveModule> test;

    public void BuildData(ISettingsSaveModule[] settingsSaveModules)
    {
        test = settingsSaveModules.ToList();
    }
}

[Serializable]
public class ISettingsSaveModule
{

}

[Serializable]

public class aa : ISettingsSaveModule
{
    public int antiAliasingMode = 1;
    public int antiAliasingQuality = 2;
    public int antiAliasingTAAQuality = 321;
    public int antiAliasingTAASharpening = 231321;

    public int antiAliasingHardwareMode;
}

[Serializable]

public class bb : ISettingsSaveModule
{
    public int sdasdadas = 231321;
}
