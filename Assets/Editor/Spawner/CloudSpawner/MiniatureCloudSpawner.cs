using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEngine;

namespace Editor.Spawner.CloudSpawner
{
    public class MiniatureCloudSpawner : BaseCloudSpawner
    {
        /// <summary>
        /// Miniature map doesnt need size offset from position.
        /// </summary>
        protected override float LatDistortionValue => 1;
        
        public MiniatureCloudSpawner(string mapName, string cdfFilePath, GameObject map, string cloudPrefabName, float rotationAngle, double elevation) 
            : base(mapName, cdfFilePath, map, cloudPrefabName, rotationAngle, elevation)
        {
        }


        protected override void CreateCloudHolder()
        {
            CloudHolder = new GameObject(HolderName);
            CloudHolder.transform.SetParent(Map.transform, false);
            CloudHolder.transform.localRotation = Quaternion.Euler(0, RotationAngle, 0);

            MapPin mapPin = CloudHolder.AddComponent<MapPin>();

            mapPin.Location = SelectedCdfAttributes.position;
            mapPin.IsLayerSynchronized = true;
            mapPin.UseRealWorldScale = true;
            mapPin.ShowOutsideMapBounds = true;
            mapPin.Altitude = Elevation;
            mapPin.AltitudeReference = AltitudeReference.Surface;
        }
    }
}