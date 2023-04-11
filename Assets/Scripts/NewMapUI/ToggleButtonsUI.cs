using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NewMapUI
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
            NewToggleObjectScript.ToggleActiveState(MapUI.BuildingHolder, value);
        }

        private void ToggleRadiation(bool value)
        {
            NewToggleObjectScript.ToggleActiveState(MapUI.RadiationHolder, value);
        }
    }
}