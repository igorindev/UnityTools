using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCharacterController : MonoBehaviour
{
    [Header("Body")]
    [SerializeField] Vector3 center;
    [SerializeField, Min(0)] float height = 2;
    [SerializeField, Min(0)] float radius = 0.5f;
    [SerializeField] bool collideWithTrigges = true;
    [SerializeField] LayerMask collisionLayer = ~0;

    [Header("Ground")]
    [SerializeField] LayerMask groundLayers = ~0;

    [Header("Forces")]
    [SerializeField] bool hasGravity;

    public Action<RaycastHit> onControllerHit;
        
    public bool IsGrounded => GroundCheck();

    float HalfHeight => Mathf.Max(height * 0.5f, radius);
    public Vector3 Center => center;
    Vector3 Bottom => center - HalfHeight * transform.up;
    Vector3 Top => center + HalfHeight * transform.up;

    public Vector3 WorldCenter => transform.position + center;
    public Vector3 WorldBottom => transform.position + Bottom;
    public Vector3 WorldTop => transform.position + Top;

    public Vector3 CapsuleTop => WorldTop - (radius * transform.up);
    public Vector3 CapsuleBottom => WorldBottom + (radius * transform.up);

    Vector3 move;

    public Collider[] myColliders;
    
    readonly Collider[] groundCollisionBuffer = new Collider[1];

    void Reset()
    {
        myColliders = GetComponentsInChildren<Collider>();
    }

    public void Move(Vector3 motion)
    {
        move = transform.position + motion;
        CollisionDetection();
    }

    void CollisionDetection()
    {
        //RaycastHit[] hitBuffer = Physics.CapsuleCastAll(CapsuleTop, CapsuleBottom, radius, transform.up, height, collisionLayer, (QueryTriggerInteraction)(collideWithTrigges ? 2 : 1));
        Collider[] hitBuffer = Physics.OverlapCapsule(CapsuleTop, CapsuleBottom, radius, collisionLayer, (QueryTriggerInteraction)(collideWithTrigges ? 2 : 1));
        if (hitBuffer.Length > 0)
        {
            for (int i = 0; i < myColliders.Length; i++)
            {
                for (int j = 0; j < hitBuffer.Length; j++)
                {
                    if (myColliders[i] == hitBuffer[j]) { continue; }

                    if (Physics.ComputePenetration(myColliders[i],
                                               move,
                                               transform.rotation,
                                               hitBuffer[j],
                                               hitBuffer[j].transform.position,
                                               hitBuffer[j].transform.rotation,
                                               out Vector3 direction,
                                               out float distance))
                    {
                        ResolveIntersection(direction, distance);
                    }

                    //onControllerHit?.Invoke(hitBuffer[j]);
                }
            }
        }

        ApplyResult(move);
    }
    void ResolveIntersection(Vector3 direction, float distance)
    {
        move += direction * distance;
    }
    void ApplyResult(Vector3 result)
    {
        transform.position = result;
    }

    void Gravity()
    {
        
    }

    bool GroundCheck()
    {
        return Physics.OverlapSphereNonAlloc(CapsuleBottom, radius, groundCollisionBuffer, groundLayers) > 0;
    }
}
