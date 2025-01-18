using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimatorLODSystem
{
    public class AnimatorLODSetTransformTarget : MonoBehaviour
    {
        [SerializeField] Transform _target;

        private void Start()
        {
            AnimatorLODManager.SetTarget(_target);
        }
    }
}
