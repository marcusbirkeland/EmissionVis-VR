using Editor.NetCDF.Types;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEngine;

namespace Editor.Spawner.BuildingSpawner
{
    /// <summary>
    /// The MiniatureBuildingSpawner is a derived class of BaseBuildingSpawner used for spawning buildings in miniature scenes.
    /// </summary>
    public class MiniatureBuildingSpawner : BaseBuildingSpawner
    {
        private double _metersPerUnit;
        private Vector3 _worldSpacePin;
        
        /// <summary>
        /// Initializes a new instance of the MiniatureBuildingSpawner class.
        /// </summary>
        /// <param name="mapName">The name of the map.</param>
        /// <param name="cdfFilePath">The path to the building NetCDF file.</param>
        /// <param name="map">The map GameObject.</param>
        /// <param name="rotationAngle">The rotation angle for the buildings holder.</param>
        public MiniatureBuildingSpawner(string mapName, string cdfFilePath, GameObject map, float rotationAngle)
            : base(mapName, cdfFilePath, map, rotationAngle)
        {
        }
        
        /// <summary>
        /// Creates and sets up the building holder for the miniature scene.
        /// </summary>
        protected override void CreateAndSetupBuildingHolder()
        {
            Debug.Log("Creating building holder");
            
            MapRenderer mapRenderer = Map.GetComponent<MapRenderer>();
            
            _metersPerUnit = mapRenderer.ComputeUnityToMapScaleRatio(SelectedCdfAttributes.position) / Map.transform.lossyScale.x;
            _worldSpacePin = mapRenderer.TransformLatLonAltToWorldPoint(SelectedCdfAttributes.position);
            
            BuildingsHolder = new GameObject(HolderName);
            BuildingsHolder.transform.SetParent(Map.transform, false);
            BuildingsHolder.transform.localRotation = Quaternion.Euler(0, RotationAngle, 0);
            
            MapPin mapPin = BuildingsHolder.AddComponent<MapPin>();
            mapPin.Location = SelectedCdfAttributes.position;
            mapPin.UseRealWorldScale = true;
            mapPin.AltitudeReference = AltitudeReference.Ellipsoid;
        }

        /// <summary>
        /// Spawns a building in the miniature scene with the given building data.
        /// </summary>
        /// <param name="buildingData">The data for the building to be spawned.</param>
        protected override void SpawnBuilding(BuildingData buildingData)
        {
            float distanceX = (float)(buildingData.X / _metersPerUnit);
            float distanceZ = (float)(buildingData.Y/ _metersPerUnit);
            string objectName = $"Small Building {BuildingsHolder.transform.childCount + 1}";

            Vector3 mapUp = Map.transform.up;
    
            Vector3 rotatedOffset = Quaternion.Euler(0, RotationAngle, 0) * new Vector3(distanceX, 0, distanceZ);

            Vector3 origin =
                _worldSpacePin +
                Map.transform.right * rotatedOffset.x +
                Map.transform.forward * rotatedOffset.z +
                mapUp * (10.0f * Map.transform.lossyScale.y);
            
            Ray ray = new(origin, mapUp * -1);
            
            Map.GetComponent<MapRenderer>().Raycast(ray, out MapRendererRaycastHit hitInfo);
            
            Vector3 pos = BuildingsHolder.transform.InverseTransformVector(hitInfo.Point - _worldSpacePin) * ((float)_metersPerUnit * Map.transform.lossyScale.x);
            GameObject building = Object.Instantiate(BuildingPrefab, BuildingsHolder.transform, false);
            
            building.name = objectName;
            building.transform.localPosition += pos;
        }
    }
}
