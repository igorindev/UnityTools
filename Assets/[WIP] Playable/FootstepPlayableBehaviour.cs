using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class FootstepPlayableBehaviour : PlayableBehaviour
{
    public AudioClip[] footstepSounds;
    public ExposedReference<AudioSource> audioSource;

    AudioSource resolvedAudioSource;
    PlayableGraph playableGraph;
    AudioMixerPlayable audioMixerPlayable;
    AudioClipPlayable currentClipPlayable;

    public override void OnGraphStart(Playable playable)
    {
        resolvedAudioSource = audioSource.Resolve(playable.GetGraph().GetResolver());
        playableGraph = playable.GetGraph();

        audioMixerPlayable = AudioMixerPlayable.Create(playableGraph, 1);

        var audioOutput = AudioPlayableOutput.Create(playableGraph, "FootstepAudio", resolvedAudioSource);
        audioOutput.SetSourcePlayable(audioMixerPlayable);
    }

    public void PlayFootstepSound()
    {
        if (footstepSounds == null || footstepSounds.Length == 0 || resolvedAudioSource == null) return;

        var selectedClip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        var newClipPlayable = AudioClipPlayable.Create(playableGraph, selectedClip, false);

        if (currentClipPlayable.IsValid())
        {
            audioMixerPlayable.DisconnectInput(0);
            playableGraph.DestroyPlayable(currentClipPlayable);
        }

        playableGraph.Connect(newClipPlayable, 0, audioMixerPlayable, 0);
        audioMixerPlayable.SetInputWeight(0, 1f);

        currentClipPlayable = newClipPlayable;
    }
}
