using UnityEngine;
using UnityEngine.UI;
using Microsoft.Maps.Unity;



public class EmissionHeightScript : MonoBehaviour
{
    [SerializeField]
    private MapPin _mapPin;

    [SerializeField]
    private Slider _slider;

   

    private void Start()
    {
        _slider.value = (float)_mapPin.Altitude;
    }

    private void Update()
    {
        _mapPin.Altitude = _slider.value;
    }

    
}

