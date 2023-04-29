using Editor.NetCDF;
using Editor.NetCDF.Types;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.HPFramework;
using UnityEngine;
using Visualization;

namespace Editor.Spawner.BuildingSpawner
{

    public class FullScaleBuildingSpawner : BaseBuildingSpawner
    {
        public FullScaleBuildingSpawner(string mapName, string cdfFilePath, GameObject map, float rotationAngle)
            : base(mapName, cdfFilePath, map, rotationAngle)
        {
        }
        
        
        protected override void CreateAndSetupBuildingHolder()
        {
            VisualizationHolder = new GameObject(HolderName);
            VisualizationHolder.transform.SetParent(Map.transform, false);
            VisualizationHolder.AddComponent<BuildingsManager>();

            VisualizationHolder.AddComponent<HPTransform>();

            ArcGISLocationComponent location = VisualizationHolder.AddComponent<ArcGISLocationComponent>();
            location.runInEditMode = true;
            location.Position = SelectedCdfAttributes.position;

            location.Rotation = new ArcGISRotation(RotationAngle, 90, 0);
        }


        protected override void SpawnBuilding(BuildingData buildingData)
        {
            GameObject building = Object.Instantiate(BuildingPrefab, VisualizationHolder.transform, false);

            building.name = $"Small Building {VisualizationHolder.transform.childCount}";

            building.transform.localScale = new Vector3(UnityUnitsPerMeter, UnityUnitsPerMeter, UnityUnitsPerMeter);
            
            building.transform.localPosition = new Vector3((float)buildingData.X * UnityUnitsPerMeter, (float)buildingData.Altitude, (float)buildingData.Y * UnityUnitsPerMeter);
        }
    }
}