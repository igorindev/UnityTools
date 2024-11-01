using UnityEngine;

namespace AudioSubsystem
{
    public class AudioPlayer : MonoBehaviour
    {
        private AudioSource audioSource;
        private Transform audioParent;
        private AudioData audioData;
        private AudioGroupWrapper group;
        private uint audioLayer;
        private bool inUse;
        private float audioDataPitch;
        private float audioBeginPlayTime;
        private int audioTotalSamples;
        private float lastTimeScale;
        private bool isPaused;

        private int currentTimeSamples;
        private bool currentLoop;
        private float currentPitch;
        private float currentVolume;

        public bool InUse => inUse;

        public uint AudioLayer => audioLayer;

        internal void Setup(Transform parent)
        {
            transform.SetParent(parent);
            audioParent = parent;
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void OnDestroy()
        {
            if (audioData)
            {
                Stop();
            }
        }

        internal void Play(AudioData audioData, uint layer, AudioGroupWrapper group, Vector3 localPosition, Transform parent, int listenPriority)
        {
            if (parent)
            {
                transform.SetParent(parent);
            }

            transform.localPosition = localPosition;

            Play(audioData, layer, group, listenPriority);
        }

        internal void Play(AudioData audioData, uint layer, AudioGroupWrapper group, int listenPriority)
        {
            this.audioData = audioData;
            AudioClip clip = audioData.Clip;

            audioSource.clip = clip;
            audioSource.outputAudioMixerGroup = audioData.AudioGroup.OutputAudioMixerGroup;
            audioSource.spatialBlend = audioData.Is2D ? 0 : 1;
            audioSource.loop = audioData.Loop;
            audioSource.volume = audioData.Volume;
            audioSource.pitch = audioData.Pitch;

            audioSource.rolloffMode = AudioRolloffMode.Custom;
            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, audioData.VolumeRolloff);
            audioSource.maxDistance = audioData.MaxDistance;

            audioSource.priority = listenPriority;

            audioSource.Play();

            audioLayer = layer;
            audioBeginPlayTime = Time.time;
            audioTotalSamples = clip.samples;
            audioDataPitch = audioData.Pitch;
            inUse = true;

            currentTimeSamples = -1;
            currentVolume = audioData.Volume;
            currentLoop = audioData.Loop;
            lastTimeScale = 0;
            UpdatePitch();

            group.OnAudioGroupPaused += HandlePauseAudio;
        }

        private void HandlePauseAudio(bool value)
        {
            if (value)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.UnPause();
            }

            isPaused = value;
        }

        internal bool Tick(float deltaTime)
        {
            if (inUse == false)
            {
                return true;
            }

            UpdatePitch();

            if (audioSource.timeSamples > 0)
            {
                currentTimeSamples = audioSource.timeSamples;
            }

            return IsAudioFinished();
        }

        private bool IsAudioFinished()
        {
            if (inUse == false)
            {
                return true;
            }

            if (!audioData.Loop && (currentTimeSamples == audioTotalSamples || currentTimeSamples == 0))
            {
                Stop();

                return true;
            }

            return false;
        }

        internal void Stop()
        {
            audioSource.Stop();
            transform.SetParent(audioParent);
            inUse = false;

            group.OnAudioGroupPaused -= HandlePauseAudio;
            group = null;
            audioData = null;
        }

        internal void RestoreAudioToLastCachedSample()
        {
            audioSource.Play();
            audioSource.timeSamples = currentTimeSamples == -1 ? 0 : currentTimeSamples;
            audioSource.volume = currentVolume;
            audioSource.pitch = currentPitch;
            audioSource.loop = currentLoop;

            if (isPaused)
            {
                audioSource.Pause();
            }
        }

        private void UpdatePitch()
        {
            if (audioData.AudioGroup.UseTimeScale && lastTimeScale != Time.timeScale)
            {
                currentPitch = audioSource.pitch = audioDataPitch * Time.timeScale;
                lastTimeScale = Time.timeScale;
            }
        }
    }
}