using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using Microsoft.Maps.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace MapUiComponents
{
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

        //TODO: Reduce function calls by storing locations somewhere.
        private static void UpdateCloudHeight(float value)
        {
            MapUI.CloudManager.ChangeCurvatureByHeight(value);

            const int heightValueMultiplier = 1000;

            MapPin mapPin = MapUI.Instance.CloudHolder.GetComponent<MapPin>();
            if(mapPin){
                mapPin.Altitude = MapUI.CloudManager.baseElevation + value*heightValueMultiplier;
                return;
            }
            
            ArcGISLocationComponent arcGisLocation = MapUI.Instance.CloudHolder.GetComponent<ArcGISLocationComponent>();
            if (!arcGisLocation) return;
            
            Debug.Log("Adjusting arcgis position");
                
            arcGisLocation.Position = new ArcGISPoint(
                arcGisLocation.Position.X, 
                arcGisLocation.Position.Y, 
                MapUI.CloudManager.baseElevation + value*heightValueMultiplier, 
                ArcGISSpatialReference.WGS84()
            );
        }
    }
}