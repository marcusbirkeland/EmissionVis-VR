using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using UnityEngine;

namespace Editor.Spawner.RadiationSpawner
{
    /// <summary>
    /// The FullScaleRadiationSpawner class is a derived class of BaseRadiationSpawner,
    /// designed for creating and managing full-scale radiation visualizations in a 3D map
    /// using the Esri ArcGIS Maps SDK.
    /// </summary>
    public class FullScaleRadiationSpawner : BaseRadiationSpawner
    {
        /// <summary>
        /// Initializes a new instance of the FullScaleRadiationSpawner class.
        /// </summary>
        /// <param name="mapName">The name of the map to spawn the radiation visualization in.</param>
        /// <param name="cdfFilePath">The file path of the Cloud Data File (CDF).</param>
        /// <param name="map">The GameObject representing the map in the Unity scene.</param>
        /// <param name="rotationAngle">The rotation angle for the radiation visualization.</param>
        public FullScaleRadiationSpawner(string mapName, string cdfFilePath, GameObject map, float rotationAngle) 
            : base(mapName, cdfFilePath, map, rotationAngle)
        {
        }

        /// <summary>
        /// Creates the radiation holder GameObject for the full-scale radiation visualization.
        /// </summary>
        protected override void CreateRadiationHolder()
        {
            RadiationHolder = new GameObject(HolderName);
            RadiationHolder.transform.SetParent(Map.transform, false);

            RadiationHolder.AddComponent<HPTransform>();

            ArcGISLocationComponent location = RadiationHolder.AddComponent<ArcGISLocationComponent>();
            location.runInEditMode = true;
            
            location.Position = new ArcGISPoint(SelectedDatasetScope.position.lon, SelectedDatasetScope.position.lat, 350.0f,
                ArcGISSpatialReference.WGS84());
            
            location.Rotation = new ArcGISRotation(RotationAngle, 90, 0);
        }
    }
}
