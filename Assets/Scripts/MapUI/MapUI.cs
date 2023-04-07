using UnityEngine;

namespace MapUI
{
    //Main class for mapUI, makes the UI unique and lets it persist across scenes.
    //Mainly serves as a way to easily access its daughter components.
    public class MapUI : MonoBehaviour
    {
        public static MapUI Instance { get; private set; }

        public ToggleUI toggleUI;
        
        public ToggleObjectScript radiationToggle;
        public ToggleObjectScript buildingsToggle;

        public CloudPlaybackButton cloudPlaybackButton;
        

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
            }
        }
    }
}
