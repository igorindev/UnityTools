using System;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace PlayerLoopInjectionSystem
{
    public abstract class Timer : IDisposable
    {
        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; protected set; }

        protected float initialTime;

        public float Progress => Mathf.Clamp(CurrentTime / initialTime, 0.0f, 1.0f);

        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };

        private bool disposed;

        protected Timer(float value)
        {
            initialTime = value;
        }

        ~Timer()
        {
            Dispose(false);
        }

        //Call Dispose to ensure deregistration of the timer from the TimerManager
        //when the consumer is done with the timer or being destroyed
        public void Dispose()
        {
            Dispose(true);
            //Calling this to tell the GC to not call the Finalizer (Destructor) as we handled cleaning (this is more performant)
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                TimerManager.DeregisterTimer(this);
            }

            disposed = true;
        }

        public void Start()
        {
            CurrentTime = initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                TimerManager.RegisterTimer(this);
                OnTimerStart.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                TimerManager.DeregisterTimer(this);
                OnTimerStop.Invoke();
            }
        }

        public abstract void Tick();
        public abstract bool IsFinished { get; }

        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;

        public virtual void Reset() => CurrentTime = initialTime;

        public virtual void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }
    }
}
