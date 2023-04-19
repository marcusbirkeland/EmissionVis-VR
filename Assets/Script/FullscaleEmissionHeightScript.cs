using UnityEngine;
using UnityEngine.UI;

public class FullscaleEmissionHeightScript : MonoBehaviour
{
    public Slider altitudeSlider;
    public Transform objectTransform;
    
    
    private float currentAltitude;

    void Start()
    {
        
        currentAltitude = objectTransform.position.y;
        altitudeSlider.onValueChanged.AddListener(onValueChanged);
    }

    private void onValueChanged(float value)
    {
       
        currentAltitude = altitudeSlider.value;
        Vector3 newPosition = objectTransform.position;
        newPosition.y = currentAltitude;
        objectTransform.position = newPosition;
    }
}
