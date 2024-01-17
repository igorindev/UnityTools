using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public SO_AISkill[] skills;
    public SO_AIBehaviour[] behaviours;

    public bool onBehaviourActive;

    public void UpdateBrain()
    {
        if (onBehaviourActive)
        {
            for (int i = 0; i < behaviours.Length; i++)
            {
                behaviours[i].OnUpdate(out bool exitBehaviourState);
                onBehaviourActive = exitBehaviourState;
            }
        }
        else
        {
            for (int i = 0; i < skills.Length; i++)
            {
                if (skills[i].active)
                {
                    skills[i].OnUpdate(out bool triggered);

                    if (triggered)
                    {
                        skills[i].OnTrigger();
                        onBehaviourActive = triggered;
                    }
                }
            }
        }
    }
}
