using UnityEngine;
using UnityEngine.SceneManagement;

namespace MapUiComponents
{
    public class FollowCamera : MonoBehaviour
    {
        private Transform _xrCamera;
        [SerializeField] private float followDistance = 2.0f;
        [SerializeField] private float rotationSpeed = 3.0f;
        [SerializeField] private float maxRotationAngle = 60.0f;
        [SerializeField] private float moveSpeed = 5.0f;
        [SerializeField] private float tiltAngle = 10.0f;
        [SerializeField] private float heightOffset = -0.1f;


        private Quaternion _previousCameraRotation;
        private Vector3 _targetPosition;
        
        
        private void SetStartPosition()
        {
            UpdateTargetPosition();

            // Set the position of the UI to match the target position
            transform.position = _targetPosition;

            _previousCameraRotation = _xrCamera.rotation;
        }


        private void Start()
        {
            _xrCamera = Camera.main.transform;

            SetStartPosition();

            if (MapUI.Instance != null)
            {
                MapUI.Instance.OnToggle += HandleToggle;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (MapUI.Instance != null)
            {
                MapUI.Instance.OnToggle -= HandleToggle;
            }
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

            // Maintain the same vertical position as the camera and adjust it slightly below
            _targetPosition.y = _xrCamera.position.y + heightOffset;
        }
        

        private void RotateUIFacingCamera()
        {
            Vector3 directionToCamera = _xrCamera.position - transform.position;
            Vector3 directionToCameraHorizontal = Vector3.ProjectOnPlane(directionToCamera, Vector3.up);
            Quaternion targetRotation = Quaternion.LookRotation(-directionToCameraHorizontal);

            // Add a slight tilt towards the camera
            targetRotation *= Quaternion.Euler(tiltAngle, 0, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }


        private void HandleToggle(bool isActive)
        {
            if (isActive)
            {
                SetStartPosition();
            }
        }
        
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _xrCamera = Camera.main.transform;
            
            SetStartPosition();
        }
    }
}
