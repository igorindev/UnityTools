using System.Collections.Generic;

#if UNITY_EDITOR
#endif

namespace PlayerLoopInjectionSystem
{
    public static class TimerManager
    {
        private static readonly List<Timer> timers = new();

        public static void RegisterTimer(Timer timer) => timers.Add(timer);
        public static void DeregisterTimer(Timer timer) => timers.Remove(timer);
        public static void Clear() => timers.Clear();

        public static void UpdateTimers()
        {
            foreach (Timer timer in timers)
            {
                timer.Tick();
            }
        }
    }
}
