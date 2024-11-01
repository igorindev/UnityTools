using UnityEngine;
using System;

namespace AudioSubsystem
{
    [CreateAssetMenu(fileName = "AudioData.asset", menuName = "AudioSubsystem/AudioData")]
    public class AudioData : ScriptableObject
    {
        [SerializeField] private AudioGroup audioGroup;
        [SerializeField] private AudioClip clip;

        [Space(5)]
        [Header("Volume")]
        [SerializeField][Range(0, 1)] private float volume = 1f;
        [SerializeField][AudioMinMaxSlider(0, 3)] private Vector2 pitchVariation = Vector2.one;

        [Header("Conditions")]
        [SerializeField] private bool loop;
        [SerializeField] private bool is2D;

        [Header("3D Audio Settings")]
        [SerializeField] private AnimationCurve volumeRolloff;
        [SerializeField] private float maxDistance = 100;

        [Header("Audio Repetition")]
        [SerializeField] private bool isFrequentAudio;
        [SerializeField] private int maxSimultaneousAudios = 5;

        public AudioClip Clip => clip;
        public AudioGroup AudioGroup => audioGroup;
        public float Volume => volume;
        public float Pitch => GetRandomInRange(pitchVariation);
        public bool Loop => loop;
        public bool Is2D => is2D;
        public bool IsFrequentAudio => isFrequentAudio;
        public AnimationCurve VolumeRolloff => volumeRolloff;
        public float MaxDistance => maxDistance;
        public int MaxSimultaneousAudios => maxSimultaneousAudios;

        public float GetRandomInRange(Vector2 range)
        {
            return UnityEngine.Random.Range(range.x, range.y);
        }
    }
}
