using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_AIBehaviour_Follow.asset", menuName = "ScriptableObject/Behaviour/Follow")]
public class SO_AIBehaviour_Follow : SO_AIBehaviour
{
    public override void OnUpdate(out bool exitBehaviourState)
    {
        exitBehaviourState = true;
    }
}