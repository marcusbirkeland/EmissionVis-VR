using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using UnityEngine;

namespace Editor.Spawner.RadiationSpawner
{
    public class FullScaleRadiationSpawner : BaseRadiationSpawner
    {
        public FullScaleRadiationSpawner(string mapName, string cdfFilePath, GameObject map, string radiationPrefabName, float rotationAngle) 
            : base(mapName, cdfFilePath, map, radiationPrefabName, rotationAngle)
        {
        }

        
        protected override void CreateRadiationHolder()
        {
            RadiationHolder = new GameObject(HolderName);
            RadiationHolder.transform.SetParent(Map.transform, false);

            RadiationHolder.AddComponent<HPTransform>();

            ArcGISLocationComponent location = RadiationHolder.AddComponent<ArcGISLocationComponent>();
            location.runInEditMode = true;
            
            location.Position = new ArcGISPoint(SelectedCdfAttributes.position.lon, SelectedCdfAttributes.position.lat, 350.0f,
                ArcGISSpatialReference.WGS84());
            
            location.Rotation = new ArcGISRotation(RotationAngle, 90, 0);
        }
    }
}