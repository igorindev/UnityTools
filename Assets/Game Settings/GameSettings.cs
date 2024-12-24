using UnityEngine;

[CreateAssetMenu(fileName = "Game Settings", menuName = "ScriptableObjects/Game Setting")]
public class GameSettings : ScriptableObject
{
    [Header("Camera")]
    public bool invertX;
    public bool invertY;
    public float sensivity = 1;

    [Header("Resolution")]
    public int width;
    public int height;
    public FullScreenMode screenMode;

    [Header("Config shadows properties")]
    public SettingsShadow[] settingsShadow;

    [Header("Audio")]
    [Range(0, 1)] public float mainVolume = 1;
    [Range(0, 1)] public float musicVolume = 0.5f;
    [Range(0, 1)] public float voiceVolume = 0.5f;
    [Range(0, 1)] public float soundEffectVolume = 0.5f;

    public void ApplySettings()
    {
        Screen.SetResolution(width, height, screenMode);
        //Application.targetFrameRate = frameRate;
        //settingsShadow[qualityShadow].ApplySettings();
        //QualitySettings.SetQualityLevel((int)qualityLevel);
        //QualitySettings.vSyncCount = (int)vSync;
    }

    public void InvertCameraHorizontal(string option)
    {
        if (option == "Yes")
        {
            invertX = true;
        }
        else if (option == "No")
        {
            invertX = false;
        }
    }
    public void InvertCameraVertical(string option)
    {
        if (option == "Yes")
        {
            invertY = true;
        }
        else if (option == "No")
        {
            invertY = false;
        }
    }

    public void SetValueSensivity(float value)
    {
        sensivity = value;
    }
}

[System.Serializable]
public struct SettingsShadow
{
    public ShadowQuality shadowQuality;
    public ShadowResolution shadowResolution;
    public ShadowProjection shadowProjection;
    public ShadowmaskMode shadowMaskMode;
    public float shadowDistance;
    public float shadowNearPlaneOffset;
    public int shadowCascades;
    public float shadowCascade2Split;
    public Vector3 shadowCascade4Split;

    public readonly void ApplySettings()
    {
        QualitySettings.shadows = shadowQuality;
        QualitySettings.shadowResolution = shadowResolution;
        QualitySettings.shadowProjection = shadowProjection;
        QualitySettings.shadowmaskMode = shadowMaskMode;
        QualitySettings.shadowDistance = shadowDistance;
        QualitySettings.shadowNearPlaneOffset = shadowNearPlaneOffset;
        QualitySettings.shadowCascades = shadowCascades;
        QualitySettings.shadowCascade2Split = shadowCascade2Split;
        QualitySettings.shadowCascade4Split = shadowCascade4Split;
    }
}
