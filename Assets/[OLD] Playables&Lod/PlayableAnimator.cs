//// Licensed to the .NET Foundation under one or more agreements.
//// The .NET Foundation licenses this file to you under the MIT license.

//using UnityEngine;

//[RequireComponent(typeof(Animator))]
//public class PlayableAnimatorComponent : DistanceObjectComponent<AnimatorLODDistance, AnimationCascadeData>
//{
//    [SerializeField] private AnimationClip idle;
//    [SerializeField] private AnimationClip idle2;
//    [SerializeField] private AnimationClip wa;
//    [SerializeField] private AnimationClip running;
//    [SerializeField] private AvatarMask aa;
//    [SerializeField] private AvatarMask bb;

//    private AnimationSystem _animationSystem;

//    public bool update;

//    [Space(10)]
//    public float t;
//    public float t2;
//    [Space(10)]
//    public float b;
//    public float b2;
//    [Space(10)]
//    public float l1;
//    public float l2;

//    protected override AnimatorLODDistance DistanceStateAPI => AnimatorLODDistance.activeAPI;

//    private void Start()
//    {
//        var config = new AnimationConfig
//        {
//            animator = GetComponent<Animator>(),
//            idleClip = idle,
//            idle2Clip = idle2,
//            walkClip = wa,
//            runClip = running,
//            avatarMask = aa,
//            avatarMask2 = bb
//        };

//        _animationSystem = new AnimationSystem(config, null);
//    }

//    private void Update()
//    {
//        _animationSystem.UpdateMixerTop(0, t);
//        _animationSystem.UpdateMixerTop(1, t2);
//        _animationSystem.UpdateLayers(0, l1);
//        _animationSystem.UpdateLayers(1, l2);
//        // animationSystem.UpdateMixerBottom(0, b);
//        // animationSystem.UpdateMixerBottom(1, b2);
//    }

//    public override void Tick(float accumulatedDeltaTime, DistanceCascadeDataHolder<AnimationCascadeData> cascade)
//    {
//        if (!update)
//        {
//            return;
//        }

//        _animationSystem.Update(accumulatedDeltaTime);
//    }
//}
