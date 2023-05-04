using UnityEngine;
using UnityEngine.SceneManagement;

namespace MapUiComponents
{
    /// <summary>
    /// The FollowCamera class is responsible for making the mapUI stay in view of the player.
    /// </summary>
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
        
        /// <summary>
        /// Sets the starting position of the UI element.
        /// </summary>
        private void SetStartPosition()
        {
            UpdateTargetPosition();
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
            float angleDifference = Quaternion.Angle(_previousCameraRotation, _xrCamera.rotation);

            if (angleDifference >= maxRotationAngle)
            {
                UpdateTargetPosition();
                _previousCameraRotation = _xrCamera.rotation;
            }

            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * moveSpeed);
            RotateUIFacingCamera();
        }

        /// <summary>
        /// Updates the target position of the UI element based on the camera's horizontal forward vector.
        /// </summary>
        private void UpdateTargetPosition()
        {
            Vector3 cameraForwardHorizontal = Vector3.ProjectOnPlane(_xrCamera.forward, Vector3.up).normalized;
            _targetPosition = _xrCamera.position + cameraForwardHorizontal * followDistance;
            _targetPosition.y = _xrCamera.position.y + heightOffset;
        }
        
        /// <summary>
        /// Rotates the UI element to face the camera.
        /// </summary>
        private void RotateUIFacingCamera()
        {
            Vector3 directionToCamera = _xrCamera.position - transform.position;
            Vector3 directionToCameraHorizontal = Vector3.ProjectOnPlane(directionToCamera, Vector3.up);
            Quaternion targetRotation = Quaternion.LookRotation(-directionToCameraHorizontal);
            targetRotation *= Quaternion.Euler(tiltAngle, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Handles the toggle event for the UI element.
        /// </summary>
        /// <param name="isActive">Indicates whether the UI element is active or not.</param>
        private void HandleToggle(bool isActive)
        {
            if (isActive)
            {
                SetStartPosition();
            }
        }
        
        /// <summary>
        /// Resets the starting position of the UI element when a new scene is loaded.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The loading mode for the scene.</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _xrCamera = Camera.main.transform;
            SetStartPosition();
        }
    }
}

