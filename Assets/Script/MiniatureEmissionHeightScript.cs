using UnityEngine;
using UnityEngine.UI;
using Microsoft.Maps.Unity;



public class MiniatureEmissionHeightScript : MonoBehaviour
{
    [SerializeField]
    private MapPin currentAltitude;

    [SerializeField]
    private Slider altitudeSlider;

   

    private void Start()
    {
        altitudeSlider.onValueChanged.AddListener(onValueChanged);
        altitudeSlider.value = (float)currentAltitude.Altitude;
    }

    private void onValueChanged(float value)
    {
        currentAltitude.Altitude = value;
    }

    
    
}
