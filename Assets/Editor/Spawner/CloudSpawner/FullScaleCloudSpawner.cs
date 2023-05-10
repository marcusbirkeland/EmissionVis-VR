using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using UnityEngine;

namespace Editor.Spawner.CloudSpawner
{
    /// <summary>
    /// The FullScaleCloudSpawner class is responsible for spawning a full-scale cloud
    /// in the scene based on the provided data.
    /// </summary>
    public class FullScaleCloudSpawner : BaseCloudSpawner
    {
        /// <summary>
        /// Prefab variant specialized for the full scale model
        /// </summary>
        protected override string PrefabName => "Cloud Full Scale";
        
        /// <summary>
        /// Base elevation in the full scale scene. Represents meters above sea level.
        /// </summary>
        protected override double Elevation => 150;

        
        /// <summary>
        /// Initializes a new instance of the FullScaleCloudSpawner class.
        /// </summary>
        /// <param name="mapName">The name of the map being used.</param>
        /// <param name="cdfFilePath">The file path to the netCDF file containing the data.</param>
        /// <param name="map">The map GameObject in the scene.</param>
        /// <param name="rotationAngle">The rotation angle for the cloud GameObject.</param>
        public FullScaleCloudSpawner(string mapName, string cdfFilePath, GameObject map, float rotationAngle) 
            : base(mapName, cdfFilePath, map, rotationAngle)
        {
        }

        
        /// <summary>
        /// Creates and sets up the cloud holder GameObject using ArcGis' map system.
        /// </summary>
        protected override void CreateCloudHolder()
        {
            CloudHolder = new GameObject(HolderName);
            CloudHolder.transform.SetParent(Map.transform, false);

            CloudHolder.AddComponent<HPTransform>();

            ArcGISLocationComponent location = CloudHolder.AddComponent<ArcGISLocationComponent>();
            location.runInEditMode = true;

            ArcGISPoint pos = new(SelectedDatasetScope.position.lon, SelectedDatasetScope.position.lat, Elevation,
                ArcGISSpatialReference.WGS84());
            
            location.Position = pos;

            location.Rotation = new ArcGISRotation(RotationAngle, 90, 0);
        }
    }
}
