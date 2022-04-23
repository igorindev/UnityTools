using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class CustomCharacterController : MonoBehaviour
{
    [Header("Body")]
    public CapsuleCollider capsuleCollider;
    [SerializeField] Vector3 center;
    [SerializeField] bool collideWithTrigges = true;
    [SerializeField] LayerMask collisionLayer = ~0;

    [Header("Ground")]
    [SerializeField] LayerMask groundLayers = ~0;

    public Action<RaycastHit> onControllerHit;
        
    public bool IsGrounded => GroundCheck();
    
    float Radius => capsuleCollider.radius;
    float Height => capsuleCollider.height;
    float HalfHeight => Mathf.Max(Height * 0.5f, Radius);

    public Vector3 Center => center;
    Vector3 Bottom => center - HalfHeight * transform.up;
    Vector3 Top => center + HalfHeight * transform.up;

    public Vector3 WorldCenter => transform.position + center;
    public Vector3 WorldBottom => transform.position + Bottom;
    public Vector3 WorldTop => transform.position + Top;

    public Vector3 CapsuleTop => WorldTop - (Radius * transform.up);
    public Vector3 CapsuleBottom => WorldBottom + (Radius * transform.up);

    public Vector3 Velocity { get; private set; }

    Vector3 previousPos;

    Vector3 move;

    bool grounded;
    
    readonly Collider[] groundCollisionBuffer = new Collider[6];
    Collider[] hitBuffer;
    void Reset()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public void Move(Vector3 motion)
    {
        previousPos = transform.position;
        move = transform.position + motion;
        CollisionDetection();
    }

    void CollisionDetection()
    {
        //RaycastHit[] hitBuffer = Physics.CapsuleCastAll(CapsuleTop, CapsuleBottom, radius, transform.up, height, collisionLayer, (QueryTriggerInteraction)(collideWithTrigges ? 2 : 1));
        hitBuffer = Physics.OverlapCapsule(CapsuleTop, CapsuleBottom, Radius, collisionLayer, (QueryTriggerInteraction)(collideWithTrigges ? 2 : 1));
        if (hitBuffer.Length > 0)
        {
            for (int j = 0; j < hitBuffer.Length; j++)
            {
                if (capsuleCollider == hitBuffer[j]) { continue; }
                if (hitBuffer[j].isTrigger) { continue; }

                if (Physics.ComputePenetration(capsuleCollider,
                                               move,
                                               transform.rotation,
                                               hitBuffer[j],
                                               hitBuffer[j].bounds.center,
                                               hitBuffer[j].transform.rotation,
                                               out Vector3 direction,
                                               out float distance))
                {
                    ResolveIntersection(direction, distance);
                }

                //onControllerHit?.Invoke(hitBuffer[j]);
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

        Velocity = (transform.position - previousPos)/Time.deltaTime;
    }

    bool GroundCheck()
    {
        int hits = Physics.OverlapSphereNonAlloc(CapsuleBottom, Radius, groundCollisionBuffer, groundLayers);
        for (int i = 0; i < hits; i++)
        {
            if (groundCollisionBuffer[i] == capsuleCollider)
            {
                hits -= 1;
                break;
            }
        }

        return hits > 0;
    }

    void GroundChecking()
    {
        Ray ray = new Ray(CapsuleBottom, Vector3.down);

        if (Physics.SphereCast(ray, Radius, out RaycastHit tempHit, 20))
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
        int num = Physics.OverlapSphereNonAlloc(CapsuleBottom, Radius, col);

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

    private void OnDrawGizmos()
    { 
        for (int j = 0; j < hitBuffer.Length; j++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(hitBuffer[j].bounds.center, 0.1f);
        }
    }
}
