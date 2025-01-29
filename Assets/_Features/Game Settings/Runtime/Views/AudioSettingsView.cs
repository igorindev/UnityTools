using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsView : MonoBehaviour
{
    [SerializeField] Image musicVolumeSlider;
    [SerializeField] Image effectsVolumeSlider;
    [SerializeField] Toggle muteToggle;

    protected static bool initialized;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        musicVolumeSlider.fillAmount = AudioSettings.GetMusicVolume() / 1;
        effectsVolumeSlider.fillAmount = AudioSettings.GetEffectVolume() / 1;
        muteToggle.SetIsOnWithoutNotify(AudioSettings.GetMute());
    }

    public void SetMusicVolume(float value)
    {
        AudioSettings.SetMusicVolume(((value * 100) * (1 - 0.0001f) / 100) + 0.0001f);
    }
    public void SetEffectVolume(float value)
    {
        AudioSettings.SetEffectVolume(((value * 100) * (1 - 0.0001f) / 100) + 0.0001f);
    }
    public void SetMute(bool value)
    {
        AudioSettings.SetMute(value);
    }

    //Example:
    //mainVolumeSlider.onValueChanged.AddListener((v) => SliderUpdateInputField(mainVolumeInput, v));
    //mainVolumeInput.onEndEdit.AddListener((v) => InputFieldUpdateSlider(mainVolumeInput, mainVolumeSlider, v));
    private void SliderUpdateInputField(TMP_InputField input, float value)
    {
        input.SetTextWithoutNotify((value * 100).ToString("0"));
    }

    private void InputFieldUpdateSlider(TMP_InputField input, Slider slider, string value)
    {
        if (int.TryParse(value, out int v))
        {
            if (v > 100)
                input.SetTextWithoutNotify("100");
            else if (v < 0)
                input.SetTextWithoutNotify("0");

            slider.SetValueWithoutNotify((v * (1 - 0.0001f) / 100) + 0.0001f);
        }
        else
        {
            SliderUpdateInputField(input, slider.value);
        }
    }

    public void FillOneCell(Image imageToFill)
    {
        imageToFill.fillAmount += 0.1f;
    }

    public void MinusOneCell(Image imageToFill)
    {
        imageToFill.fillAmount -= 0.1f;
    }
}
