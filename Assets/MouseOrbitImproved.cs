using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour
{
    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    public float x = 0.0f;
    public float y = 0.0f;
    public float z = 0.0f;
    private float zoom;

    void Start() {
        target = GameObject.Find("MainCamera").transform;
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        z = angles.z;
    }

    void LateUpdate() {
        if (Input.GetMouseButton(0)) {
            Debug.Log("Pressed left click.");

            x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            transform.rotation = rotation;
        }

        if ((zoom = Input.GetAxis("Mouse ScrollWheel")) != 0) {
            Debug.Log("Mouse scroll wheel click.");

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            distance = Mathf.Clamp(distance - zoom, distanceMin, distanceMax);

            RaycastHit hit;
            if (Physics.Linecast(target.position, transform.position, out hit)) {
                distance -= hit.distance;
            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position;
            if (zoom > 0) {
                position = target.position - (rotation * negDistance) * 0.02f;
            } else {
                position = (rotation * negDistance) * 0.02f + target.position;
            }

            transform.position = position;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}