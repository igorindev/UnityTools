using AudioSubsystem;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    [SerializeField] string audioReference;
    [SerializeField] string frequentLayer = "Test";
    [SerializeField] int priority = 1;

    [Header("Conditions")]
    [SerializeField] bool playOnStart;
    [SerializeField] bool playOnUpdate;
    [SerializeField] float updateDelay = 0.5f;

    AudioPlayer audioPlayer;
    uint layer;
    float lastUpdate;

    private void Start()
    {
        layer = Audio.StringToHash(frequentLayer);

        if (playOnStart)
        {
            PlayAudio();
        }
    }

    private void Update()
    {
        if (playOnUpdate && Time.time > updateDelay + lastUpdate)
        {
            lastUpdate = Time.time;
            PlayAudio();
        }
    }

    [ContextMenu("Play Audio")]
    public void PlayAudio()
    {
        audioPlayer = Audio.Play(audioReference, layer, priority);
    }

    [ContextMenu("Stop Audio")]
    public void StopAudio()
    {
        if (audioPlayer)
        {
            audioPlayer.Stop();
        }
    }
}
