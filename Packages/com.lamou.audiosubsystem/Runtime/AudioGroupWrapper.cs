using System;
using UnityEngine.Audio;

namespace AudioSubsystem
{
    internal class AudioGroupWrapper
    {
        internal event Action<bool> OnAudioGroupPaused;

        internal bool UseTimeScale { get; private set; }
        internal AudioMixerGroup AudioMixerGroup { get; private set; }

        internal AudioGroupWrapper(AudioGroup group)
        {
            UseTimeScale = group.UseTimeScale;
            AudioMixerGroup = group.OutputAudioMixerGroup;
        }

        internal void PauseGroup(bool value)
        {
            OnAudioGroupPaused?.Invoke(value);
        }
    }
}