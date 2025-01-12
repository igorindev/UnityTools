using System;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
//https://github.com/Unity-Technologies/animation-jobs-samples/blob/master/Assets/animation-jobs-samples/Runtime/AnimationJobs/MixerJob.cs
//https://docs.unity3d.com/ScriptReference/Animations.AnimationScriptPlayable.Create.html
//https://github.com/SolarianZ/UnityPlayableGraphMonitorTool
[Serializable]
public class AnimationConfig
{
    public Animator animator;
    public AnimationClip idleClip;
    public AnimationClip idle2Clip;
    public AnimationClip walkClip;
    public AnimationClip runClip;
    public AvatarMask avatarMask;
    public AvatarMask avatarMask2;
}

[Serializable]
public class AudioConfig
{
    public AudioSource audioSource;
    public AudioClip[] footstepSounds;
}

public class AnimationSystem
{
    PlayableGraph playableGraph;
    AnimationMixerPlayable topLevelMixer;
    AnimationMixerPlayable locomotionMixer;
    AnimationLayerMixerPlayable _layerMixerPlayable;
    AnimationClipPlayable oneShotPlayable;
    ScriptPlayable<FootstepPlayableBehaviour> footstepPlayable;
    AnimationScriptPlayable m_AnimationScriptPlayable;

    CancellationTokenSource _cancellationTokenSource;

    public AnimationSystem(AnimationConfig animationConfig, AudioConfig audioConfig)
    {
        playableGraph = PlayableGraph.Create("AnimationSystem");

        SetupAnimations(animationConfig);
        //SetupFootsteps(audioConfig);
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
    }

    public void Update(float deltaTime)
    {
        playableGraph.Evaluate(deltaTime);
    }

    private void SetupAnimations(AnimationConfig animationConfig)
    {
        var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", animationConfig.animator);

        _layerMixerPlayable = AnimationLayerMixerPlayable.Create(playableGraph, 2);
        //_layerMixerPlayableBottom = AnimationLayerMixerPlayable.Create(playableGraph, 2);

        _layerMixerPlayable.SetLayerAdditive(0, false);
        _layerMixerPlayable.SetLayerMaskFromAvatarMask(0, animationConfig.avatarMask);

        _layerMixerPlayable.SetLayerAdditive(1, false);
        _layerMixerPlayable.SetLayerMaskFromAvatarMask(1, animationConfig.avatarMask2);


        AnimationJob animationJob = new AnimationJob();
        m_AnimationScriptPlayable = AnimationScriptPlayable.Create(playableGraph, animationJob, 2);

        //topLevelMixer = AnimationMixerPlayable.Create(playableGraph, 2);
        playableOutput.SetSourcePlayable(m_AnimationScriptPlayable);

        //locomotionMixer = AnimationMixerPlayable.Create(playableGraph, 2);
        //m_AnimationScriptPlayable.SetProcessInputs(false);
        m_AnimationScriptPlayable.AddInput(_layerMixerPlayable, 0, 1);

        playableGraph.GetRootPlayable(0).SetInputWeight(0, 1f);

        var idlePlayable = AnimationClipPlayable.Create(playableGraph, animationConfig.idleClip);
        var walkPlayable = AnimationClipPlayable.Create(playableGraph, animationConfig.idle2Clip);

        idlePlayable.GetAnimationClip().wrapMode = WrapMode.Loop;
        walkPlayable.GetAnimationClip().wrapMode = WrapMode.Loop;

        _layerMixerPlayable.ConnectInput(0, idlePlayable, 0);
        _layerMixerPlayable.ConnectInput(1, walkPlayable, 0);
    }

    private void SetupFootsteps(AudioConfig audioConfig)
    {
        var scriptOutput = ScriptPlayableOutput.Create(playableGraph, "Footsteps");
    
        footstepPlayable = ScriptPlayable<FootstepPlayableBehaviour>.Create(playableGraph);
        var behaviour = footstepPlayable.GetBehaviour();
        behaviour.footstepSounds = audioConfig.footstepSounds;
        behaviour.audioSource = new ExposedReference<AudioSource> { defaultValue = audioConfig.audioSource };
    
        scriptOutput.SetSourcePlayable(footstepPlayable);
    }
    
    public void OnFootstep() => footstepPlayable.GetBehaviour().PlayFootstepSound();

    public void UpdateLayers(int index, float weight)
    {
        m_AnimationScriptPlayable.SetInputWeight(index, weight);
    }

    public void UpdateMixerTop(int index, float weight)
    {
        _layerMixerPlayable.SetInputWeight(index, weight);
    }

    public void UpdateLocomotion(int index, Vector3 velocity, float maxSpeed)
    {
        float weight = Mathf.InverseLerp(0f, maxSpeed, velocity.magnitude);

        if (index == 0)
        {
            _layerMixerPlayable.SetInputWeight(0, 1f - weight);
            _layerMixerPlayable.SetInputWeight(0, weight);
        }

    }

    public void PlayOneShot(AnimationClip oneShotClip)
    {
        if (oneShotPlayable.IsValid() && oneShotPlayable.GetAnimationClip() == oneShotClip)
        {
            return;
        }

        InterruptOneShot();
        oneShotPlayable = AnimationClipPlayable.Create(playableGraph, oneShotClip);
        topLevelMixer.ConnectInput(1, oneShotPlayable, 0);
        topLevelMixer.SetInputWeight(1, 1f);

        float blendDuration = Mathf.Clamp(oneShotClip.length * 0.1f, 0.1f, oneShotClip.length * 0.5f);

        BlendInOut(blendDuration, oneShotClip.length - blendDuration).Forget();
    }

    private async UniTask BlendInOut(float blendDuration, float delay)
    {
        await Blend(blendDuration, blendTime =>
        {
            float weight = Mathf.Lerp(1f, 0f, blendTime);
            topLevelMixer.SetInputWeight(0, weight);
            topLevelMixer.SetInputWeight(1, 1f - weight);
        });

        await UniTask.WaitForSeconds(delay, cancellationToken: _cancellationTokenSource.Token).SuppressCancellationThrow();

        await Blend(blendDuration, blendTime =>
        {
            float weight = Mathf.Lerp(0f, 1f, blendTime);
            topLevelMixer.SetInputWeight(0, weight);
            topLevelMixer.SetInputWeight(1, 1f - weight);
        });

        DisconnectOneShot();
    }

    private async UniTask Blend(float duration, Action<float> blendCallback)
    {
        float blendTime = 0f;
        while (blendTime < 1f)
        {
            blendTime += Time.deltaTime / duration;
            blendCallback(blendTime);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, _cancellationTokenSource.Token).SuppressCancellationThrow();
        }

        blendCallback(1f);
    }

    private void InterruptOneShot()
    {
        _cancellationTokenSource?.Cancel();

        topLevelMixer.SetInputWeight(0, 1f);
        topLevelMixer.SetInputWeight(1, 0f);

        if (oneShotPlayable.IsValid())
        {
            DisconnectOneShot();
        }
    }

    private void DisconnectOneShot()
    {
        topLevelMixer.DisconnectInput(1);
        playableGraph.DestroyPlayable(oneShotPlayable);
    }

    private void Destroy()
    {
        if (playableGraph.IsValid())
        {
            playableGraph.Destroy();
        }
    }

    public struct AnimationJob : IAnimationJob
    {
        public int userData;

        public void ProcessRootMotion(AnimationStream stream)
        {
            // This method is called during the root motion process pass.
        }

        public void ProcessAnimation(AnimationStream stream)
        {
            // This method is called during the animation process pass.
            Debug.Log(string.Format("Value of the userData: {0}", userData));
        }
    }
}
