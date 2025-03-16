using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSynchronizerTest : MonoBehaviour
{
    [SerializeField] AnimatorSynchroniser _animatorSynchronizer;
    [SerializeField, Range(0, 1)] float _range;

    [Button]
    public void ChangeBool()
    {
        _animatorSynchronizer.SetBool("testBool", !_animatorSynchronizer.GetBool("testBool"));
    }

    private void Update()
    {
        _animatorSynchronizer.SetFloat("testFloat", _range);
    }
}
