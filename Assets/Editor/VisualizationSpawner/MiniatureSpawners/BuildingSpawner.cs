using System;
using System.Collections.Generic;
using Editor.NetCDF;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.VisualizationSpawner.MiniatureSpawners
{
    public class BuildingSpawner : MapVisualizerSpawner
    {
        private readonly GameObject _buildingPrefab;

        private readonly List<BuildingData> _buildingDataList;
        
        private double _metersPerUnit;
        private Vector3 _worldSpacePin;
        
        
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
            MapRenderer mapRenderer = Map.GetComponent<MapRenderer>();
            
            _metersPerUnit = mapRenderer.ComputeUnityToMapScaleRatio(SelectedCdfAttributes.position) / Map.transform.lossyScale.x;
            _worldSpacePin = mapRenderer.TransformLatLonAltToWorldPoint(SelectedCdfAttributes.position);
            
            VisualizationHolder = new GameObject("Buildings Holder");
            VisualizationHolder.transform.SetParent(Map.transform, false);
            VisualizationHolder.transform.localRotation = Quaternion.Euler(0, RotationAngle, 0);
            
            MapPin mapPin = VisualizationHolder.AddComponent<MapPin>();
            mapPin.Location = SelectedCdfAttributes.position;
            mapPin.UseRealWorldScale = true;
            mapPin.AltitudeReference = AltitudeReference.Ellipsoid;
        }
        
        
        private void SpawnAllBuildings()
        {
            for (int i = 0; i < _buildingDataList.Count; i++)
            {
                string progressStr = $"Parsing building data ({i}/{_buildingDataList.Count})";
                float progress = (float) i / _buildingDataList.Count;
                
                if (EditorUtility.DisplayCancelableProgressBar("Creating buildings from data", progressStr, progress))
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
            float distanceX = (float)(buildingData.X / _metersPerUnit);
            float distanceZ = (float)(buildingData.Y/ _metersPerUnit);
            string objectName = $"Small Building {VisualizationHolder.transform.childCount + 1}";

            Vector3 mapUp = Map.transform.up;
    
            Vector3 rotatedOffset = Quaternion.Euler(0, RotationAngle, 0) * new Vector3(distanceX, 0, distanceZ);

            Vector3 origin =
                _worldSpacePin +
                Map.transform.right * rotatedOffset.x +
                Map.transform.forward * rotatedOffset.z +
                mapUp * (10.0f * Map.transform.lossyScale.y);
            
            Ray ray = new(origin, mapUp * -1);
            
            Map.GetComponent<MapRenderer>().Raycast(ray, out MapRendererRaycastHit hitInfo);
            
            Vector3 pos = VisualizationHolder.transform.InverseTransformVector(hitInfo.Point - _worldSpacePin) * ((float)_metersPerUnit * Map.transform.lossyScale.x);
            GameObject building = Object.Instantiate(_buildingPrefab, VisualizationHolder.transform, false);
            
            building.name = objectName;
            building.transform.localPosition += pos;
        }
    }
}