using System;
using UnityEngine;

namespace Vandals.LockedFrameRate
{
    public class LockedFrameRateUpdate : IDisposable
    {
        private static LockedFrameRateUpdateManager manager;
        private readonly float updateTimerFramesPerSecond;
        private readonly float fixedDeltaTime;

        float fixedTimeAccumulator;

        public Action<float> OnUpdate;

        public LockedFrameRateUpdate(float fixedFramesPerSecond)
        {
            updateTimerFramesPerSecond = fixedFramesPerSecond;
            fixedDeltaTime = 1f / updateTimerFramesPerSecond;
        }

        /// <summary>
        /// Allows creating a class that updates in a fixed frame rate, similar to FixedUpdate(). 
        /// E.g. when doing UI, create a <seeref=LockedFrameRateUpdate> to update the timer text, instead of using the Update(). 
        /// </summary>
        /// <param name="fixedFramesPerSecond">The frame rate to call update</param>
        /// <returns></returns>
        public static LockedFrameRateUpdate New(float fixedFramesPerSecond)
        {
            if (manager == null)
            {
                GameObject gameObject = new GameObject("LockedFrameRateUpdateManager");
                manager = gameObject.AddComponent<LockedFrameRateUpdateManager>();
                GameObject.DontDestroyOnLoad(gameObject);
            }

            LockedFrameRateUpdate lockedFrameRateUpdate = new LockedFrameRateUpdate(fixedFramesPerSecond);
            manager.Add(lockedFrameRateUpdate);
            return lockedFrameRateUpdate;
        }

        internal void Update(float dt)
        {
            fixedTimeAccumulator += dt;

            while (fixedTimeAccumulator >= fixedDeltaTime)
            {
                OnUpdate?.Invoke(fixedDeltaTime);
                fixedTimeAccumulator -= fixedDeltaTime;
            }
        }

        public void Dispose()
        {
            OnUpdate = null;
            if (manager)
                manager.Remove(this);
        }
    }
}