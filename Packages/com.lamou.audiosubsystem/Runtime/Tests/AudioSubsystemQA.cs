using UnityEngine;

namespace AudioSubsystem.Tests
{
    public class AudioSubsystemQA : MonoBehaviour
    {
        [SerializeField] string groupToPause = "Test";

        [ContextMenu("Test Audio Reset")]
        private void TestReset()
        {
            AudioSettings.Reset(AudioSettings.GetConfiguration());
        }

        [ContextMenu("Pause Group")]
        private void PauseGroup()
        {
            Audio.PauseGroup(groupToPause);
        }

        [ContextMenu("UnPause Group")]
        private void UnPauseGroup()
        {
            Audio.UnPauseGroup(groupToPause);
        }
    }
}