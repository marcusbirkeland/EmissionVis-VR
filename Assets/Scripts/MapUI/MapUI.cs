using UnityEngine;
using UnityEngine.InputSystem;
using Visualization;

namespace MapUI
{
    //Main class for mapUI, makes the UI unique and lets it persist across scenes.
    //Mainly serves as a way to easily access its daughter components.
    public class MapUI : MonoBehaviour
    {
        public static MapUI Instance { get; private set; }
        
        [SerializeField]
        private InputActionReference [] inputActions;

        //Must be static to be accessible with editor script
        public CloudManager cloudManager;
        public GameObject buildingHolder;
        public GameObject radiationHolder;
        
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
            
            foreach(InputActionReference iar in inputActions){
                iar.action.started += Toggle;
            }

            gameObject.SetActive(false);
        }
        
        private void OnDestroy() {
            foreach(InputActionReference iar in inputActions){
                iar.action.started -= Toggle;
            }
        }
        
        private void Toggle(InputAction.CallbackContext context = default){
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
