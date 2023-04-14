using System;
using System.Collections.Generic;
using System.IO;
using Editor.NetCDF;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.VisualizationSpawner
{
    public class BuildingSpawner : MapVisualizerSpawner
    {
        private readonly string _dataPath;
        private readonly GameObject _smallBuilding;

        private readonly List<BuildingDataLoader.BuildingData> _buildingDataList;
        
        private double _metersPerUnit;
        private Vector3 _worldSpacePin;
        
        
        public BuildingSpawner(string jsonDataPath, string attributesFilePath, string cdfFilePath, GameObject map, GameObject buildingModel, float rotationAngle) 
            : base(attributesFilePath, cdfFilePath, map, rotationAngle)
        {
            string newDataPath = jsonDataPath + "BuildingData/buildingData.csv";
            _buildingDataList = BuildingDataLoader.GetBuildingData(newDataPath);
            
            if (!File.Exists(newDataPath)) throw new ArgumentException("The file at " + newDataPath + " does not exist!");
            _dataPath = newDataPath;
            
            _smallBuilding = buildingModel;
        }


        private void CreateAndSetupBuildingHolder()
        {
            MapRenderer mapRenderer = Map.GetComponent<MapRenderer>();
            
            _metersPerUnit = mapRenderer.ComputeUnityToMapScaleRatio(SelectedCdfAttributes.position) / Map.transform.lossyScale.x;
            _worldSpacePin = mapRenderer.TransformLatLonAltToWorldPoint(SelectedCdfAttributes.position);
            
            VisualizerHolder = new GameObject("Buildings Holder");
            VisualizerHolder.transform.SetParent(Map.transform, false);
            VisualizerHolder.transform.localRotation = Quaternion.Euler(0, RotationAngle, 0);

            MapUISetup.SetBuildingHolder(VisualizerHolder);

            MapPin mapPin = VisualizerHolder.AddComponent<MapPin>();
            mapPin.Location = SelectedCdfAttributes.position;
            mapPin.UseRealWorldScale = true;
            mapPin.AltitudeReference = AltitudeReference.Ellipsoid;
        }

        
        public void SpawnAllBuildings()
        {
            DeletePreviousObject("Buildings Holder");
            CreateAndSetupBuildingHolder();
            
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

            Debug.Log($"Spawned {VisualizerHolder.transform.childCount} buildings.");
        }

        
        private void SpawnBuilding(BuildingDataLoader.BuildingData buildingData)
        {
            float distanceX = (float)(buildingData.X / _metersPerUnit);
            float distanceZ = (float)(buildingData.Y/ _metersPerUnit);
            string objectName = $"Small Building {VisualizerHolder.transform.childCount + 1}";

            Vector3 mapUp = Map.transform.up;
    
            Vector3 rotatedOffset = Quaternion.Euler(0, RotationAngle, 0) * new Vector3(distanceX, 0, distanceZ);

            Vector3 origin =
                _worldSpacePin +
                Map.transform.right * rotatedOffset.x +
                Map.transform.forward * rotatedOffset.z +
                mapUp * (10.0f * Map.transform.lossyScale.y);
            
            Ray ray = new(origin, mapUp * -1);
            
            Map.GetComponent<MapRenderer>().Raycast(ray, out MapRendererRaycastHit hitInfo);
            
            Vector3 pos = VisualizerHolder.transform.InverseTransformVector(hitInfo.Point - _worldSpacePin) * ((float)_metersPerUnit * Map.transform.lossyScale.x);
            GameObject building = Object.Instantiate(_smallBuilding, VisualizerHolder.transform, false);
            
            building.name = objectName;
            building.transform.localPosition += pos;
        }
    }
}