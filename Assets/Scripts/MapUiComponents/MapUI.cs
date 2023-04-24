using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Visualization;

namespace MapUiComponents
{
    public class MapUI : MonoBehaviour
    {
        public static MapUI Instance { get; private set; }
        
        [SerializeField]
        private InputActionReference [] inputActions;

        private GameObject CloudHolder { get; set; }
        public GameObject BuildingHolder { get; private set; }
        public GameObject RadiationHolder { get; private set; }
        
        public string miniatureSceneName;
        public string fullScaleSceneName;
        
        public static CloudManager CloudManager => Instance.CloudHolder.GetComponentInChildren<CloudManager>();

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

        private void OnDestroy() {
            foreach(InputActionReference iar in inputActions){
                iar.action.started -= Toggle;
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            SetHolderValues();
        }

        private void SetHolderValues() {
            CloudHolder = GameObject.Find("Cloud Holder");
            BuildingHolder = GameObject.Find("Buildings Holder");
            RadiationHolder = GameObject.Find("Radiation Holder");
        }
        
        private void Toggle(InputAction.CallbackContext context = default){
            gameObject.SetActive(!gameObject.activeSelf);

            Debug.Log("Toggled UI");

            OnToggle?.Invoke(gameObject.activeSelf);
        }
    }
}