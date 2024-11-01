using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSubsystem
{
    [CreateAssetMenu(fileName = "AudioSubsystemSettings.asset", menuName = "AudioSubsystem/AudioSubsystemSettings")]
    public class AudioSubsystemSettings : ScriptableObject
    {
        [SerializeField][Min(1)] private int numOfDefaultAudioSources = 32;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioGroup[] audioGroups;
        [SerializeField] List<AudioData> audios;

        public int NumOfDefaultAudioSources { get => numOfDefaultAudioSources; set => numOfDefaultAudioSources = value; }
        public AudioMixer AudioMixer { get => audioMixer; set => audioMixer = value; }
        public AudioGroup[] AudioGroups { get => audioGroups; set => audioGroups = value; }
        public List<AudioData> Audios { get => audios; set => audios = value; }
    }
}
