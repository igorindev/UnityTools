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

    bool grounded;

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
        //GroundChecking();

        transform.position = result;
    }

    bool GroundCheck()
    {
        return Physics.OverlapSphereNonAlloc(CapsuleBottom, radius, groundCollisionBuffer, groundLayers) > 0;
    }

    void GroundChecking()
    {
        Ray ray = new Ray(CapsuleBottom, Vector3.down);

        if (Physics.SphereCast(ray, radius, out RaycastHit tempHit, 20))
        {
           // GroundConfirm(tempHit);
        }
        else
        {
            grounded = false;
        }

    }
    Vector3 GroundConfirm(RaycastHit tempHit, Vector3 newPos)
    {
        Collider[] col = new Collider[6];
        int num = Physics.OverlapSphereNonAlloc(CapsuleBottom, radius, col);

        grounded = false;

        for (int i = 0; i < num; i++)
        {
            if (col[i].transform == tempHit.transform)
            {
                RaycastHit groundHit = tempHit;
                grounded = true;

                return new Vector3(newPos.x, (groundHit.point.y + HalfHeight), newPos.z);
            }
        }

        if (num <= 1 && tempHit.distance <= 3.1f)
        {
            if (col[0] != null)
            {
                Ray ray = new Ray(CapsuleBottom, Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit hit, 3.1f))
                {
                    if (hit.transform != col[0].transform)
                    {
                        grounded = false;
                        return newPos;
                    }
                }
            }
        }

        return newPos;
    }
}
