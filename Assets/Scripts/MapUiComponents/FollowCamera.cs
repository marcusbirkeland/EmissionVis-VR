using UnityEngine;
using UnityEngine.SceneManagement;

namespace MapUiComponents
{
    /// <summary>
    /// The FollowCamera class is responsible for making the mapUI stay in view of the player.
    /// </summary>
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] 
        private float followDistance = 2.0f;
        
        [SerializeField] 
        private float rotationSpeed = 3.0f;
        
        [SerializeField] 
        private float maxRotationAngle = 60.0f;
        
        [SerializeField] 
        private float moveSpeed = 5.0f;
        
        [SerializeField] 
        private float tiltAngle = 10.0f;
        
        [SerializeField] 
        private float heightOffset = -0.1f;
        
        private Transform _xrCamera;
        private Quaternion _previousCameraRotation;
        private Vector3 _targetPosition;

        
        private void Start()
        {
            _xrCamera = Camera.main.transform;
            SnapToTargetPosition();

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
        /// Updates target position and snaps the UI to it.
        /// </summary>
        private void SnapToTargetPosition()
        {
            UpdateTargetPosition();
            transform.position = _targetPosition;
            _previousCameraRotation = _xrCamera.rotation;
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
        /// Responds to the mapUI getting toggled. Snaps to the target position if the UI gets toggled on.
        /// </summary>
        /// <param name="isActive">The mapUIs current state.</param>
        private void HandleToggle(bool isActive)
        {
            if (isActive)
            {
                SnapToTargetPosition();
            }
        }
        
        
        /// <summary>
        /// Triggered when the scene changes. Updates the camera value, and snaps to target position.
        /// </summary>
        /// <param name="scene">The loaded scene. (unused)</param>
        /// <param name="mode">The loading mode for the scene. (unused)</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _xrCamera = Camera.main.transform;
            SnapToTargetPosition();
        }
    }
}

