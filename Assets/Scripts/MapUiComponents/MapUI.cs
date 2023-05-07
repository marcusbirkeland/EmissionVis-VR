using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Visualization;

namespace MapUiComponents
{
    /// <summary>
    /// The MapUI class is a singleton responsible for managing the UI elements in the scene, such as clouds,
    /// buildings, and radiation. It provides access to the scene elements for the mapUI subcomponents.
    /// </summary>
    public class MapUI : MonoBehaviour
    {
        public static MapUI Instance { get; private set; }
        
        [SerializeField]
        private InputActionReference [] inputActions;

        public GameObject CloudHolder { get; private set; }
        public GameObject BuildingHolder { get; private set; }
        public GameObject RadiationHolder { get; private set; }
        
        public string miniatureSceneName;
        public string fullScaleSceneName;

        private CloudManager _cloudManager;
        
        public static CloudManager CloudManager => Instance._cloudManager;

        public event Action<bool> OnToggle;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            
            foreach(InputActionReference iar in inputActions){
                iar.action.started += Toggle;
            }

            SetHolderValues();
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            foreach(InputActionReference iar in inputActions){
                iar.action.started -= Toggle;
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("New scene loaded, setting holder values");
            SetHolderValues();

            if (!CloudHolder || !BuildingHolder || !RadiationHolder)
            {
                Debug.Log("Missing a holder object");
            }
        }

        /// <summary>
        /// Gets the holder elements for clouds, buildings, and radiation objects from the current scene.
        /// </summary>
        private void SetHolderValues()
        {
            CloudHolder = GameObject.Find("Cloud Holder");

            if (CloudHolder != null)
            {
                _cloudManager = CloudHolder.GetComponentInChildren<CloudManager>();
            }
            
            BuildingHolder = GameObject.Find("Buildings Holder");
            RadiationHolder = GameObject.Find("Radiation Holder");
        }
        
        /// <summary>
        /// Toggles the visibility of the mapUI gameObject.
        /// </summary>
        /// <param name="context">The input action callback context (optional).</param>
        private void Toggle(InputAction.CallbackContext context = default)
        {
            gameObject.SetActive(!gameObject.activeSelf);

            Debug.Log("Toggled UI");

            OnToggle?.Invoke(gameObject.activeSelf);
        }
    }
}