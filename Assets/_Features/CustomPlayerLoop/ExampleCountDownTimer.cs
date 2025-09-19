using UnityEngine;

#if UNITY_EDITOR
#endif

namespace PlayerLoopInjectionSystem
{
    public class ExampleCountDownTimer : Timer
    {
        public ExampleCountDownTimer(float value) : base(value) { }

        public override bool IsFinished => CurrentTime <= 0;

        public override void Tick()
        {
            if (IsRunning && CurrentTime > 0)
            {
                CurrentTime -= Time.deltaTime;
            }

            if (IsRunning && CurrentTime <= 0)
            {
                Stop();
            }
        }
    }
}
