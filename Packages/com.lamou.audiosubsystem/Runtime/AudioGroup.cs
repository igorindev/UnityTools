using System;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioGroup", menuName = "ScriptableObjects/Audio/Audio Group")]
public class AudioGroup : ScriptableObject
{
    [SerializeField] private AudioMixerGroup outputAudioMixerGroup;
    [SerializeField] private bool timeScaleDependent;

    public AudioMixerGroup OutputAudioMixerGroup => outputAudioMixerGroup;

    public bool UseTimeScale => timeScaleDependent;

    public event Action<bool> OnAudioPaused;

    public void PauseGroup(bool value)
    {
        OnAudioPaused?.Invoke(value);
    }
}
