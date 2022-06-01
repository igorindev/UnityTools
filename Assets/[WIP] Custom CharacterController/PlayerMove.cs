using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 100;
    // Update is called once per frame
    void Update()
    {
       float x = Input.GetAxis("Horizontal");
       float z = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x, 0, z) * speed * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
           // move.y = 10f;
        }
        else
        {
            //move.y = -0.1f;
        }
        
        GetComponent<CustomCharacterController>().Move(transform.TransformDirection(move));

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Do();
        }
    }

    void Do()
    {
        for (int i = 0; i < 10; i++)
        {
            //MathExtension.RandomVector(out Vector3 v, Vector3.zero, Vector3.one * 100f);
            //GetComponent<CustomCharacterController>().Move(v);
        }

        transform.position = Vector3.up;

        for (int i = 0; i < 6; i++)
        {
            Vector3 move = new Vector3(0, 0, 1) * 10 * Time.fixedDeltaTime;
            GetComponent<CustomCharacterController>().Move(move);
        }
    }
}