using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStimulusVision : AIStimulus
{
    [SerializeField] protected Vector3 localStiumulusOrigin;
    [SerializeField] protected LayerMask affectMask;
    [SerializeField] float maxVisionDistance = 10;

    Collider[] allocatedColliders = new Collider[10];

    void FixedUpdate()
    {
        Stimulus();
    }

    public void Stimulus()
    {
        //Check if Join in range
        if (Physics.OverlapSphereNonAlloc(transform.TransformPoint(localStiumulusOrigin), maxVisionDistance, allocatedColliders, affectMask) > 0)
        {

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.TransformPoint(localStiumulusOrigin) + Vector3.up * 0.01f, maxVisionDistance);
    }
}
