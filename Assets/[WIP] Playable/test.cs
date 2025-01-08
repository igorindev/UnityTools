using UnityEngine;

public class test : MonoBehaviour
{
    public AnimationClip idle;
    public AnimationClip idle2;
    public AnimationClip wa;
    public AnimationClip running;
    public AvatarMask aa;
    public AvatarMask bb;
    AnimationSystem animationSystem;
    [Space(10)]
    public float t;
    public float t2;
    [Space(10)]
    public float b;
    public float b2;
    [Space(10)]
    public float l1;
    public float l2;

    void Start()
    {
        var a = new AnimationConfig
        {
            animator = GetComponent<Animator>(),
            idleClip = idle,
            idle2Clip = idle2,
            walkClip = wa,
            runClip = running,
            avatarMask = aa,
            avatarMask2 = bb
        };

        animationSystem = new AnimationSystem(a, null);
    }

    private void Update()
    {
        animationSystem.UpdateMixerTop(0, t);
        animationSystem.UpdateMixerTop(1, t2);

       // animationSystem.UpdateMixerBottom(0, b);
       // animationSystem.UpdateMixerBottom(1, b2);

        animationSystem.UpdateLayers(0, l1);
        animationSystem.UpdateLayers(1, l2);
    }
}
