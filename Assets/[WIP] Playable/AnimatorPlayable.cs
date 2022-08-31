using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class AnimatorPlayable : MonoBehaviour
{
    //https://docs.unity3d.com/Manual/Playables-Examples.html
    //https://forum.unity.com/threads/why-does-the-playables-api-exist-when-should-i-use-it-rather-than-mecanim-animator-controllers.912857/

    public AnimationClip idle;
    public AnimationClip running;

    public Animator[] targetAnimators;
    AnimationMixerPlayable[] mixerPlayable;

    public float weight;

    PlayableGraph playableGraph;

    void OnEnable()
    {
        playableGraph = PlayableGraph.Create("Animator Playable");

        //GraphVisualizerClient.Show(playableGraph); Need the graph plugin
        mixerPlayable = new AnimationMixerPlayable[targetAnimators.Length];
        for (int i = 0; i < targetAnimators.Length; i++)
        {
            //Create the mixer
            mixerPlayable[i] = AnimationMixerPlayable.Create(playableGraph, 2);

            // Create the outputs
            AnimationPlayableOutput animationOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", targetAnimators[i]);

            // Create the playables.
            AnimationClipPlayable idlePlayable = AnimationClipPlayable.Create(playableGraph, idle);
            AnimationClipPlayable runningPlayable = AnimationClipPlayable.Create(playableGraph, running);

            // Connect the playables to an output
            animationOutput.SetSourcePlayable(mixerPlayable[i]);

            playableGraph.Connect(idlePlayable, 0, mixerPlayable[i], 0);
            playableGraph.Connect(runningPlayable, 0, mixerPlayable[i], 1);
        }

        // Plays the Graph.
        playableGraph.Play();
    }

    void OnDisable()
    {
        // Destroys all Playables and Outputs created by the graph.
        playableGraph.Destroy();
    }

    void Update()
    {
        weight = Mathf.Clamp01(weight);
        for (int i = 0; i < mixerPlayable.Length; i++)
        {
            mixerPlayable[i].SetInputWeight(0, 1.0f - weight);
            mixerPlayable[i].SetInputWeight(1, weight);
        }
    }
}