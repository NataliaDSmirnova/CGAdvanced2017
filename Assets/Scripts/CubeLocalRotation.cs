using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLocalRotation : MonoBehaviour
{
    float rotx = 0f;
    float roty = 0f;
    public float rotationSpeedX = 10f;
    public float rotationSpeedY = 10f;

    Vector3 upFromWorld;
    Vector3 rightFromWorld;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            rotx = Input.GetAxis("Mouse X") * rotationSpeedX;
            roty = Input.GetAxis("Mouse Y") * rotationSpeedY;

            upFromWorld = transform.InverseTransformVector(Vector3.up);
            rightFromWorld = transform.InverseTransformVector(Vector3.right);

            transform.localRotation *= Quaternion.AngleAxis(-rotx, upFromWorld) * Quaternion.AngleAxis(roty, rightFromWorld);
        }
    }
}