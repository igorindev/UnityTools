using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////NOT FINISHED JUST AN EXAMPLE

public class AttractObject : MonoBehaviour
{
    [SerializeField] float gravity = 9.81f;
    [SerializeField] float accelerationModifier = 2;

    [SerializeField] Transform target;
    Vector3 attractionForceVector;
    Rigidbody rb;
    float topSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();    
    }

    void FixedUpdate()
    {
        rb.AddForce(attractionForceVector);

        rb.velocity = Vector2.ClampMagnitude((target.position - transform.position).normalized * topSpeed, rb.velocity.magnitude);
    }

    void Attract()
    {
        float force = gravity * accelerationModifier / (target.position - transform.position).sqrMagnitude;
    }
}
