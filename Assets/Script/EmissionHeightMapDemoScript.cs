using UnityEngine;
using UnityEngine.UI;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.Unity.Location;

public class EmissionHeightMapDemoScript : MonoBehaviour
{
    public ArcGISCameraController cameraController;
    public Slider altitudeSlider;

    private void Start()
    {
        // Set the initial value of the slider to the camera's current altitude
        altitudeSlider.value = cameraController.Camera.transform.position.y;
    }

    public void OnSliderValueChanged(float value)
    {
        // Change the altitude of the camera
        Vector3 cameraPosition = cameraController.Camera.transform.position;
        cameraPosition.y = value;
        cameraController.Camera.transform.position = cameraPosition;
    }
}