using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLocalRotation : MonoBehaviour {
    Quaternion originalLocalRotation;
    float angle = 0f;
    float rotation_speed = 15f;
	// Use this for initialization
	void Start () {
        originalLocalRotation = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(1))
        {
            angle -= rotation_speed * Input.GetAxis("Mouse X");
            if (angle < -360f)
                angle += 360f;
            if (angle > 360f)
                angle -= 360f;
            transform.localRotation = originalLocalRotation * Quaternion.Euler(0, angle, 0);
        }
    }
}
