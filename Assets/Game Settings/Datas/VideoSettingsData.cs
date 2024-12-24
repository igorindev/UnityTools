using System;

[Serializable]
public struct VideoSettingsData : ISettingsSaveData
{
    public int resolutionIndex;
    public int resolutionWidth;
    public int resolutionHeight;
    public uint resolutionRefreshRateNumerator;
    public uint resolutionRefreshRateDenominator;

    public int screenMode;

    public int displayWindowIndex;

    public int vSync;
    public int frameRate;
    public int qualityLevelGraphic;
    public int qualityLevelShadow;
}
