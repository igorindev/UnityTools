using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    [SerializeField] string audioReference;

    AudioPlayer player;

    [ContextMenu("Play Audio")]
    public void PlayAudio()
    {
        player = Audio.Play(audioReference);
    }

    [ContextMenu("Stop Audio")]
    public void StopAudio()
    {
        if (player)
        {
            player.Stop();
        }
    }
}
