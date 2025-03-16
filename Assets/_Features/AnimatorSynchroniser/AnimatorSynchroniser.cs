using UnityEngine;

public class AnimatorSynchroniser : MonoBehaviour
{
    [SerializeField] private Animator[] animators;

    private void Start()
    {
        foreach (Animator animator in animators)
        {
            animator.playableGraph.SetTimeUpdateMode(UnityEngine.Playables.DirectorUpdateMode.Manual);
        }
    }

    private void Update()
    {
        foreach (Animator animator in animators)
        {
            animator.Update(Time.deltaTime);
        }
    }

    public void SetBool(string name, bool value)
    {
        foreach (Animator anim in animators)
        {
            anim.SetBool(name, value);
        }
    }

    public void SetFloat(string name, float value)
    {
        foreach (Animator anim in animators)
        {
            anim.SetFloat(name, value);
        }
    }

    public void SetInteger(string name, int value)
    {
        foreach (Animator anim in animators)
        {
            anim.SetInteger(name, value);
        }
    }

    public void SetTrigger(string name)
    {
        foreach (Animator anim in animators)
        {
            anim.SetTrigger(name);
        }
    }

    public void SetLayerWeight(int layer, float value)
    {
        foreach (Animator anim in animators)
        {
            anim.SetLayerWeight(layer, value);
        }
    }

    public bool GetBool(string name)
    {
        return animators[0].GetBool(name);
    }

    public float GetFloat(string name)
    {
        return animators[0].GetFloat(name);
    }

    public int GetInteger(string name)
    {
        return animators[0].GetInteger(name);
    }
}
