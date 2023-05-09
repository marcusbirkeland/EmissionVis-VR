using System;
using Editor.NetCDF.Types;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.HPFramework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.Spawner.BuildingSpawner
{
    /// <summary>
    /// The FullScaleBuildingSpawner is a derived class of BaseBuildingSpawner used for spawning buildings in full-scale scenes.
    /// </summary>
    public class FullScaleBuildingSpawner : BaseBuildingSpawner
    {
        /// <summary>
        /// A value representing the size distortion based on latitude when using the mercator projection.
        /// </summary>
        private readonly float _latitudeOffset;
        
        
        /// <summary>
        /// Initializes a new instance of the FullScaleBuildingSpawner class.
        /// </summary>
        /// <param name="mapName">The name of the map.</param>
        /// <param name="cdfFilePath">The path to the building NetCDF file.</param>
        /// <param name="map">The map GameObject.</param>
        /// <param name="rotationAngle">The rotation angle for the buildings holder.</param>
        public FullScaleBuildingSpawner(string mapName, string cdfFilePath, GameObject map, float rotationAngle)
            : base(mapName, cdfFilePath, map, rotationAngle)
        {
            _latitudeOffset = (float) (1 / Math.Cos(Math.PI * SelectedDatasetScope.position.lat / 180.0));
        }

        /// <summary>
        /// Creates and sets up the building holder for the full-scale scene.
        /// </summary>
        protected override void CreateAndSetupBuildingHolder()
        {
            BuildingsHolder = new GameObject(HolderName);
            BuildingsHolder.transform.SetParent(Map.transform, false);

            BuildingsHolder.AddComponent<HPTransform>();

            ArcGISLocationComponent location = BuildingsHolder.AddComponent<ArcGISLocationComponent>();
            location.runInEditMode = true;
            location.Position = SelectedDatasetScope.position;

            location.Rotation = new ArcGISRotation(RotationAngle, 90, 0);
        }

        /// <summary>
        /// Spawns a building in the full-scale scene with the given building data.
        /// </summary>
        /// <param name="buildingData">The data for the building to be spawned.</param>
        protected override void SpawnBuilding(BuildingData buildingData)
        {
            GameObject building = Object.Instantiate(BuildingPrefab, BuildingsHolder.transform, false);

            building.name = $"Small Building {BuildingsHolder.transform.childCount}";

            building.transform.localScale = new Vector3(_latitudeOffset, _latitudeOffset, _latitudeOffset);
            
            building.transform.localPosition = new Vector3((float)buildingData.X * _latitudeOffset, (float)buildingData.Altitude, (float)buildingData.Y * _latitudeOffset);
        }
    }
}
