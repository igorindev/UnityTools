using System;

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
}
