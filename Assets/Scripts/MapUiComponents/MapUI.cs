using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MapUiComponents
{
    /// <summary>
    /// The MapUI class is a singleton responsible for managing the UI elements in the scene, such as clouds,
    /// buildings, and radiation. It provides access to scene elements for the mapUI subcomponents.
    /// </summary>
    public class MapUI : MonoBehaviour
    {
        /// <summary>
        /// Full name of the miniature scene.
        /// Used when switching scenes.
        /// </summary>
        public string miniatureSceneName;
        
        /// <summary>
        /// Full name of the full scale scene.
        /// Used when switching scenes.
        /// </summary>
        public string fullScaleSceneName;

        /// <summary>
        /// The VR interaction actions used to toggle the UI on/off.
        /// (Represents buttons and joystick actions with the VR controller.)
        /// </summary>
        [SerializeField]
        private InputActionReference [] inputActions;
        
        private CloudManager _cloudManager;

        /// <summary>
        /// The GameObject containing the cloud visualization.
        /// </summary>
        public GameObject CloudHolder { get; private set; }
        
        /// <summary>
        /// The GameObject containing the building Visualization.
        /// </summary>
        public GameObject BuildingHolder { get; private set; }
        
        /// <summary>
        /// The GameObject containing the radiation Visualization.
        /// </summary>
        public GameObject RadiationHolder { get; private set; }
        
        
        /// <summary>
        /// The singleton instance of the mapUI component.
        /// </summary>
        public static MapUI Instance { get; private set; }
        
        /// <summary>
        /// Provides static access to the CloudManager component.
        /// </summary>
        public static CloudManager CloudManager => Instance._cloudManager;

        
        /// <summary>
        /// Event letting other component know when the mapUI gets toggled.
        /// </summary>
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

        
        /// <summary>
        /// Handles scene change. Replaces the holder references.
        /// </summary>
        /// <param name="scene">The loaded scene. (unused)</param>
        /// <param name="mode">The scene loading mode. (unused)</param>
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
        /// Updates the holder elements for clouds, buildings, and radiation objects based on objects in the current scene.
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
        /// Toggles the visibility of the mapUI gameObject. Invokes the <see cref="OnToggle"/> event.
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