using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Editor.BuildingSpawning;
using UnityEditor;
using UnityEngine;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using Object = UnityEngine.Object;

namespace Editor
{
    public class BuildingSpawner
    {
        private readonly string _dataPath;
        private GameObject _map;
        private GameObject _smallBuilding;
        private float _rotationAngle;

        private readonly MapDataLoader.CdfData _selectedCdfData;
        private readonly List<BuildingData> _buildingDataList;
        
        private double _metersPerUnit;
        private Vector3 _worldSpacePin;
        private GameObject _buildingsHolder;
        
        
        //GUI for the EditorWindow. Runs the SpawnBuildings method on click using the selected map and building model. 
        private void Draw()
        {
            _map = EditorGUILayout.ObjectField("Map", _map, typeof(GameObject), true) as GameObject;
            _smallBuilding = EditorGUILayout.ObjectField("Building model", _smallBuilding, typeof(GameObject), false) as GameObject;
            _rotationAngle = EditorGUILayout.FloatField("Rotation Angle", _rotationAngle);
            
            bool canGenerateBuildings = _smallBuilding != null && _map != null;
            
            EditorGUI.BeginDisabledGroup(!canGenerateBuildings);
            if (GUILayout.Button("Generate Buildings"))
            {
                SpawnAllBuildings();
            }
            EditorGUI.EndDisabledGroup();
        }
        
        
        /**
         * dataPath = "Assets/Resources/MapData/{mapName}/"
         * attributesFilePath = "Assets/Resources/MapData/attributes.json"
         * cdfFilePath  = Full path to cdfFile containing building data
         */
        public BuildingSpawner(string jsonDataPath, string attributesFilePath, string cdfFilePath, GameObject map, GameObject buildingModel, float rotationAngle)
        {
            _selectedCdfData = MapDataLoader.LoadMapData(attributesFilePath, cdfFilePath);

            string newDataPath = jsonDataPath + "BuildingData/buildingData.csv";
            _buildingDataList = BuildingSpawnerController.LoadBuildingData(newDataPath);
            _dataPath = newDataPath;
            
            _map = map;
            _smallBuilding = buildingModel;
            _rotationAngle = rotationAngle;
        }


        public void SpawnAllBuildings()
        {
            CheckDataFileExists();

            MapRenderer mapRenderer = GetMapRenderer();
            if (mapRenderer == null) return;
            
            // Wait for the map to load or the user to cancel
            while (!mapRenderer.IsLoaded)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Waiting for map to load", "Loading map, please wait...", -1))
                {
                    Debug.Log("Cancelled building spawning");
                    EditorUtility.ClearProgressBar();
                    return;
                }
            }
            
            EditorUtility.ClearProgressBar();

            DeletePreviousBuildings();
            SetupBuildingHolder(mapRenderer);
            
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

            Debug.Log($"Spawned {_buildingsHolder.transform.childCount} buildings.");
        }
        
        
        private void CheckDataFileExists()
        {
            if (!File.Exists(_dataPath)) throw new ArgumentException("The file at " + _dataPath + " does not exist!");
        }

        //TODO: Fix error display. It currently doesnt stop the function call.
        private MapRenderer GetMapRenderer()
        {
            MapRenderer mapRenderer = _map.GetComponent<MapRenderer>();
            if (mapRenderer == null)
            {
                EditorUtility.DisplayDialog("Error", "The selected Map GameObject does not have a MapRenderer component. Please select a GameObject with the MapRenderer component.", "Ok");
            }
            return mapRenderer;
        }

        
        private void DeletePreviousBuildings()
        {
            for (int i = _map.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = _map.transform.GetChild(i);
                if (child.name == "Map Buildings")
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }
        
        
        private void SetupBuildingHolder(MapRenderer mapRenderer)
        {
            _metersPerUnit = mapRenderer.ComputeUnityToMapScaleRatio(_selectedCdfData.position) / _map.transform.lossyScale.x;
            _worldSpacePin = mapRenderer.TransformLatLonAltToWorldPoint(_selectedCdfData.position);
            
            _buildingsHolder = new GameObject("Map Buildings");
            _buildingsHolder.transform.SetParent(_map.transform, false);
            _buildingsHolder.transform.localRotation = Quaternion.Euler(0, _rotationAngle, 0);

            MapPin mapPin = _buildingsHolder.AddComponent<MapPin>();
            mapPin.Location = _selectedCdfData.position;
            mapPin.UseRealWorldScale = true;
            mapPin.AltitudeReference = AltitudeReference.Ellipsoid;
        }
        
        
        private void SpawnBuilding(BuildingData buildingData)
        {
            float distanceX = (float)(buildingData.x / _metersPerUnit);
            float distanceZ = (float)(buildingData.y/ _metersPerUnit);
            string objectName = $"Small Building {_buildingsHolder.transform.childCount + 1}";

            Vector3 mapUp = _map.transform.up;
    
            Vector3 rotatedOffset = Quaternion.Euler(0, _rotationAngle, 0) * new Vector3(distanceX, 0, distanceZ);

            Vector3 origin =
                _worldSpacePin +
                _map.transform.right * rotatedOffset.x +
                _map.transform.forward * rotatedOffset.z +
                mapUp * (10.0f * _map.transform.lossyScale.y);
            
            Ray ray = new(origin, mapUp * -1);
            
            _map.GetComponent<MapRenderer>().Raycast(ray, out MapRendererRaycastHit hitInfo);
            
            Vector3 pos = _buildingsHolder.transform.InverseTransformVector(hitInfo.Point - _worldSpacePin) * ((float)_metersPerUnit * _map.transform.lossyScale.x);
            GameObject building = Object.Instantiate(_smallBuilding, _buildingsHolder.transform, false);
            
            building.name = objectName;
            building.transform.localPosition += pos;
        }
    }
}