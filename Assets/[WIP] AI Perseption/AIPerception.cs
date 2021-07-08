using UnityEngine;

[AddComponentMenu("AI/AIPerseption")]
public class AIPerception : MonoBehaviour
{
    [SerializeField] AISense[] perceptions;

    [SerializeField] Transform target;
    [SerializeField] Vector3 lastPosition;

    int currentSense = 0;

    void FixedUpdate()
    {
        if (target != null)
        {
            //Keep using the current sense
            perceptions[currentSense].Sense(ref target, ref lastPosition);
        }
        else
        {
            //Senses try find a target
            for (int i = 0; i < perceptions.Length; i++)
            {
                if (perceptions[i].Sense(ref target, ref lastPosition))
                {
                    Stimuled();

                    if (target != null)
                    {
                        currentSense = i;
                        return;
                    }
                }
            }
        }
    }

    void Stimuled()
    {

    }

    //audiotion never sets a target, only return last positions
}