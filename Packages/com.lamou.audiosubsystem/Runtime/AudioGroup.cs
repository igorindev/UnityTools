using UnityEngine;
using UnityEngine.Audio;

namespace AudioSubsystem
{
    [CreateAssetMenu(fileName = "AudioGroup.asset", menuName = "AudioSubsystem/AudioGroup")]
    public class AudioGroup : ScriptableObject
    {
        [SerializeField] private AudioMixerGroup outputAudioMixerGroup;
        [SerializeField] private bool timeScaleDependent;

        public string Name => outputAudioMixerGroup.name;
        public AudioMixerGroup OutputAudioMixerGroup => outputAudioMixerGroup;
        public bool UseTimeScale => timeScaleDependent;
    }
}