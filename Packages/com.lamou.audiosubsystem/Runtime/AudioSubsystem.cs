using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSubsystem
{
    public class AudioSubsystem : MonoBehaviour
    {
        public const string AudioSubsystemSettingsPath = "AudioSubsystem/AudioSubsystemSettings";
        private readonly Dictionary<uint, Queue<AudioPlayer>> audioLayers = new();

        private int numOfAudioSources;
        private AudioMixer audioMixer;

        private Dictionary<string, AudioGroupWrapper> groups = new();
        private Dictionary<string, AudioData> audioDatabase = new();
        private List<AudioPlayer> activeAudioPool = new();
        private Queue<AudioPlayer> availableAudioPool = new();

        private string currentSnapshot;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeSubsystem()
        {
            GameObject instance = new GameObject("AudioSubsystem");
            AudioSubsystem audioSubsystem = instance.AddComponent<AudioSubsystem>();

            if (!Audio.RegisterAudioSubsystem(audioSubsystem))
            {
                Destroy(instance);
                return;
            }

            audioSubsystem.Setup(Resources.Load(AudioSubsystemSettingsPath) as AudioSubsystemSettings);
            DontDestroyOnLoad(instance);
        }

        private void Setup(AudioSubsystemSettings audioSubsystemSettings)
        {
            numOfAudioSources = audioSubsystemSettings.NumOfDefaultAudioSources;
            audioMixer = audioSubsystemSettings.AudioMixer;
            BuildAudioDatabase(audioSubsystemSettings.Audios);

            currentSnapshot = audioMixer.FindSnapshot("Snapshot").name;

            availableAudioPool = new(numOfAudioSources);
            activeAudioPool = new(numOfAudioSources);

            for (int i = 0; i < numOfAudioSources; i++)
            {
                availableAudioPool.Enqueue(CreateAudioPlayer());
            }

            foreach (AudioGroup group in audioSubsystemSettings.AudioGroups)
            {
                groups.Add(group.Name, new AudioGroupWrapper(group));
            }

            AudioSettings.OnAudioConfigurationChanged += AudioSettings_OnAudioConfigurationChanged;
        }

        private void BuildAudioDatabase(List<AudioData> availableAudios)
        {
            audioDatabase = availableAudios.ToDictionary(audio => audio.name, audio => audio);
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
                    if (activeAudioPool[i].AudioLayer != 0 && audioLayers.TryGetValue(activeAudioPool[i].AudioLayer, out Queue<AudioPlayer> queue))
                    {
                        queue.Dequeue();
                    }

                    availableAudioPool.Enqueue(activeAudioPool[i]);
                    activeAudioPool.RemoveAt(i);
                }
            }
        }

        internal AudioPlayer PlayAtPosition(string name, Vector3 position, Transform followTarget, uint layer = 0, int listenPriority = 0)
        {
            if (!TryPlayAudio(name, out AudioData audioData))
            {
                return null;
            }

            AudioPlayer audioPlayer;
            if (layer != 0 && audioLayers.TryGetValue(layer, out Queue<AudioPlayer> audios) && audios.Count == audioData.MaxSimultaneousAudios)
            {
                audioPlayer = audioLayers[layer].Dequeue();
            }
            else
            {
                audioPlayer = GetFirstAudioSourceAvailable();
            }

            audioPlayer.Play(audioData, layer, groups[audioData.AudioGroup.Name], position, followTarget, listenPriority);

            if (layer != 0)
            {
                if (audioLayers.ContainsKey(layer))
                {
                    audioLayers[layer].Enqueue(audioPlayer);
                }
                else
                {
                    var FrequentAudioQueue = new Queue<AudioPlayer>();
                    FrequentAudioQueue.Enqueue(audioPlayer);

                    audioLayers.Add(layer, FrequentAudioQueue);
                }
            }

            return audioPlayer;
        }

        internal AudioPlayer Play(string name, uint layer = 0, int listenPriority = 0)
        {
            if (!TryPlayAudio(name, out AudioData audioData))
            {
                return null;
            }

            AudioPlayer audioPlayer;
            if (layer != 0 && audioLayers.TryGetValue(layer, out Queue<AudioPlayer> audios) && audios.Count == audioData.MaxSimultaneousAudios)
            {
                audioPlayer = audioLayers[layer].Dequeue();
            }
            else
            {
                audioPlayer = GetFirstAudioSourceAvailable();
            }

            audioPlayer.Play(audioData, layer, groups[audioData.AudioGroup.Name], listenPriority);

            if (layer != 0)
            {
                if (audioLayers.ContainsKey(layer))
                {
                    audioLayers[layer].Enqueue(audioPlayer);
                }
                else
                {
                    var FrequentAudioQueue = new Queue<AudioPlayer>();
                    FrequentAudioQueue.Enqueue(audioPlayer);

                    audioLayers.Add(layer, FrequentAudioQueue);
                }
            }

            return audioPlayer;
        }

        internal void Stop(AudioPlayer audioPlayer)
        {
            audioPlayer.Stop();
        }

        private bool TryPlayAudio(string name, out AudioData audioData)
        {
            if (!audioDatabase.ContainsKey(name))
            {
                Debug.LogError("No clip found with name: " + name);
                audioData = null;
                return false;
            }

            audioData = audioDatabase[name];

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

        internal void TransitionAudioSnapshot(string snapshotName, float transitionTime)
        {
            var snapshot = audioMixer.FindSnapshot(snapshotName);
            snapshot.TransitionTo(transitionTime);

            currentSnapshot = snapshot.name;
        }

        internal void TransitionAudioSnapshot(AudioMixerSnapshot snapshot, float transitionTime)
        {
            snapshot.TransitionTo(transitionTime);
            currentSnapshot = snapshot.name;
        }

        internal void PauseGroup(string groupName)
        {
            groups[groupName].PauseGroup(true);
        }

        internal void UnPauseGroup(string groupName)
        {
            groups[groupName].PauseGroup(false);
        }

        private AudioPlayer CreateAudioPlayer()
        {
            GameObject instance = new GameObject("AudioPlayer");
            AudioPlayer audioPlayer = instance.AddComponent<AudioPlayer>();
            audioPlayer.Setup(transform);
            return audioPlayer;
        }
    }
}
