using UnityEngine;
using UnityEngine.Audio;

public static class AudioSettings
{
    public const string MAIN_VOLUME = "Main";
    public const string MUSIC_VOLUME = "Music";
    public const string EFFECT_VOLUME = "Effect";

    public static AudioMixer activeAudioMixer;

    public static void Initalize(AudioMixer audioMixer)
    {
        //Load values
        activeAudioMixer = audioMixer;

        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_VOLUME, GetMusicVolume()));
        SetEffectVolume(PlayerPrefs.GetFloat(EFFECT_VOLUME, GetEffectVolume()));
    }
    public static void Save()
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME, GetMusicVolume());
        PlayerPrefs.SetFloat(EFFECT_VOLUME, GetEffectVolume());
    }

    //0.00001 -> 1
    public static void SetMainVolume(float value)
    {
        activeAudioMixer.SetFloat(MAIN_VOLUME, Mathf.Log10(value) * 20);
        Save();
    }
    public static void SetMusicVolume(float value)
    {
        activeAudioMixer.SetFloat(MUSIC_VOLUME, Mathf.Log10(value) * 20);
        Save();
    }
    public static void SetEffectVolume(float value)
    {
        activeAudioMixer.SetFloat(EFFECT_VOLUME, Mathf.Log10(value) * 20);
        Save();
    }
    public static void SetMute(bool value)
    {
        AudioListener.volume = value ? 0 : 1;
        Save();
    }

    public static float GetMainVolume() { activeAudioMixer.GetFloat(MAIN_VOLUME, out float value); return Mathf.Pow(10, value / 20); }
    public static float GetMusicVolume() { activeAudioMixer.GetFloat(MUSIC_VOLUME, out float value); return Mathf.Pow(10, value / 20); }
    public static float GetEffectVolume() { activeAudioMixer.GetFloat(EFFECT_VOLUME, out float value); return Mathf.Pow(10, value / 20); }
    public static bool GetMute() => AudioListener.volume == 0;
}
