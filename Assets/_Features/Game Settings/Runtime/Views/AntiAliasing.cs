using System;
using System.Collections.Generic;
using Modules;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AntiAliasing : Module<AntiAliasing, AntiAliasingSaveModule, VideoSettings>
{
    private readonly Dictionary<Camera, UniversalAdditionalCameraData> _cameras = new Dictionary<Camera, UniversalAdditionalCameraData>();

    public AntiAliasing(SettingsSaveModule settingsSaveModule) : base(settingsSaveModule)
    { }

    public override void Initialize()
    { }

    public override void Dispose()
    {
        _cameras.Clear();
    }

    public void AddCamera(Camera camera)
    {
        _cameras.Add(camera, camera.GetUniversalAdditionalCameraData());
    }

    public void RemoveCamera(Camera camera)
    {
        _cameras.Remove(camera);
    }

    private void SetAAConfigToCameras<T>(T antiAliasingIndex, Action<T, UniversalAdditionalCameraData> setAAConfig)
    {
        foreach (KeyValuePair<Camera, UniversalAdditionalCameraData> cameraData in _cameras)
        {
            setAAConfig(antiAliasingIndex, cameraData.Value);
        }
    }

    public void SetAA(int antiAliasingIndex) => SetAAConfigToCameras(antiAliasingIndex, SetAAMode);

    public void SetSMAAQuality(int quality) => SetAAConfigToCameras(quality, SetSMAAQuality);

    public void SetTAAQuality(int taaQualityIndex) => SetAAConfigToCameras(taaQualityIndex, SetTAAQuality);

    public void SetTAASharpening(float sharpening) => SetAAConfigToCameras(sharpening, SetTAASharpening);

    public AntialiasingMode GetAAMode() => (AntialiasingMode)_settingsSaveModule.antiAliasingMode;

    public AntialiasingQuality GetSMAAQuality() => (AntialiasingQuality)_settingsSaveModule.antiAliasingQuality;

    public TemporalAAQuality GetTAAQuality() => (TemporalAAQuality)_settingsSaveModule.antiAliasingTAAQuality;

    public float GetTAASharpening() => _settingsSaveModule.antiAliasingTAASharpening;

    private void SetAAMode(int antialiasingMode, UniversalAdditionalCameraData cameraData) => cameraData.antialiasing = (AntialiasingMode)antialiasingMode;

    private void SetSMAAQuality(int antialiasingQuality, UniversalAdditionalCameraData cameraData) => cameraData.antialiasingQuality = (AntialiasingQuality)antialiasingQuality;

    private void SetTAAQuality(int taaQaulity, UniversalAdditionalCameraData cameraData) => cameraData.taaSettings.quality = (TemporalAAQuality)taaQaulity;

    private void SetTAASharpening(float sharpening, UniversalAdditionalCameraData cameraData) => cameraData.taaSettings.contrastAdaptiveSharpening = sharpening;
}
