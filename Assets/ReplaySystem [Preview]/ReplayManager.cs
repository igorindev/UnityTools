using System.Collections;
using UnityEngine;

public class ReplayManager : MonoBehaviour
{
    public static ReplayManager instance;

    [SerializeField] int replayDurationInSeconds = 60; //60 seconds
    [SerializeField] int frameRate = 60; //60 frames per second
    [SerializeField] ReplayActor[] replayActors;
    [SerializeField] float realTime;
    [SerializeField] float replayTime;

    public int ReplayDuration { get => replayDurationInSeconds * FrameRate; }
    public int FrameRate { get => frameRate; }

    bool isReplayActive = false;
    float frameCount = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        frameCount = FrameRate;
    }

    // Update is called once per frame
    void Update()
    {
        realTime = Time.time;
        if (frameCount == 0)
        {
            frameCount = FrameRate;
            if (!isReplayActive)
            {
                for (int i = 0; i < replayActors.Length; i++)
                {
                    replayActors[i].SaveReplayTransform(ReplayDuration);
                }
            }
        }
        else
        {
            frameCount--;
        }
    }

    [ContextMenu("Play")]
    public void StartReplay()
    {
        isReplayActive = true;
        StartCoroutine(Replay());
    }

    IEnumerator Replay()
    {
        replayTime = 0;
        frameCount = FrameRate;
        int frame = 0;
        while (frame < ReplayDuration)
        {
            replayTime += Time.deltaTime;
            if (frameCount == 0)
            {
                frameCount = FrameRate;
                for (int i = 0; i < replayActors.Length; i++)
                {
                    replayActors[i].Replay(frame);
                }
                frame++;
            }
            else
            {
                frameCount--;
            }

            yield return null;
        }
        isReplayActive = false;
    }
}
