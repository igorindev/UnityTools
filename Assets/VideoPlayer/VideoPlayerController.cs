using System.Collections;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] VideoClip[] videoClips;

    VideoClip playing;
    int actual = 0;
    VideoPlayer videoPlayer;
    ImagePlayerController imageControll;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        imageControll = GetComponent<ImagePlayerController>();
        actual = 0;
        playing = videoClips[0];
        videoPlayer.clip = playing;
        StartCoroutine(Play());
    }
    void PlayNext()
    {
        videoPlayer.clip = playing = videoClips[actual];
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        videoPlayer.Play();
        yield return new WaitForSeconds((float)playing.length);
        actual += 1;
        if (actual < videoClips.Length)
        {
            PlayNext();
        }
        else
        {
            videoPlayer.targetCameraAlpha = 0;
            imageControll.Initialize();
        }
    }
}
