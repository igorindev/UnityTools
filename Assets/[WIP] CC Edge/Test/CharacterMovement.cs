using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField, Min(0)] float groundRadius = 0.2f;

    CharacterController characterController;
    CharacterControllerEdge characterControllerEdge;

    void OnValidate()
    {
        characterController = GetComponent<CharacterController>();
        characterControllerEdge = GetComponent<CharacterControllerEdge>();
    }

    void Update()
    {
        Vector3 m = Vector3.zero;
        if (Input.GetKey(KeyCode.D)) { m = Vector3.right; }
        else if (Input.GetKey(KeyCode.A)) { m = -Vector3.right; }

        Vector3 edgeDirection = characterControllerEdge.CheckEdge();

        characterController.Move(3 * Time.deltaTime * (m + (Vector3.down * 1) + edgeDirection));
    }
}