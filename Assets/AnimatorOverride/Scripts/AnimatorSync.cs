using UnityEngine;

public class AnimatorSync : MonoBehaviour
{
    [SerializeField] Animator parent;
    [SerializeField] Animator[] childs;

    void Update()
    {
        for (int i = 0; i < parent.layerCount; i++)
        {
            foreach (Animator child in childs)
            {
                TickSync(parent, child, i);
            }
        }
    }

    private void TickSync(Animator source, Animator target, int layer)
    {
        target.Play(source.GetCurrentAnimatorStateInfo(layer).fullPathHash, layer, source.GetCurrentAnimatorStateInfo(layer).normalizedTime);
        target.SetLayerWeight(layer, source.GetLayerWeight(layer));
    }
}
