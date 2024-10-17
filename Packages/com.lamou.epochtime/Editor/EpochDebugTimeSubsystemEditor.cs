using UnityEditor;

namespace EpochTime.Editor
{
    internal static class EpochDebugTimeSubsystemEditor
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EpochDebugTimeSubsystem.InitializeSubsystem();
            }
        }

        [MenuItem("EpochTime/Add30Seconds")]
        private static void Add30Seconds()
        {
            EpochDebugTimeSubsystem.SetDebugTimeOffset(30);
        }

        [MenuItem("EpochTime/AddOneMinute")]
        private static void AddOneMinute()
        {
            EpochDebugTimeSubsystem.SetDebugTimeOffset(60);
        }

        [MenuItem("EpochTime/AddOneHour")]
        private static void AddOneHour()
        {
            EpochDebugTimeSubsystem.SetDebugTimeOffset(60 * 60);
        }

        [MenuItem("EpochTime/Reset")]
        private static void Reset()
        {
            EpochDebugTimeSubsystem.Clear();
        }
    }
}