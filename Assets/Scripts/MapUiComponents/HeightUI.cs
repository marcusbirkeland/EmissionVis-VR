
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using Microsoft.Maps.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace MapUiComponents
{
    /// <summary>
    /// The HeightUI class is responsible for managing the height slider and updating
    /// the cloud height based on the slider's value.
    /// </summary>
    public class HeightUI : MonoBehaviour
    {
        [SerializeField]
        private Slider heightSlider;

        private void Start()
        {
            heightSlider.onValueChanged.AddListener(
                UpdateCloudHeight
            );
        }

        /// <summary>
        /// Updates the cloud height based on the slider value.
        /// </summary>
        /// <param name="value">The slider value.</param>
        private static void UpdateCloudHeight(float value)
        {
            MapUI.CloudManager.ChangeCurvatureByHeight(value);

            //The difference in meters between the minimum and maximum cloud position.
            const int maxHeightDifference = 1000;

            MapPin mapPin = MapUI.Instance.CloudHolder.GetComponent<MapPin>();
            if(mapPin){
                mapPin.Altitude = MapUI.CloudManager.baseElevation + value * maxHeightDifference;
                return;
            }
        
            ArcGISLocationComponent arcGisLocation = MapUI.Instance.CloudHolder.GetComponent<ArcGISLocationComponent>();
            if (!arcGisLocation) return;
        
            Debug.Log("Adjusting arcgis position");
            
            arcGisLocation.Position = new ArcGISPoint(
                arcGisLocation.Position.X, 
                arcGisLocation.Position.Y, 
                MapUI.CloudManager.baseElevation + value * maxHeightDifference, 
                ArcGISSpatialReference.WGS84()
            );
        }
    }

}