using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AntiAliasing : SettingsModule
{
    private Camera[] _cameras;

    public override void Initialize() => throw new NotImplementedException();
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
            SetTAAQuality((TemporalAAQuality)taaQualityIndex, sharpening, universalAdditionalCameraData);
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

    private void SetTAAQuality(TemporalAAQuality taaQaulity, float sharpening, UniversalAdditionalCameraData universalAdditionalCameraData)
    {
        universalAdditionalCameraData.taaSettings.quality = taaQaulity;
        universalAdditionalCameraData.taaSettings.contrastAdaptiveSharpening = sharpening;
    }
}
