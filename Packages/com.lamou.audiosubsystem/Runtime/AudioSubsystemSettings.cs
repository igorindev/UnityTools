using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioSubsystemSettings.asset", menuName = "AudioSubsystem/AudioSubsystemSettings")]
public class AudioSubsystemSettings : ScriptableObject
{
    [Min(1)] public int numOfDefaultAudioSources = 32;
    public AudioMixer audioMixer;
    public AudioGroup[] audioGroups;
    public SerializableDictionary<string, AudioData> audios;
}