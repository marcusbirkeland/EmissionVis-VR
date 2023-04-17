﻿using System;
using System.Collections.Generic;
using Editor.NetCDF;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.VisualizationSpawner.FullScaleSpawners
{

    public class BuildingSpawner : MapVisualizerSpawner
    {
        private readonly GameObject _buildingPrefab;

        private readonly List<BuildingData> _buildingDataList;

        
        public BuildingSpawner(string mapName, string cdfFilePath, GameObject map, float rotationAngle) 
            : base(cdfFilePath, map, rotationAngle)
        {
            _buildingDataList = BuildingDataLoader.GetBuildingData(mapName);
            
            //Sets the building prefab.
            GameObject buildingPrefab = Resources.Load<GameObject>("Prefabs/House");
            if (buildingPrefab == null)
            {
                throw new Exception("Cannot find a building model at: Resources/Prefabs/House");
            }
            _buildingPrefab = buildingPrefab;
        }
        
        
        public void SpawnAndSetupBuildings()
        {
            DeletePreviousObject("Buildings Holder");
            CreateAndSetupBuildingHolder();
            SpawnAllBuildings();
        }

        
        private void CreateAndSetupBuildingHolder()
        {
            VisualizationHolder = new GameObject("Buildings Holder");
            VisualizationHolder.transform.SetParent(Map.transform, false);

            HPTransform hp = VisualizationHolder.AddComponent<HPTransform>();
            
            ArcGISLocationComponent location = VisualizationHolder.AddComponent<ArcGISLocationComponent>();
            location.runInEditMode = true;
            location.Position = SelectedCdfAttributes.position;

            location.Rotation = new ArcGISRotation(RotationAngle, 90, 0);
        }

        
        private void SpawnAllBuildings()
        {
            for (int i = 0; i < _buildingDataList.Count; i++)
            {
                string progressString = $"Parsing building data ({i}/{_buildingDataList.Count})";
                float progress = (float) i / _buildingDataList.Count;
                
                if (EditorUtility.DisplayCancelableProgressBar("Creating buildings from data", progressString, progress))
                {
                    Debug.Log("Cancelled building spawning");
                    break;
                }
                
                SpawnBuilding(_buildingDataList[i]);
            }
            
            EditorUtility.ClearProgressBar();

            Debug.Log($"Spawned {VisualizationHolder.transform.childCount} buildings.");
        }

        private void SpawnBuilding(BuildingData buildingData)
        {
            GameObject building = Object.Instantiate(_buildingPrefab, VisualizationHolder.transform, false);

            building.name = $"Small Building {VisualizationHolder.transform.childCount}";

            //TODO: replace with actual value.
            const float UnityUnitsPerMeter = 2.0f;
            
            building.transform.localPosition = new Vector3((float)buildingData.X * UnityUnitsPerMeter, 10000, (float)buildingData.Y * UnityUnitsPerMeter);
        }
    }
}