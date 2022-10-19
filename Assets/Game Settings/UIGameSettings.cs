using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIGameSettings : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;
    [SerializeField] Image musicVolumeSlider;
    [SerializeField] Image effectsVolumeSlider;
    [SerializeField] Toggle muteToggle;

    [Header("Video")]
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown screenModeDropdown;

    protected static bool initialized;

    void Awake()
    {
        if (initialized == false)
        {
            initialized = true; //Enter here only one time per game launch

            AudioSettings.Initalize(audioMixer);
            VideoSettings.Initalize();
        }

        Initialize();
        LoadSettings();
        StartCoroutine(WindowsFullscreenShortcutHandler());
    }

    IEnumerator WindowsFullscreenShortcutHandler()
    {
        bool pressed = false;
        while (true)
        {
            if (pressed == false && (Keyboard.current.altKey.isPressed && Keyboard.current.enterKey.isPressed))
            {
                pressed = true;
                Debug.Log(Screen.currentResolution + " | " + Screen.fullScreenMode);
                yield return null;
                Debug.Log(Screen.currentResolution + " | " + Screen.fullScreenMode);
                VideoSettings.SetNewResolutionAndScreenModeIndex(Screen.currentResolution, Screen.fullScreenMode);

                LoadSettings();
            }

            if (Keyboard.current.altKey.wasReleasedThisFrame || Keyboard.current.enterKey.wasReleasedThisFrame)
            {
                pressed = false;
            }

            yield return null;
        }
    }

    void Initialize()
    {
        resolutionDropdown.AddOptions(VideoSettings.resolutionsNames.ToList());

        musicVolumeSlider.fillAmount = AudioSettings.GetMusicVolume() / 1;
        effectsVolumeSlider.fillAmount = AudioSettings.GetEffectVolume() / 1;
        muteToggle.SetIsOnWithoutNotify(AudioSettings.GetMute());
    }
    public void LoadSettings()
    {
        resolutionDropdown.SetValueWithoutNotify(VideoSettings.currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();
        screenModeDropdown.SetValueWithoutNotify(VideoSettings.screenModeIndex);
        screenModeDropdown.RefreshShownValue();
        //Debug.Log("Loaded to: " + VideoSettings.currentResolutionIndex + ": " + VideoSettings.GetCurrentSettedResolution() + " | " + VideoSettings.screenModeIndex + ": " + VideoSettings.GetScreenMode());
    }

    public void CancelVideoChanges()
    {
        VideoSettings.CancelVideoChanges();
    }

    public void SetMusicVolume(Image value)
    {
        AudioSettings.SetMusicVolume(((value.fillAmount * 100) * (1 - 0.0001f) / 100) + 0.0001f);
    }
    public void SetEffectVolume(Image value)
    {
        AudioSettings.SetEffectVolume((value.fillAmount * (1 - 0.0001f) / 1) + 0.0001f);
    }
    public void SetMute(bool value)
    {
        AudioSettings.SetMute(value);
    }

    public void SetResolution(int value)
    {
        VideoSettings.SetResolution(value);
    }
    public void SetScreenMode(int value)
    {
        VideoSettings.SetScreenMode(value);
    }

    public void SetScreenModeAndResolution()
    {
        VideoSettings.ApplyScreen();
    }

    //Example:
    //mainVolumeSlider.onValueChanged.AddListener((v) => SliderUpdateInputField(mainVolumeInput, v));
    //mainVolumeInput.onEndEdit.AddListener((v) => InputFieldUpdateSlider(mainVolumeInput, mainVolumeSlider, v));
    void SliderUpdateInputField(TMP_InputField input, float value)
    {
        input.SetTextWithoutNotify((value * 100).ToString("0"));
    }
    void InputFieldUpdateSlider(TMP_InputField input, Slider s, string value)
    {
        if (int.TryParse(value, out int v))
        {
            if (v > 100)
                input.SetTextWithoutNotify("100");
            else if (v < 0)
                input.SetTextWithoutNotify("0");

            s.SetValueWithoutNotify((v * (1 - 0.0001f) / 100) + 0.0001f);
        }
        else
        {
            SliderUpdateInputField(input, s.value);
        }
    }

    void OnApplicationQuit()
    {
        AudioSettings.Save();
        VideoSettings.Save();
    }
}