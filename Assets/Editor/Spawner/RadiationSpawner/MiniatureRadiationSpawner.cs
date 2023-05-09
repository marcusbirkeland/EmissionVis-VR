using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEngine;

namespace Editor.Spawner.RadiationSpawner
{
    /// <summary>
    /// The MiniatureRadiationSpawner class is a derived class of BaseRadiationSpawner,
    /// designed for creating and managing miniature radiation visualizations in a 3D map
    /// using Microsoft Maps Unity SDK.
    /// </summary>
    public class MiniatureRadiationSpawner : BaseRadiationSpawner
    {
        /// <summary>
        /// Miniature map doesn't need size offset from position. Overrides the base class LatDistortionValue.
        /// </summary>
        protected override float LatDistortionValue => 1;

        /// <summary>
        /// Initializes a new instance of the MiniatureRadiationSpawner class.
        /// </summary>
        /// <param name="mapName">The name of the map to spawn the radiation visualization in.</param>
        /// <param name="cdfFilePath">The file path of the Cloud Data File (CDF).</param>
        /// <param name="map">The GameObject representing the map in the Unity scene.</param>
        /// <param name="rotationAngle">The rotation angle for the radiation visualization.</param>
        public MiniatureRadiationSpawner(string mapName, string cdfFilePath, GameObject map, float rotationAngle) 
            : base(mapName, cdfFilePath, map, rotationAngle)
        {
        }

        /// <summary>
        /// Creates the radiation holder GameObject for the miniature radiation visualization.
        /// </summary>
        protected override void CreateRadiationHolder()
        {
            RadiationHolder = new GameObject(HolderName);
            RadiationHolder.transform.SetParent(Map.transform, false);
            RadiationHolder.transform.localRotation = Quaternion.Euler(0, RotationAngle, 0);
            
            MapPin mapPin = RadiationHolder.AddComponent<MapPin>();

            mapPin.Location = SelectedDatasetScope.position;
            mapPin.IsLayerSynchronized = true;
            mapPin.UseRealWorldScale = true;
            mapPin.ShowOutsideMapBounds = true;
            mapPin.Altitude = 100;
            mapPin.AltitudeReference = AltitudeReference.Surface;
        }
    }
}
