using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEngine;

namespace Editor.Spawner.RadiationSpawner
{
    public class MiniatureRadiationSpawner : BaseRadiationSpawner
    {
        /// <summary>
        /// Miniature map doesnt need size offset from position.
        /// </summary>
        protected override float LatDistortionValue => 1;

        public MiniatureRadiationSpawner(string mapName, string cdfFilePath, GameObject map, string radiationPrefabName, float rotationAngle) 
            : base(mapName, cdfFilePath, map, radiationPrefabName, rotationAngle)
        {
        }

        
        protected override void CreateRadiationHolder()
        {
            RadiationHolder = new GameObject(HolderName);
            RadiationHolder.transform.SetParent(Map.transform, false);
            RadiationHolder.transform.localRotation = Quaternion.Euler(0, RotationAngle, 0);
            
            MapPin mapPin = RadiationHolder.AddComponent<MapPin>();

            mapPin.Location = SelectedCdfAttributes.position;
            mapPin.IsLayerSynchronized = true;
            mapPin.UseRealWorldScale = true;
            mapPin.ShowOutsideMapBounds = true;
            mapPin.Altitude = 100;
            mapPin.AltitudeReference = AltitudeReference.Surface;
        }
    }
}