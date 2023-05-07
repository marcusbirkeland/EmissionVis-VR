using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MapUiComponents
{
    /// <summary>
    /// The ToggleButtonsUI class manages the building and radiation toggle buttons,
    /// updating the visibility of the corresponding game objects based on the toggle state.
    /// </summary>
    public class ToggleButtonsUI : MonoBehaviour
    {
        [SerializeField]
        private Toggle buildingToggle;

        [SerializeField]
        private Toggle radiationToggle;

        /// <summary>
        /// Initializes the toggle button UI by setting up event listeners.
        /// </summary>
        private void Start()
        {
            buildingToggle.onValueChanged.AddListener(ToggleBuildings);
            radiationToggle.onValueChanged.AddListener(ToggleRadiation);
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Removes the event listener when the object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Handles scene changes by updating the visibility of buildings and radiation
        /// based on the current toggle states.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The scene loading mode.</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ToggleBuildings(buildingToggle.isOn);
            ToggleRadiation(radiationToggle.isOn);
        }

        /// <summary>
        /// Toggles the visibility of the buildings.
        /// </summary>
        /// <param name="value">True to show buildings, false to hide them.</param>
        private static void ToggleBuildings(bool value)
        {
            ToggleObjectScript.ToggleActiveState(MapUI.Instance.BuildingHolder, value);
        }

        /// <summary>
        /// Toggles the visibility of the radiation visualization.
        /// </summary>
        /// <param name="value">True to show radiation, false to hide it.</param>
        private static void ToggleRadiation(bool value)
        {
            ToggleObjectScript.ToggleActiveState(MapUI.Instance.RadiationHolder, value);
        }
    }
}
