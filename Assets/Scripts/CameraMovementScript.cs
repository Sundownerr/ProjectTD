using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementScript : ExtendedMonoBehaviour
{   
    private Rigidbody camRigidBody;
    private float speed, boundary, rotationX;

    private void Start()
    {     
        camRigidBody = GetComponent<Rigidbody>();

        speed = 2000f;
        boundary = 10f;

        rotationX = 54f;  
    }

    private void FixedUpdate()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (rotationX > 33)
            {
                rotationX -= 2;
                camRigidBody.AddTorque(Vector3.left * 7);
                camRigidBody.AddForce(Vector3.down * 2500);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (rotationX < 82)
            {
                rotationX += 2;
                camRigidBody.AddTorque(Vector3.right * 7);
                camRigidBody.AddForce(Vector3.up * 2500);
            }
        }

        var onLeftEdge = Input.mousePosition.x > Screen.width - boundary;
        var onRightEdge = Input.mousePosition.x < 0 + boundary;
        var onTopEdge = Input.mousePosition.y > Screen.height - boundary;
        var onBottomEdge = Input.mousePosition.y < 0 + boundary;

        var vel = Vector3.zero;

        if (transform.position.x < 240f & onLeftEdge)
            vel = Vector3.right * speed;

        if (transform.position.x > -305f & onRightEdge)
            vel = Vector3.left * speed;

        if (transform.position.z > -700f & onBottomEdge)
            vel = Vector3.back * speed;

        if (transform.position.z < 104f & onTopEdge)
            vel = Vector3.forward * speed;

        camRigidBody.AddForce(vel);
    }
}






