using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>MouseOrbitImproved</c> implements to operate the camera.
/// </summary>

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour
{
    public Transform target;
    public float distance = 2f;

    /// <summary>
    /// The instance variables
    /// <c>xSpeed</c>, <c>ySpeed</c>,
    /// <c>yMinLimit</c>, <c>yMaxLimit</c>,
    /// <c>distanceMin</c>, <c>distanceMax</c>
    /// define the parameters of the camera.
    /// </summary>
    public readonly float xSpeed = 120f;
    public readonly float ySpeed = 120f;

    public readonly float yMinLimit = -85f;
    public readonly float yMaxLimit = 85f;

    public readonly float distanceMin = 2f;
    public readonly float distanceMax = 15f;

    public float x = 0.0f;
    public float y = 0.0f;
    public float z = 0.0f;
    private float zoom = 0.0f;

    void Start() {
        target = GameObject.Find("MainCamera").transform;
        distance = -target.position.z;

        Vector3 angles = transform.eulerAngles;
        x = angles.x;
        y = angles.y;
        z = angles.z;
    }
    
    void LateUpdate() {
        if (Input.GetMouseButton(0)) {
            Debug.Log("Pressed left click.");
            SphericalMovement();
        }

        if ((zoom = Input.GetAxis("Mouse ScrollWheel")) != 0) {
            Debug.Log("Mouse scroll wheel click.");
            Translate();
            SphericalMovement();
        }
    }

    private void SphericalMovement() {
        x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

        y = ClampAngle(y, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(y, x, 0);

        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

        RaycastHit hit;
        if (Physics.Linecast(target.position, transform.position, out hit)) {
            distance -= hit.distance;
        }

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance;

        transform.rotation = rotation;
        transform.position = position;
    }

    private void Translate() {
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        distance = Mathf.Clamp(distance - zoom * 5, distanceMin, distanceMax);

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

    public static float ClampAngle(float angle, float min, float max) {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}