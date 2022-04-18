using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerEdge : MonoBehaviour
{
    [SerializeField, Min(0)] float groundRadius = 0.2f;
    [SerializeField] Vector3 groundCheckPos;
    [SerializeField] LayerMask groundMask;

    CharacterController characterController;

    Vector3 edgeCheckPosition;
    Vector3 edgeCollision;

    Vector3 controllerGroundPos;
    Vector3 edgeDirection;

    Vector3 normalized;

    Collider[] colliders = new Collider[1];

    void OnValidate()
    {
        characterController = GetComponent<CharacterController>();
        groundRadius = groundRadius > characterController.radius ? characterController.radius : groundRadius;

        controllerGroundPos = (characterController.center - ((0.5f * characterController.height) + characterController.skinWidth) * Vector3.up);
    }

    public Vector3 CheckEdge()
    {
        edgeCheckPosition = transform.TransformPoint(controllerGroundPos);

        normalized = (edgeCollision.ZeroY() - edgeCheckPosition.ZeroY()).normalized;
        edgeDirection = -normalized;

        if (CheckGroundCollision())
        {

        }

        if (Vector3.Distance(edgeCheckPosition, edgeCollision) <= groundRadius || characterController.isGrounded == false)
        {
            return Vector3.zero;
        }

        return edgeDirection;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log(hit.collider.name);
        edgeCollision = hit.point;
    }

    bool CheckGroundCollision()
    {
        return Physics.OverlapSphereNonAlloc(transform.TransformPoint(groundCheckPos), groundRadius, colliders, groundMask, QueryTriggerInteraction.Ignore) > 0 ;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        controllerGroundPos = (characterController.center - ((0.5f * characterController.height) + characterController.skinWidth) * Vector3.up);
        edgeCheckPosition = transform.TransformPoint(controllerGroundPos);

        Handles.color = Color.red;
        Handles.DrawSolidDisc(edgeCheckPosition, transform.up, characterController.radius);

        Handles.color = Color.green;
        Handles.DrawSolidDisc(edgeCheckPosition, transform.up, groundRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(edgeCollision, 0.1f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(edgeCheckPosition, edgeDirection);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(edgeCheckPosition, normalized);
    }
#endif
}