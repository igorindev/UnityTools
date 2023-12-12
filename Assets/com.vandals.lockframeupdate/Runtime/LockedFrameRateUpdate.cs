using System;
using UnityEngine;

public class LockedFrameRateUpdate : MonoBehaviour, IDisposable
{
    float updateTimerFramesPerSecond;
    float fixedDeltaTime;
    float fixedTimeAccumulator;

    public Action<float> OnUpdate;

    /// <summary>
    /// Allows creating a class that updates in a fixed frame rate, similar to FixedUpdate(). 
    /// E.g. when doing UI, create a <seeref=LockedFrameRateUpdate> to update the timer text, instead of using the Update(). 
    /// </summary>
    /// <param name="fixedFramesPerSecond">The frame rate to call update</param>
    /// <returns></returns>
    public static LockedFrameRateUpdate New(float fixedFramesPerSecond)
    {
        GameObject gameObject = new GameObject("LockedFrameRateUpdate");
        LockedFrameRateUpdate lockedFrameRateUpdate = gameObject.AddComponent<LockedFrameRateUpdate>();
        lockedFrameRateUpdate.Setup(fixedFramesPerSecond);

        return lockedFrameRateUpdate;
    }

    void Setup(float fixedFramesPerSecond)
    {
        updateTimerFramesPerSecond = fixedFramesPerSecond;
        fixedDeltaTime = 1f / updateTimerFramesPerSecond;
    }

    void Update()
    {
        fixedTimeAccumulator += Time.deltaTime;

        while (fixedTimeAccumulator >= fixedDeltaTime)
        {
            OnUpdate?.Invoke(fixedDeltaTime);
            fixedTimeAccumulator -= fixedDeltaTime;
        }
    }

    public void Dispose()
    {
        OnUpdate = null;
        Destroy(gameObject);
    }
}
