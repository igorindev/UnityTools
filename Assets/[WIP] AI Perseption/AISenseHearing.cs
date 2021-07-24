using UnityEngine;

public class AISenseHearing : AISense
{
    AIPerception perception;

    [SerializeField] float hearRange = 5;

    public override bool Sense(ref Transform target, ref Vector3 lastPosition)
    {
        return false;
    }

    //Sound stimulues create a overlapsphere that try find ai hearing senses inside, calling this
    public void Sense(Vector3 soundLocation)
    {
        if (Vector3.Distance(transform.position, soundLocation) < hearRange)
        {
            perception.Stimuled(soundLocation);
        }
    }

    void OnDrawGizmosSelected()
    {

    }
}
