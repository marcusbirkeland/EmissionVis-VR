using UnityEngine;

namespace MapUI
{
    public class FollowCamera : MonoBehaviour
    {
        private Transform _xrCamera;
        [SerializeField] private float followDistance = 2.0f;
        [SerializeField] private float rotationSpeed = 3.0f;
        [SerializeField] private float maxRotationAngle = 60.0f;
        [SerializeField] private float moveSpeed = 5.0f;

        private Quaternion _previousCameraRotation;
        private Vector3 _targetPosition;

        private void Awake()
        {
            // If the xrCamera is not assigned, use the main camera
            if (_xrCamera == null)
            {
                _xrCamera = Camera.main.transform;
            }

            UpdateTargetPosition();
            transform.position = _targetPosition;
            _previousCameraRotation = _xrCamera.rotation;
        }

        private void Update()
        {
            // Calculate the angle difference between the current and previous camera rotations
            float angleDifference = Quaternion.Angle(_previousCameraRotation, _xrCamera.rotation);

            // Update the target position if the angle difference exceeds the maximum allowed rotation angle
            if (angleDifference >= maxRotationAngle)
            {
                UpdateTargetPosition();
                _previousCameraRotation = _xrCamera.rotation;
            }

            // Move the UI smoothly towards the target position
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * moveSpeed);

            // Rotate the UI to face the camera
            RotateUIFacingCamera();
        }

        private void UpdateTargetPosition()
        {
            // Project the camera's forward vector onto the horizontal plane
            Vector3 cameraForwardHorizontal = Vector3.ProjectOnPlane(_xrCamera.forward, Vector3.up).normalized;

            // Update the target position based on the camera's horizontal forward vector
            _targetPosition = _xrCamera.position + cameraForwardHorizontal * followDistance;

            // Maintain the current vertical position
            _targetPosition.y = transform.position.y;
        }


        private void RotateUIFacingCamera()
        {
            Vector3 directionToCamera = _xrCamera.position - transform.position;
            Vector3 directionToCameraHorizontal = Vector3.ProjectOnPlane(directionToCamera, Vector3.up);
            Quaternion targetRotation = Quaternion.LookRotation(-directionToCameraHorizontal);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
