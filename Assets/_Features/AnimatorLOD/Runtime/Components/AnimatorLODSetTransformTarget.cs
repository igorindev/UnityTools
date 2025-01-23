using UnityEngine;

namespace AnimatorLODSystem
{
    public class AnimatorLODSetTransformTarget : MonoBehaviour
    {
        [SerializeField] Transform _target;

        private void Start()
        {
            AnimatorLODSubsystem.SetTarget(_target);
        }
    }
}
