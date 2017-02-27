using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLocalRotation : MonoBehaviour
{
    float rotx = 0f;
    float roty = 0f;
    public readonly float rotation_speed_x = 10f;
    public readonly float rotation_speed_y = 10f;

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
            rotx = Input.GetAxis("Mouse X") * rotation_speed_x;
            roty = Input.GetAxis("Mouse Y") * rotation_speed_y;

            upFromWorld = transform.InverseTransformVector(Vector3.up);
            rightFromWorld = transform.InverseTransformVector(Vector3.right);
            
            transform.localRotation *= Quaternion.AngleAxis(-rotx, upFromWorld) * Quaternion.AngleAxis(roty, rightFromWorld);        }
    }
}