using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AISkill.asset", menuName = "ScriptableObject/AISkill")]
public abstract class SO_AISkill : ScriptableObject
{
    public bool active = true;

    public abstract void OnInitialize();
    public abstract void OnUpdate(out bool triggered);
    public abstract void OnTrigger();
}