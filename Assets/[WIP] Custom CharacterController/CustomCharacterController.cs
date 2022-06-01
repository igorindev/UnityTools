using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CustomControllerHit
{
    public Vector3 point;
    public Vector3 moveDirection;
    public Vector3 normal;
    public float moveLength;
    public Collider collider;
    public CustomCharacterController controller;
    public Transform transform;
    public GameObject gameObject;
}

[RequireComponent(typeof(CapsuleCollider))]
public class CustomCharacterController : MonoBehaviour
{
    [Header("Body")]
    public CapsuleCollider capsuleCollider;
    [SerializeField] bool collideWithTriggers = true;
    [SerializeField] LayerMask collisionLayer = ~0;
    [SerializeField] LayerMask discludePlayer;

    [Header("Movement")]
	//[SerializeField] float slopeLimit = 60;

	[Header("Ground")]
    [SerializeField] LayerMask groundLayers = ~0;

    public Action<CustomControllerHit> onControllerHit;

    public bool IsGrounded => grounded;

    float Radius => capsuleCollider.radius;
    float Height => capsuleCollider.height;
    float HalfHeight => Mathf.Max(Height * 0.5f, Radius);

    public Vector3 Center => capsuleCollider.center;
    Vector3 Bottom => Center - HalfHeight * transform.up;
    Vector3 Top => Center + HalfHeight * transform.up;

    public Vector3 WorldCenter => transform.position + Center;
    public Vector3 WorldBottom => transform.position + Bottom;
    public Vector3 WorldTop => transform.position + Top;

    public Vector3 CapsuleTop => WorldTop - (Radius * transform.up);
    public Vector3 CapsuleBottom => WorldBottom + (Radius * transform.up);

    public Vector3 velocity { get; private set; }

    Vector3 previousPos;
    Vector3 move;

    bool grounded;

    RaycastHit groundHit;

    readonly Collider[] groundCollisionBuffer = new Collider[6];
    Collider[] hitBuffer = new Collider[0];
    Collider[] allocatedGroundCollisions = new Collider[3];

    void Reset()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    ///TODO: PASS VELOCITY AND MODIFY 
    ///Vector3 velocityProjected = Vector3.Project (velocity, -dir);
    ///transform.position = transform.position + penetrationVector;
    ///vel -= velocityProjected;
    public void Move(Vector3 motion)
    {
        previousPos = transform.position;
        transform.position = transform.position + motion;

        //GroundChecking();
        CollisionDetection();
    }

	Vector3 ColliderPosition(Collider collider)
	{
		return collider.transform.position + collider.contactOffset * collider.transform.up;
	}

	void CollisionDetection()
    {
        hitBuffer = Physics.OverlapCapsule(CapsuleTop, CapsuleBottom, Radius, collisionLayer, (QueryTriggerInteraction)(collideWithTriggers ? 2 : 1));
        if (hitBuffer.Length > 0)
        {
            for (int j = 0; j < hitBuffer.Length; j++)
            {
                if (capsuleCollider == hitBuffer[j] || hitBuffer[j].isTrigger) { continue; }

                if (Physics.ComputePenetration(capsuleCollider,
                                               transform.position,
                                               transform.rotation,
                                               hitBuffer[j],
                                               hitBuffer[j].transform.position,
                                               hitBuffer[j].transform.rotation,
                                               out Vector3 direction,
                                               out float distance))
                {
                    ResolveIntersection(direction, distance);
                }
                
                Vector3 hitPoint = transform.TransformPoint(capsuleCollider.center) + (direction * (capsuleCollider.radius - distance));
                CustomControllerHit controllerHit = new CustomControllerHit
                {
                    point = hitPoint,
                    normal = direction,
                    controller = this,
                    collider = hitBuffer[j],
                    moveDirection = -direction,
                    moveLength = distance,
                    transform = hitBuffer[j].transform,
                    gameObject = hitBuffer[j].gameObject,
                };

                onControllerHit?.Invoke(controllerHit);
            }
        }

        ApplyResult(transform.position);
    }
    void ResolveIntersection(Vector3 direction, float distance)
    {
        transform.position += direction * distance;
    }
    void ApplyResult(Vector3 result)
    {
        transform.position = result;

        velocity = (transform.position - previousPos) / Time.deltaTime;
    }

    bool GroundCheck()
    {
        int numOfHits;
        int hits = Physics.OverlapSphereNonAlloc(CapsuleBottom, Radius, groundCollisionBuffer, groundLayers);
        numOfHits = hits;
        for (int i = 0; i < numOfHits; i++)
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
        Ray ray = new Ray(CapsuleBottom, -transform.up);
        if (Physics.Raycast(ray, out RaycastHit tempHit, Radius + 1f, discludePlayer))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }
    void GroundConfirm(RaycastHit tempHit)
    {
        int groundHits = Physics.OverlapSphereNonAlloc(CapsuleBottom, Radius, allocatedGroundCollisions, discludePlayer);

        grounded = false;

        for (int i = 0; i < groundHits; i++)
        {
            if (allocatedGroundCollisions[i].transform == tempHit.transform)
            {
                groundHit = tempHit;
                grounded = true;

                ResolveIntersection(tempHit.normal, Radius - tempHit.distance);
                break;
            }

        }
    }
    void OnDrawGizmos()
    {
        for (int j = 0; j < hitBuffer.Length; j++)
        {
            Gizmos.color = Color.yellow;
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(groundHit.point, 0.1f);
    }
}