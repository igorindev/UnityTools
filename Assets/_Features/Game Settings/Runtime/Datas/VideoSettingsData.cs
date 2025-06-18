using System;
using System.Collections.Generic;

[Serializable]
public class VideoSettingsData : ISettingsSaveData
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

    public int antiAliasingHardwareMode;

    public Dictionary<Type, SettingsSaveModule> settingsSaveModule;
}

[Serializable]
public class SettingsSaveModule
{
    private string _moduleName;
    public string ModuleName { get => GetType().FullName; set => _moduleName = value; }
}

[Serializable]

public class AntiAliasingModuleSave : SettingsSaveModule
{
    public int antiAliasingMode = 0;
    public int antiAliasingQuality = 2;

    public int antiAliasingTAAQuality = 3;
    public float antiAliasingTAASharpening = 0.3f;

    public int antiAliasingHardwareMode;
}

[Serializable]

public class Test : SettingsSaveModule
{
    public int sdasdadas = 231321;
}
