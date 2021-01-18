using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayActor : MonoBehaviour
{
    public int replayFramesSaved = 0;

    Queue<ReplayTransform> replayFrames = new Queue<ReplayTransform>();
    public Queue<ReplayTransform> ReplayFrames { get => replayFrames; set => replayFrames = value; }

    private void Reset()
    {
        
    }

    public void SaveReplayTransform(int replayFramesDuration)
    {
        ReplayTransform replayTransform = new ReplayTransform();

        if (ReplayFrames.Count >= replayFramesDuration)
        {
            ReplayFrames.Dequeue();
        }

        replayTransform.position.x = transform.position.x;
        replayTransform.position.y = transform.position.y;
        replayTransform.position.z = transform.position.z;

        replayTransform.eulerAngles.x = transform.eulerAngles.x;
        replayTransform.eulerAngles.y = transform.eulerAngles.y;
        replayTransform.eulerAngles.z = transform.eulerAngles.z;

        ReplayFrames.Enqueue(replayTransform);
        replayFramesSaved = ReplayFrames.Count;
    }

    public void Replay(int frame)
    {
        StartCoroutine(Lerp(frame));
    }

    IEnumerator Lerp(int frame)
    {
        ReplayTransform[] replayTransforms = ReplayFrames.ToArray();

        float frameRate = ReplayManager.instance.FrameRate;

        if (frame >= replayTransforms.Length - 1) { yield break;  }

        float timeElapsed = 0;
        Vector3 oldValue = new Vector3(replayTransforms[frame].position.x, replayTransforms[frame].position.y, replayTransforms[frame].position.z);
        Vector3 newValue = new Vector3(replayTransforms[frame + 1].position.x, replayTransforms[frame + 1].position.y, replayTransforms[frame + 1].position.z);

        while (timeElapsed < frameRate)
        {
            transform.position = Vector3.Slerp(oldValue, newValue, timeElapsed / frameRate);
            timeElapsed++;

            yield return null;
        }

        transform.position = new Vector3(replayTransforms[frame + 1].position.x, replayTransforms[frame + 1].position.y, replayTransforms[frame + 1].position.z);
        transform.position = new Vector3(replayTransforms[frame + 1].position.x, replayTransforms[frame + 1].position.y, replayTransforms[frame + 1].position.z);
    }
}

//TODO: works with int instead of floats
public struct Vector3Int
{
    public int x, y, z;
}

public struct ReplayTransform
{
    public Vector3 position;
    public Vector3 eulerAngles;
}
