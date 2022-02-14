using System.Collections.Generic;
using UnityEngine;

public class ReplayActor : MonoBehaviour
{
    public Queue<ReplayTransform> ReplayFrames { get; set; } = new Queue<ReplayTransform>();
    ReplayTransform[] replayTransforms;

    Vector3 oldPosValue;
    Vector3 newPosValue;

    Quaternion oldRotValue;
    Quaternion newRotValue;

    public void SaveReplayTransform(int replayFramesDuration)
    {
        ReplayTransform replayTransform = new ReplayTransform();

        if (ReplayFrames.Count >= replayFramesDuration)
        {
            ReplayFrames.Dequeue();
        }

        replayTransform.position = transform.position;
        replayTransform.rotation = transform.rotation;

        ReplayFrames.Enqueue(replayTransform);
    }

    public void PrepareLerp(int frame)
    {
        replayTransforms = ReplayFrames.ToArray();
        
        if (frame >= replayTransforms.Length - 1) { return; }

        oldPosValue = replayTransforms[frame].position;
        oldRotValue = replayTransforms[frame].rotation;

        newPosValue = replayTransforms[frame + 1].position;
        newRotValue = replayTransforms[frame + 1].rotation;
    }
    public void Lerp(float value)
    {
        transform.SetPositionAndRotation(Vector3.Lerp(oldPosValue, newPosValue, value), Quaternion.Lerp(oldRotValue, newRotValue, value));
    }
    public void FinishLerp()
    {
        transform.SetPositionAndRotation(newPosValue, newRotValue);
    }
}

[System.Serializable]
public struct ReplayTransform
{
    public Vector3 position;
    public Quaternion rotation;
}
