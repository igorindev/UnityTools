using UnityEngine;
using System.Collections.Generic;

public class AnimatorOverrider : MonoBehaviour
{
    [SerializeField] AnimatorOverrideGroup group;

    int index;

    [ContextMenu("Override")]
    public void Override()
    {
        OverrideAnimatorAnimations(GetComponent<Animator>(), group.GetOverride(index.ToString()).overrideController);

        index++;
        index %= group.Count;
    }

    public void OverrideAnimatorAnimations(Animator animator, AnimatorOverrideController animatorOverrideController)
    {
        animator.runtimeAnimatorController = animatorOverrideController;
    }
}
