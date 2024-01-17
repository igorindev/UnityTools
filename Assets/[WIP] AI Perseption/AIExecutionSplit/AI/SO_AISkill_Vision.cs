using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AISkillVision.asset", menuName = "ScriptableObject/AISkillVision")]
public class SO_AISkill_Vision : SO_AISkill
{
    public override void OnInitialize()
    {
        
    }
    public override void OnUpdate(out bool triggered)
    {
        Debug.LogWarning("aaa");
        Debug.LogWarning("aaa");
        Debug.LogWarning("aaa");
        Debug.LogWarning("aaa");
        Debug.LogWarning("aaa");
        Debug.LogWarning("aaa");
        Debug.LogWarning("aaa");
        triggered = false;
    }
    public override void OnTrigger()
    {

    }
}