using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Game Settings", menuName = "ScriptableObjects/Game Setting")]
public class GameSettings : ScriptableObject
{
    [Header("Camera")]
    public bool invertX;
    public bool invertY;
    public float sensivity = 100;

    [Header("Resolution")]
    public int width;
    public int height;
    public FullScreenMode screenMode;

    [Header("Graphics")]
    public int frameRate = 60;
    public int qualityLevel;
    public int qualityShadow;
    public VSync vSync;

    [Header("Config shadows properties")]
    public SettingsShadow[] settingsShadow;

    [Header("Audio")]
    [Range(0,1)] public float mainVolume = 1;
    [Range(0,1)] public float musicVolume = 0.5f;
    [Range(0,1)] public float voiceVolume = 0.5f;
    [Range(0,1)] public float soundEffectVolume = 0.5f;

    public void ApplySettings() 
    {
        Screen.SetResolution(width, height, screenMode);
        Application.targetFrameRate = frameRate;
        settingsShadow[qualityShadow].ApplySettings();
        QualitySettings.SetQualityLevel(qualityLevel);
        QualitySettings.vSyncCount = (int)vSync;
    }

    public void InvertCameraHorizontal(string option) 
    {
        if (option == "Yes") {
            invertX = true;
        } else if (option == "No") {
            invertX = false;
        }
    }
    public void InvertCameraVertical(string option) {
        if (option == "Yes") {
            invertY = true;
        } else if (option == "No") {
            invertY = false;
        }
    }
    public void SetValueSensivity(float value) 
    {
        sensivity = value;
    }
    public void ChangeResolution(string option) 
    {
        if (option == "1920x1080") 
        {
            width = 1920;
            height = 1080; 
        }
        else if (option == "800x600") {
            width = 800;
            height = 600;
        }
    }
    public void ChangeScreenMode(string option) 
    {
        if (option == "Window") {
            screenMode = FullScreenMode.Windowed;
        } else if (option == "Fullscreen") {
            screenMode = FullScreenMode.FullScreenWindow;
        }
    }


    public enum QualityLevel
    {
        Low,
        Meddium,
        High,
        Ultra
    }
    public enum FrameRate
    {
        Thirty,
        Six,
        Unlimited,
    }
    /// <summary>
    /// More Lower the value more stable FPS and more Higher the value, faster the response (but some frames get discarted). 
    /// </summary>
    public enum VSync
    {
        DontSync = 0,
        DoubleBuffer = 1,
        TrippleBuffer = 2,
        QuadrupleBuffer = 3,
        QuintupleBuffer = 4,
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

    public void ApplySettings() 
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
