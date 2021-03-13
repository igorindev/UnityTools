using UnityEngine;

namespace DialogSystem
{
    [CreateAssetMenu(fileName = "AudioLanguage", menuName = "ScriptableObjects/Dialog/AudioClipsLanguage")]
    public class DialogAudioClip : ScriptableObject
    {
        [Tooltip("Set clips in file language order (starts in 0 to language index 1)")]
        [SerializeField] AudioClip[] audioClips;

        public AudioClip[] Clips { get => audioClips; }
    }
}