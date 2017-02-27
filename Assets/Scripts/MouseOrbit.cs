using System;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Class <c>MouseOrbit</c> implements to operate the camera.
    /// </summary>

    [AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
    public class MouseOrbit : MonoBehaviour
    {
        private Transform _target;
        private float _distance = 2f;

        /// <summary>
        /// The instance variables
        /// <c>xSpeed</c>, <c>ySpeed</c>,
        /// <c>yMinLimit</c>, <c>yMaxLimit</c>,
        /// <c>distanceMin</c>, <c>distanceMax</c>
        /// define the parameters of the camera.
        /// </summary>
        public readonly float XSpeed = 120f;
        public readonly float YSpeed = 120f;

        public readonly float YMinLimit = -85f;
        public readonly float YMaxLimit = 85f;

        public readonly float DistanceMin = 2f;
        public readonly float DistanceMax = 15f;

        private const float DeltaPosition = 0.02f;
        private const float DeltaGetAxis = 5f;

        private float _xEulerAngles = 0.0f;
        private float _yEulerAngles = 0.0f;
        private float _zoom = 0.0f;

        void Start()
        {
            _target = transform;
            _distance = -_target.position.z;

            var angles = transform.eulerAngles;
            _xEulerAngles = angles.x;
            _yEulerAngles = angles.y;
        }

        void LateUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                Debug.Log("Pressed left click.");
                SphericalMovement();
            }
            else if (Math.Abs((_zoom = Input.GetAxis("Mouse ScrollWheel"))) > 1E-6)
            {
                Debug.Log("Mouse scroll wheel click.");
                Translate();
                SphericalMovement();
            }
        }

        private void SphericalMovement()
        {
            _xEulerAngles += Input.GetAxis("Mouse X") * XSpeed * _distance * DeltaPosition;
            _yEulerAngles -= Input.GetAxis("Mouse Y") * YSpeed * DeltaPosition;

            _yEulerAngles = ClampAngle(_yEulerAngles, YMinLimit, YMaxLimit);

            var rotation = Quaternion.Euler(_yEulerAngles, _xEulerAngles, 0);

            _distance = Mathf.Clamp(
                _distance - Input.GetAxis("Mouse ScrollWheel") * DeltaGetAxis,
                DistanceMin,
                DistanceMax);

            RaycastHit hit;
            if (Physics.Linecast(_target.position, transform.position, out hit))
            {
                _distance -= hit.distance;
            }

            var negDistance = new Vector3(0.0f, 0.0f, -_distance);
            var position = rotation * negDistance;

            transform.rotation = rotation;
            transform.position = position;
        }

        private void Translate()
        {
            var rotation = Quaternion.Euler(_yEulerAngles, _xEulerAngles, 0);
            _distance = Mathf.Clamp(_distance - _zoom * DeltaGetAxis, DistanceMin, DistanceMax);

            RaycastHit hit;
            if (Physics.Linecast(_target.position, transform.position, out hit))
            {
                _distance -= hit.distance;
            }

            var negDistance = new Vector3(0.0f, 0.0f, -_distance);
            Vector3 position;
            if (_zoom > 0)
            {
                position = _target.position - (rotation * negDistance) * DeltaPosition;
            }
            else
            {
                position = (rotation * negDistance) * DeltaPosition + _target.position;
            }
            transform.position = position;
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
}