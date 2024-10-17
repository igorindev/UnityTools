using System;
using UnityEngine;

namespace EpochTime
{
    public static class EpochDebugTimeSubsystem
    {
        private const string EpochKey = "EpochTimePlayerPreferencesKey";
        private static TimeSpan debugTimeOffset = TimeSpan.Zero;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void InitializeSubsystem()
        {
            debugTimeOffset = TimeSpan.Zero;
            LoadData();
        }

        public static DateTime GetUtcTime()
        {
            return DateTime.UtcNow + debugTimeOffset;
        }

        public static void SetDebugTimeOffset(double seconds)
        {
            UpdateTimeOffset(seconds);

            SaveData();

            Debug.Log($"[Epoch Time] DateTime: {GetUtcTime()} | (Debug Offset: {debugTimeOffset})");
        }

        public static void Clear()
        {
            SetDebugTimeOffset(-debugTimeOffset.TotalSeconds);
        }

        private static void UpdateTimeOffset(double seconds)
        {
            debugTimeOffset += TimeSpan.FromSeconds(seconds);
        }

        private static void LoadData()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            long offsetSeconds = long.Parse(PlayerPrefs.GetString(EpochKey, "0"));
            SetDebugTimeOffset(offsetSeconds);
#endif
        }

        private static void SaveData()
        {
            PlayerPrefs.SetString(EpochKey, debugTimeOffset.TotalSeconds.ToString());
        }
    }
}