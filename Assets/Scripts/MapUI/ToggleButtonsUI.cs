using UnityEngine;
using UnityEngine.UI;

namespace MapUI
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