using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using UnityEngine;

namespace Editor.Spawner.CloudSpawner
{
    public class FullScaleCloudSpawner : BaseCloudSpawner
    {
        public FullScaleCloudSpawner(string mapName, string cdfFilePath, GameObject map, string cloudPrefabName, float rotationAngle, double elevation) 
            : base(mapName, cdfFilePath, map, cloudPrefabName, rotationAngle, elevation)
        {
        }


        protected override void CreateCloudHolder()
        {
            CloudHolder = new GameObject(HolderName);
            CloudHolder.transform.SetParent(Map.transform, false);

            CloudHolder.AddComponent<HPTransform>();

            ArcGISLocationComponent location = CloudHolder.AddComponent<ArcGISLocationComponent>();
            location.runInEditMode = true;

            ArcGISPoint pos = new(SelectedCdfAttributes.position.lon, SelectedCdfAttributes.position.lat, Elevation,
                ArcGISSpatialReference.WGS84());
            
            location.Position = pos;

            location.Rotation = new ArcGISRotation(RotationAngle, 90, 0);
        }
    }
}