using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEngine;

namespace Editor.Spawner.CloudSpawner
{
    /// <summary>
    /// The MiniatureCloudSpawner class is responsible for spawning and managing
    /// miniature clouds in a Unity scene using geospatial data.
    /// </summary>
    public class MiniatureCloudSpawner : BaseCloudSpawner
    {
        /// <summary>
        /// Sets the name of the prefab to be used for the clouds.
        /// </summary>
        protected override string PrefabName => "Cloud Miniature";

        /// <summary>
        /// Sets the altitude of the clouds. The demo dataset is placed on a hill, so the value is negative.
        /// </summary>
        protected override double Elevation => -130;

        /// <summary>
        /// Miniature map doesnt need size offset from position.
        /// </summary>
        protected override float LatDistortionValue => 1;

        
        /// <summary>
        /// Initializes a new instance of the MiniatureCloudSpawner class.
        /// </summary>
        /// <param name="mapName">The name of the map to spawn the clouds in.</param>
        /// <param name="cdfFilePath">The file path of the Cloud Data File (CDF).</param>
        /// <param name="map">The GameObject representing the map in the Unity scene.</param>
        /// <param name="rotationAngle">The rotation angle for the clouds.</param>
        public MiniatureCloudSpawner(string mapName, string cdfFilePath, GameObject map, float rotationAngle) 
            : base(mapName, cdfFilePath, map, rotationAngle)
        {
        }

        
        /// <summary>
        /// Creates the CloudHolder GameObject, which serves as a container for the spawned clouds.
        /// Then sets it up using the Bing maps system.
        /// </summary>
        protected override void CreateCloudHolder()
        {
            CloudHolder = new GameObject(HolderName);
            CloudHolder.transform.SetParent(Map.transform, false);
            CloudHolder.transform.localRotation = Quaternion.Euler(0, RotationAngle, 0);

            MapPin mapPin = CloudHolder.AddComponent<MapPin>();

            mapPin.Location = SelectedDatasetScope.position;
            mapPin.IsLayerSynchronized = true;
            mapPin.UseRealWorldScale = true;
            mapPin.ShowOutsideMapBounds = true;
            mapPin.Altitude = Elevation;
            mapPin.AltitudeReference = AltitudeReference.Surface;
        }
    }
}
