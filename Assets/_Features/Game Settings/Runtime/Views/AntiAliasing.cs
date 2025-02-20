using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AntiAliasing : SettingsModule<AntiAliasing, AntiAliasingSaveModule>
{
    private Camera[] _cameras;

    public AntiAliasing(SettingsSaveModule settingsSaveModule) : base(settingsSaveModule)
    { }

    public override void Initialize()
    {

    }

    public override void Dispose() => throw new NotImplementedException();

    public void LoadCameras()
    {
        Camera.GetAllCameras(_cameras);
    }

    public void SetAA(int antiAliasingIndex)
    {
        LoadCameras();

        foreach (Camera camera in _cameras)
        {
            UniversalAdditionalCameraData universalAdditionalCameraData = camera.GetUniversalAdditionalCameraData();
            SetAAMode((AntialiasingMode)antiAliasingIndex, universalAdditionalCameraData);
        }
    }

    public void SetSMAAQuality(int quality)
    {
        LoadCameras();

        foreach (Camera camera in _cameras)
        {
            UniversalAdditionalCameraData universalAdditionalCameraData = camera.GetUniversalAdditionalCameraData();
            SetSMAAQuality((AntialiasingQuality)quality, universalAdditionalCameraData);
        }
    }

    public void SetTAAQuality(int taaQualityIndex, float sharpening)
    {
        LoadCameras();

        foreach (Camera camera in _cameras)
        {
            UniversalAdditionalCameraData universalAdditionalCameraData = camera.GetUniversalAdditionalCameraData();
            SetTAAQuality((TemporalAAQuality)taaQualityIndex, universalAdditionalCameraData);
        }
    }

    public void SetTAAShapening(float sharpening)
    {
        LoadCameras();

        foreach (Camera camera in _cameras)
        {
            UniversalAdditionalCameraData universalAdditionalCameraData = camera.GetUniversalAdditionalCameraData();
            SetTAASharpening(sharpening, universalAdditionalCameraData);
        }
    }

    private void SetAAMode(AntialiasingMode antialiasingMode, UniversalAdditionalCameraData universalAdditionalCameraData)
    {
        universalAdditionalCameraData.antialiasing = antialiasingMode;
    }

    private void SetSMAAQuality(AntialiasingQuality antialiasingQuality, UniversalAdditionalCameraData universalAdditionalCameraData)
    {
        universalAdditionalCameraData.antialiasingQuality = antialiasingQuality;
    }

    private void SetTAAQuality(TemporalAAQuality taaQaulity, UniversalAdditionalCameraData universalAdditionalCameraData)
    {
        universalAdditionalCameraData.taaSettings.quality = taaQaulity;
    }

    private void SetTAASharpening(float sharpening, UniversalAdditionalCameraData universalAdditionalCameraData)
    {
        universalAdditionalCameraData.taaSettings.contrastAdaptiveSharpening = sharpening;
    }

    public AntialiasingMode GetAAMode()
    {
        return (AntialiasingMode)_settingsSaveModule.antiAliasingMode;
    }

    public AntialiasingQuality GetSMAAQuality()
    {
        return (AntialiasingQuality)_settingsSaveModule.antiAliasingQuality;
    }

    public TemporalAAQuality GetTAAQuality()
    {
        return (TemporalAAQuality)_settingsSaveModule.antiAliasingTAAQuality;
    }

    public float GetTAASharpening()
    {
        return _settingsSaveModule.antiAliasingTAASharpening;
    }
}
