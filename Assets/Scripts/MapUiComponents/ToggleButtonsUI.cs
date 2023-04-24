using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MapUiComponents
{
    public class ToggleButtonsUI : MonoBehaviour
    {
        [SerializeField]
        private Toggle buildingToggle;

        [SerializeField]
        private Toggle radiationToggle;

        private void Start()
        {
            buildingToggle.onValueChanged.AddListener(ToggleBuildings);
            radiationToggle.onValueChanged.AddListener(ToggleRadiation);
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ToggleBuildings(buildingToggle.isOn);
            ToggleRadiation(radiationToggle.isOn);
        }

        private void ToggleBuildings(bool value)
        {
            ToggleObjectScript.ToggleActiveState(MapUI.Instance.BuildingHolder, value);
        }

        private void ToggleRadiation(bool value)
        {
            ToggleObjectScript.ToggleActiveState(MapUI.Instance.RadiationHolder, value);
        }
    }
}