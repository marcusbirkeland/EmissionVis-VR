using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using Microsoft.Maps.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MapUiComponents
{
    /// <summary>
    /// The HeightUI class is responsible for managing the height slider and updating
    /// the cloud height based on the slider's value.
    /// </summary>
    public class HeightUI : MonoBehaviour
    {
        /// <summary>
        /// Represents a number of meters.
        /// </summary>
        /// <remarks>
        /// TODO: This value should be replaced by a user selected value.
        /// </remarks>
        private const int MaxHeightDifference = 1000;
        
        [SerializeField]
        private Slider heightSlider;

        
        private void Start()
        {
            heightSlider.onValueChanged.AddListener(
                UpdateCloudHeight
            );

            SceneManager.sceneLoaded += HandleSceneChange;
        }

        
        /// <summary>
        /// Updates the cloud height based on the slider value.
        /// </summary>
        /// <param name="value">The slider value.</param>
        /// <remarks>
        /// TODO: GameObject height logic should get moved somewhere else. It is not relevant to the UI.
        /// </remarks>
        private static void UpdateCloudHeight(float value)
        {
            MapUI.CloudManager.ChangeCurvatureByHeight(value);
            
            MapPin mapPin = MapUI.Instance.CloudHolder.GetComponent<MapPin>();
            if(mapPin){
                mapPin.Altitude = MapUI.CloudManager.baseElevation + value * MaxHeightDifference;
                return;
            }
        
            ArcGISLocationComponent arcGisLocation = MapUI.Instance.CloudHolder.GetComponent<ArcGISLocationComponent>();
            if (!arcGisLocation) return;
        
            Debug.Log("Adjusting arcgis position");
            
            arcGisLocation.Position = new ArcGISPoint(
                arcGisLocation.Position.X, 
                arcGisLocation.Position.Y, 
                MapUI.CloudManager.baseElevation + value * MaxHeightDifference, 
                ArcGISSpatialReference.WGS84()
            );
        }
        
        
        /// <summary>
        /// Called when the scene changes. Updates the cloud height based on the slider value.
        /// </summary>
        /// <param name="scene">The loaded scene. (unused)</param>
        /// <param name="mode">The scene loading mode. (unused)</param>
        private void HandleSceneChange(Scene scene, LoadSceneMode mode)
        {
            UpdateCloudHeight(heightSlider.value);
        }    
    }
}