using System;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    CharacterController controller;
    //PlayerInput inputActions;
    public Transform playerPos;
    public Transform outBoatPos;

    [Header("Movement")]
    public float maxSpeed = 3f;
    public float acceleration = 10f;
    public float deacceleration = 3f;
    float speed = 0; //Debug

    [Header("Rotation")]
    public float turnMaxSpeed = 10f;
    public float rotAcceleration = 10f;
    public float rotDeacceleration = 3f;
    float rotSpeed = 0; //Debug

    Vector3 move;
    Vector3 rotate;

    //bool reflect = false;

    // Start is called before the first frame update
    void Awake()
    {
        //inputActions = new PlayerInput();
        controller = GetComponent<CharacterController>();

       //inputActions.Boat.Movement.performed += ctx => Movement(ctx.ReadValue<Vector2>());
       //inputActions.Boat.Movement.canceled += ctx => Movement(Vector2.zero);
    }

    void Update()
    {
        if (move.z > 0)
        {
            speed += acceleration * Time.deltaTime;
        }
        else if (move.z < 0 && speed > 0)
        {
            speed -= deacceleration * 3 * Time.deltaTime;
        }
        else if (speed > 0)
        {
            speed -= deacceleration * Time.deltaTime;
        }
        else
        {
            speed = 0;
        }

        if (rotate.y != 0)
        {
            rotSpeed += rotAcceleration * Time.deltaTime * rotate.y;
        }
        else if (rotSpeed > 0)
        {
            rotSpeed -= rotDeacceleration * Time.deltaTime;
        }
        else if (rotSpeed < 0)
        {
            rotSpeed += rotDeacceleration * Time.deltaTime;
        }
        else
        {
            rotSpeed = 0;
        }

        speed = Mathf.Clamp(speed, 0f, maxSpeed);
        rotSpeed = Mathf.Clamp(rotSpeed, -turnMaxSpeed, turnMaxSpeed);

        Vector3 movement = transform.forward * speed; //+ Physics.gravity;

        controller.Move(movement * Time.deltaTime);
        //rb.MovePosition(transform.position + transform.forward * speed);

        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, 1, 0) * rotSpeed * Time.deltaTime);
        transform.rotation = transform.rotation * deltaRotation;

        //rb.MoveRotation(transform.rotation * deltaRotation);
    }


    void Collide()
    {

    }

    void Movement(Vector2 movement)
    {
        move = new Vector3(0, 0, movement.normalized.y);
        rotate = new Vector3(0, movement.normalized.x, 0);
    }


    private void OnEnable()
    {
        //inputActions.Enable();
    }

    private void OnDisable()
    {
        //inputActions.Disable();
    }

}
