using System.Collections;
using UnityEngine;

public class ReplayManager : Singleton<ReplayManager>
{
    [SerializeField] ReplayActor[] replayActors;

    [Header("Reproduction")]
    [SerializeField] int replaySpeed = 1;

    [Header("Recording")]
    [SerializeField] int frameRate = 60;
    [SerializeField] int recordFramesDuration = 5;

    bool isReplayActive = false;
    float saveDelay;            
    int numOfFramesToSave;

    Coroutine coroutine;

    void Start()
    {
        saveDelay = 1f / frameRate;
        numOfFramesToSave = recordFramesDuration * frameRate;
    }

    [ContextMenu("Start Save Frames")]
    void SaveFrames()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(SaveFramesCoroutine());
    }
    IEnumerator SaveFramesCoroutine()
    {
        while (isReplayActive == false)
        {
            for (int i = 0; i < replayActors.Length; i++)
            {
                replayActors[i].SaveReplayTransform(numOfFramesToSave);
            }

            yield return new WaitForSeconds(saveDelay);
        }
    }

    [ContextMenu("Replay")]
    public void StartReplay()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(ReplayCoroutine());
    }
    IEnumerator ReplayCoroutine()
    {
        isReplayActive = true;

        float total = recordFramesDuration;
        float eachDuration = total / numOfFramesToSave;

        int frame = 0;
        float count = 0;

        while (frame < numOfFramesToSave)
        {
            for (int i = 0; i < replayActors.Length; i++)
            {
                replayActors[i].PrepareLerp(frame);
            }

            while (count < saveDelay)
            {
                count += Time.deltaTime * replaySpeed;
                for (int i = 0; i < replayActors.Length; i++)
                {
                    replayActors[i].Lerp(count / eachDuration);
                }

                yield return null;
            }

            for (int i = 0; i < replayActors.Length; i++)
            {
                replayActors[i].FinishLerp();
            }
            count = 0;
            frame += 1;
        }

        isReplayActive = false;
    }
}