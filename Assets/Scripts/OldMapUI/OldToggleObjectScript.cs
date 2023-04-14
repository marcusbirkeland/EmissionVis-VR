using Microsoft.Maps.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace OldMapUI
{
    //Toggles the visibility of a data gameObject.
    //Works with both Bing maps MapPins, and regular gameObjects.
    public class OldToggleObjectScript : MonoBehaviour
    {
        [Tooltip("*ONLY for Bing map* (can't toggle active state of game objects containing a map pin component).")]
        public MapPin pinToToggle;

        [Tooltip("*ONLY for ArcGIS map*")]
        public GameObject objectToToggle;
        
        public void ToggleObjectActiveState()
        {
            if (pinToToggle) pinToToggle.enabled = gameObject.GetComponent<Toggle>().isOn;
            if (objectToToggle) objectToToggle.SetActive(gameObject.GetComponent<Toggle>().isOn);
        }
    }
}
