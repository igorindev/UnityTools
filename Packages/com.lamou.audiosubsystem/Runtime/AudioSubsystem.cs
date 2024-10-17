using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSubsystem : MonoBehaviour
{
    [SerializeField, Min(1)] int numOfAudioSources = 32;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioGroup[] groups;

    [SerializeField] SerializableDictionary<string, AudioData> audioList;

    private List<AudioPlayer> activeAudioPool;
    private Queue<AudioPlayer> availableAudioPool;

    private string currentSnapshot;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void InitializeSubsystem()
    {
        GameObject instance = new GameObject("AudioSubsystem");
        AudioSubsystem audioSubsystem = instance.AddComponent<AudioSubsystem>();

        if (!Audio.RegisterAudioSubsystem(audioSubsystem))
        {
            Destroy(instance);
            return;
        }

        audioSubsystem.Setup();
        DontDestroyOnLoad(instance);
    }

    private void Setup()
    {
        numOfAudioSources = 32;
        audioMixer = null;
        groups = null;
        audioList = null;

        currentSnapshot = audioMixer.FindSnapshot("Snapshot").name;

        availableAudioPool = new(numOfAudioSources);
        activeAudioPool = new(numOfAudioSources);

        for (int i = 0; i < numOfAudioSources; i++)
        {
            availableAudioPool.Enqueue(CreateAudioPlayer());
        }

        AudioSettings.OnAudioConfigurationChanged += AudioSettings_OnAudioConfigurationChanged;
    }

    private void Update()
    {
        ValidateAudioPlayers();
    }

    private void OnDestroy()
    {
        AudioSettings.OnAudioConfigurationChanged -= AudioSettings_OnAudioConfigurationChanged;
    }

    private void ValidateAudioPlayers()
    {
        for (int i = activeAudioPool.Count - 1; i >= 0; i--)
        {
            if (activeAudioPool[i].Tick(Time.deltaTime))
            {
                availableAudioPool.Enqueue(activeAudioPool[i]);
                activeAudioPool.RemoveAt(i);
            }
        }
    }

    public AudioPlayer Play(string name)
    {
        if (!TryPlayAudio(name, out AudioData audioData))
        {
            return null;
        }

        AudioPlayer audioEntity = GetFirstAudioSourceAvailable();
        audioEntity.Play(audioData);

        return audioEntity;
    }

    public AudioPlayer PlayAtPosition(string name, Vector3 position, Transform followTarget)
    {
        if (!TryPlayAudio(name, out AudioData audioData))
        {
            return null;
        }

        AudioPlayer audioEntity = GetFirstAudioSourceAvailable();
        audioEntity.Play(audioData, position, followTarget);

        return audioEntity;
    }

    public void Stop(AudioPlayer audioPlayer)
    {
        audioPlayer.Stop();
    }

    private bool TryPlayAudio(string name, out AudioData audioData)
    {
        if (!audioList.ContainsKey(name))
        {
            Debug.LogError("No clip found with name: " + name);
            audioData = null;
            return false;
        }

        audioData = audioList[name];

        return true;
    }

    private AudioPlayer GetFirstAudioSourceAvailable()
    {
        if (availableAudioPool.Count == 0)
        {
            var audioPlayer = CreateAudioPlayer();
            availableAudioPool.Enqueue(audioPlayer);
        }

        AudioPlayer audioSourceEntity = availableAudioPool.Dequeue();
        activeAudioPool.Add(audioSourceEntity);
        return audioSourceEntity;
    }

    private void AudioSettings_OnAudioConfigurationChanged(bool deviceWasChanged)
    {
        TransitionAudioSnapshot(currentSnapshot, 0);

        foreach (AudioPlayer item in activeAudioPool)
        {
            item.RestoreAudioToLastCachedSample();
        }
    }

    public void TransitionAudioSnapshot(string snapshotName, float transitionTime)
    {
        var snapshot = audioMixer.FindSnapshot(snapshotName);
        snapshot.TransitionTo(transitionTime);

        currentSnapshot = snapshot.name;
    }

    public void TransitionAudioSnapshot(AudioMixerSnapshot snapshot, float transitionTime)
    {
        snapshot.TransitionTo(transitionTime);
        currentSnapshot = snapshot.name;
    }

    [ContextMenu("pause")]
    public void PauseGroup()
    {
        groups[0].PauseGroup(true);
    }

    [ContextMenu("unpause")]
    public void UnPauseGroup()
    {
        groups[0].PauseGroup(false);
    }

    private AudioPlayer CreateAudioPlayer()
    {
        GameObject instance = new GameObject("AudioPlayer");
        AudioPlayer audioPlayer = instance.AddComponent<AudioPlayer>();
        audioPlayer.Setup(transform);
        return audioPlayer;
    }
}