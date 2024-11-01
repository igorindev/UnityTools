using UnityEditor;
using UnityEngine;

public class AnimatorEventDispatcherStateBehaviour : StateMachineBehaviour
{
    AnimatorEventListener animatorEventListener;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animatorEventListener = animatorEventListener != null ? animatorEventListener : animator.GetComponent<AnimatorEventListener>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        UpdateState(stateInfo);
    }

    private void UpdateState(AnimatorStateInfo stateInfo)
    {
        int hash = stateInfo.shortNameHash;
        var currentTime = stateInfo.normalizedTime % 1f;
        animatorEventListener.Dispatch(hash, currentTime);
    }
}
