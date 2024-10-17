using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    private bool inUse;
    AudioSource audioSource;
    Transform audioParent;
    AudioData audioData;
    float audioDataPitch;
    float audioBeginPlayTime;
    int audioTotalSamples;
    int audioCurrentSamplesPlayback;
    float lastTimeScale;

    public bool InUse => inUse;

    internal void Setup(Transform parent)
    {
        transform.SetParent(parent);
        audioParent = parent;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        if (audioData)
        {
            Stop();
        }
    }

    internal void Play(AudioData audioData, Vector3 localPosition, Transform parent)
    {
        if (parent)
        {
            transform.SetParent(parent);
        }

        transform.localPosition = localPosition;

        Play(audioData);
    }

    internal void Play(AudioData audioData)
    {
        this.audioData = audioData;
        AudioClip clip = audioData.Clip;

        audioSource.clip = clip;
        audioSource.outputAudioMixerGroup = audioData.AudioGroup.OutputAudioMixerGroup;
        audioSource.minDistance = 4f;
        audioSource.spatialBlend = audioData.Is2D ? 0 : 1;
        audioSource.loop = audioData.Loop;
        audioSource.volume = audioData.Volume;
        audioSource.pitch = audioData.Pitch;

        audioSource.Play();

        audioBeginPlayTime = Time.time;
        audioTotalSamples = clip.samples;
        audioCurrentSamplesPlayback = -1;
        audioDataPitch = audioData.Pitch;
        inUse = true;
        lastTimeScale = Time.timeScale;

        audioData.AudioGroup.OnAudioPaused += HandlePauseAudio;
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
            audioCurrentSamplesPlayback = audioSource.timeSamples;
        }
        
        return IsAudioFinished();
    }

    private bool IsAudioFinished()
    {
        if (inUse == false)
        {
            return true;
        }

        if (!audioData.Loop && (audioCurrentSamplesPlayback == audioTotalSamples || audioCurrentSamplesPlayback == 0))
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

        audioData.AudioGroup.OnAudioPaused -= HandlePauseAudio;
        audioData = null;
    }

    internal void RestoreAudioToLastCachedSample()
    {
        audioSource.Play();
        audioSource.timeSamples = audioCurrentSamplesPlayback == -1 ? 0 : audioCurrentSamplesPlayback;
    }

    private void UpdatePitch()
    {
        if (audioData.AudioGroup.UseTimeScale && lastTimeScale != Time.timeScale)
        {
            audioSource.pitch = audioDataPitch * Time.timeScale;
            lastTimeScale = Time.timeScale;
        }
    }
}