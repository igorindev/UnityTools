using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_AIBehaviour.asset", menuName = "ScriptableObject/SO_AIBehaviour")]
public abstract class SO_AIBehaviour : ScriptableObject
{
    public abstract void OnUpdate(out bool exitBehaviourState);
}