using UnityEngine;

[AddComponentMenu("AI/AIPerseption")]
public class AIPerception : MonoBehaviour
{
    [SerializeField] AISense[] perceptions;

    [SerializeField] Transform target;
    [SerializeField] Vector3 lastPosition;

    int currentSense = 0;

    //If sees the player, dont need to keep updating

    void Update()
    {
       // if (target != null)
       // {
       //     //Keep using the current sense
       //     perceptions[currentSense].Sense(ref target, ref lastPosition);
       //     Stimuled();
       // }
       // else
       // {
       //     //Senses try find a target
       //     for (int i = 0; i < perceptions.Length; i++)
       //     {
       //         if (perceptions[i].Sense(ref target, ref lastPosition))
       //         {
       //             Stimuled();
       //
       //             if (target != null)
       //             {
       //                 currentSense = i;
       //                 return;
       //             }
       //         }
       //     }
       // }
    }

    public virtual void Stimuled(Vector3 stimulusLocation)
    {
        transform.LookAt(stimulusLocation);
    }

    //audiotion never sets a target, only return last positions
}