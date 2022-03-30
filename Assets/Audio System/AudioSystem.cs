using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSystem : MonoBehaviour
{
    [SerializeField, Min(0)] int numOfAudioSources = 64;
    public SerializableDictionary<string, AudioData> audioList;
    AudioSourceEntity[] poolAudioSources;
    AudioSource source2D;

    void OnValidate()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;

        UnityEditor.EditorApplication.delayCall += () =>
        {
            var tempArray = new GameObject[transform.childCount];

            for (int i = 0; i < tempArray.Length; i++)
            {
                tempArray[i] = transform.GetChild(i).gameObject;
            }

            foreach (var child in tempArray)
            {
                DestroyImmediate(child);
            }

            for (int i = 0; i < numOfAudioSources; i++)
            {
                GameObject gameObject = new GameObject();
                gameObject.transform.parent = transform;
                gameObject.AddComponent<AudioSourceEntity>().Initialize();
                gameObject.name = "AudioSourceEntity (" + i + ")";
            }
        };
#endif
    }

    void Awake()
    {
        if (Audio.audioSystem)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Audio.audioSystem = this;
        }

        source2D = GetComponent<AudioSource>();
        source2D.spatialBlend = 0;
        source2D.playOnAwake = false;
        poolAudioSources = GetComponentsInChildren<AudioSourceEntity>();

        DontDestroyOnLoad(gameObject);
    }

    public void PlayOneShot(string name) 
    {
        if (!audioList.ContainsKey(name))
        {
            Debug.LogError("No clip found with name: " + name);
            return;
        }

        AudioData audioData = audioList[name];                 
        int rand = Random.Range(0, audioData.clips.Length);

        source2D.loop = audioData.loop;

        audioData.GetConfigs(out float volume, out float pitch);
        source2D.PlayOneShot(audioData.clips[rand], volume);
    }
    public void Play(string name)
    {
        if (!audioList.ContainsKey(name))
        {
            Debug.LogError("No clip found with name: " + name);
            return;
        }

        AudioData audioData = audioList[name];

        if (Time.time - audioData.LastTimePlayed < audioData.minTimeBetweenCall)
        {
            return;
        }
        else
        {
            audioData.LastTimePlayed = Time.time;
        }

        int rand = Random.Range(0, audioData.clips.Length);

        AudioSourceEntity audioObj = null;
        for (int i = 0; i < poolAudioSources.Length; i++)
        {
            if (!poolAudioSources[i].InUse)
            {
                audioObj = poolAudioSources[i];
                break;
            }
        }

        audioObj.AudioSource.clip = audioData.clips[rand];
        audioObj.AudioSource.spatialBlend = 0;
        audioObj.AudioSource.outputAudioMixerGroup = audioData.outputAudioMixerGroup;
        audioObj.AudioSource.loop = audioData.loop;

        audioData.GetConfigs(out float volume, out float pitch);
        audioObj.AudioSource.volume = volume;
        audioObj.AudioSource.pitch = pitch;

        audioObj.AudioSource.Play();
        audioObj.InUse = true;

        if (!audioData.loop && audioObj.AudioSource.clip != null)
        {
            audioObj.ResetPoolObject(audioObj.AudioSource.clip.length);
        }
    }
    public void PlayAtPosition(string name, Vector3 worldPosition, Transform parent) 
    {
        if (!audioList.ContainsKey(name))
        {
            Debug.LogError("No clip found with name: " + name);
            return;
        }

        AudioData audioData = audioList[name];

        if (Time.time - audioData.LastTimePlayed < audioData.minTimeBetweenCall)
        {
            return;
        }
        else 
        {
            audioData.LastTimePlayed = Time.time;
        }

        int rand = Random.Range(0, audioData.clips.Length);

        AudioSourceEntity audioObj = null;
        for (int i = 0; i < poolAudioSources.Length; i++)
        {
            if (!poolAudioSources[i].InUse) 
            {
                audioObj = poolAudioSources[i];
                break;
            }
        }

        audioObj.transform.parent = parent;
        audioObj.transform.position = worldPosition;
        audioObj.AudioSource.clip = audioData.clips[rand];
        audioObj.AudioSource.spatialBlend = 1f;
        audioObj.AudioSource.minDistance = 4f;
        audioObj.AudioSource.spatialBlend = audioData._2D ? 0 : 1;
        audioObj.AudioSource.outputAudioMixerGroup = audioData.outputAudioMixerGroup;
        audioObj.AudioSource.loop = audioData.loop;

        audioData.GetConfigs(out float volume, out float pitch);
        audioObj.AudioSource.volume = volume;
        audioObj.AudioSource.pitch = pitch;

        audioObj.AudioSource.Play();
        audioObj.InUse = true;

        if (!audioData.loop && audioObj.AudioSource.clip != null) 
        {
            audioObj.ResetPoolObject(audioObj.AudioSource.clip.length);
        }
    }
    public void PlayMusic(string name) 
    {
        if (!audioList.ContainsKey(name))
        {
            Debug.LogError("No clip found with name: " + name);
            return;
        }

        AudioData audioData = audioList[name];

        if (Time.time - audioData.LastTimePlayed < audioData.minTimeBetweenCall)
        {
            return;
        }
        else
        {
            audioData.LastTimePlayed = Time.time;
        }

        int rand = Random.Range(0, audioData.clips.Length);

        AudioSourceEntity audioObj = null;
        for (int i = 0; i < poolAudioSources.Length; i++)
        {
            if (!poolAudioSources[i].InUse)
            {
                audioObj = poolAudioSources[i];
                break;
            }
        }

        audioObj.AudioSource.clip = audioData.clips[rand];
        audioObj.AudioSource.loop = audioData.loop;
        audioObj.AudioSource.spatialBlend = audioData._2D ? 0 : 1;
        audioObj.AudioSource.outputAudioMixerGroup = audioData.outputAudioMixerGroup;

        audioData.GetConfigs(out float volume, out float pitch);
        audioObj.AudioSource.volume = volume;
        audioObj.AudioSource.pitch = pitch;

        audioObj.AudioSource.Play();
        audioObj.InUse = true;

        if (!audioData.loop && audioObj.AudioSource.clip != null)
        {
            audioObj.ResetPoolObject(audioObj.AudioSource.clip.length);
        }
    }
}