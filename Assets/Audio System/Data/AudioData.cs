using UnityEngine;
using UnityEngine.Audio;
using System;

[CreateAssetMenu(fileName = "AudioData", menuName = "ScriptableObjects/Audio/Audio Data")]
public class AudioData : ScriptableObject
{
    public AudioMixerGroup outputAudioMixerGroup;

    [Space(5)]

    [Header("Modifiers")]
    [SerializeField, ReadOnly] int currentCall = 0;
    [SerializeField] OnNextCall[] nextCall = new OnNextCall[1];

    [Header("Timers")]
    public float minTimeBetweenCall = 0;
    [SerializeField, Range(0, 100)] float percentageToNext = 100;

    [Header("Conditions")]
    public bool loop;
    public bool _2D;

    [Space(8)]
    public AudioClip[] clips;

    public float LastTimePlayed { get; set; }
    public int PlayedClip { get; set; }
    public bool BeingPlayer 
    {
        get { return Time.time - LastTimePlayed > clips[PlayedClip].length; }
    }

    void OnValidate()
    {
        nextCall = nextCall.Length == 0 ? new OnNextCall[1] : nextCall;
    }

    public void GetConfigs(out float volume, out float pitch)
    {
        //If audio is not finished (modified by percentage that means an end)
        if (Time.time - LastTimePlayed < (clips[PlayedClip].length * percentageToNext * 0.01f))
        {
            currentCall += 1;
            if (currentCall > nextCall.Length - 1) 
                currentCall = nextCall.Length - 1;
        }
        else
        {
            currentCall = 0;
        }

        volume = nextCall[currentCall].volume;
        pitch = nextCall[currentCall].pitchVariation.GetRandomInRange();
    }

    public void ResetCalls()
    {
        currentCall = 0;
    }

    [Serializable]
    public class OnNextCall
    {
        [Range(0, 1)] public float volume = 1f;
        [MinMaxSlider(0, 3)] public Vector2 pitchVariation = Vector2.one;
    }
}
