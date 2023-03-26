using System;
using System.Collections.Generic;
using System.IO;
using Editor.BuildingSpawning;
using UnityEditor;
using UnityEngine;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;

namespace Editor
{
    public class BuildingSpawner : EditorWindow
    {
        public string dataPath = "Assets/Resources/MapData/Trondheim/BuildingData/buildingData.csv";
        public string jsonFilePath = "Assets/Resources/MapData/attributes.json";
        public string cdfFilePath = "";
        public GameObject map;
        public GameObject smallBuilding;
        public float rotationAngle;

        private double _metersPerUnit;
        private Vector3 _worldSpacePin;
        private GameObject _buildingsHolder;
        private MapDataLoader.CdfData _selectedCdfData;
        private List<BuildingData> _buildingDataList;

        
        private void OnEnable()
        {
            _selectedCdfData = MapDataLoader.LoadMapData(jsonFilePath, cdfFilePath);
            _buildingDataList = BuildingSpawnerController.LoadBuildingData(dataPath);
        }

        
        [MenuItem("SINTEF/Building Generator232")]
        public static void ShowWindow()
        {
            GetWindow<BuildingSpawner>("Building Generator!");
        }

        
        //GUI for the EditorWindow. Runs the SpawnBuildings method on click using the selected map and building model. 
        private void OnGUI()
        {
            map = EditorGUILayout.ObjectField("Map", map, typeof(GameObject), true) as GameObject;
            smallBuilding = EditorGUILayout.ObjectField("Building model", smallBuilding, typeof(GameObject), false) as GameObject;
            
            rotationAngle = EditorGUILayout.FloatField("Rotation Angle", rotationAngle);
            
            bool canGenerateBuildings = smallBuilding != null && map != null;
            
            EditorGUI.BeginDisabledGroup(!canGenerateBuildings);
            if (GUILayout.Button("Generate Buildings"))
            {
                SpawnAllBuildings();
            }
            EditorGUI.EndDisabledGroup();
        }

        
        private void SpawnAllBuildings()
        {
            CheckDataFileExists();

            MapRenderer mapRenderer = GetMapRenderer();
            if (mapRenderer == null) return;

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
            if (!File.Exists(dataPath)) throw new ArgumentException("The file at " + dataPath + " does not exist!");
        }

        
        private MapRenderer GetMapRenderer()
        {
            MapRenderer mapRenderer = map.GetComponent<MapRenderer>();
            if (mapRenderer == null)
            {
                EditorUtility.DisplayDialog("Error", "The selected Map GameObject does not have a MapRenderer component. Please select a GameObject with the MapRenderer component.", "Ok");
            }
            return mapRenderer;
        }

        
        private void DeletePreviousBuildings()
        {
            for (int i = map.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = map.transform.GetChild(i);
                if (child.name == "Map Buildings")
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        
        private void SetupBuildingHolder(MapRenderer mapRenderer)
        {
            _metersPerUnit = mapRenderer.ComputeUnityToMapScaleRatio(_selectedCdfData.position) / map.transform.lossyScale.x;
            _worldSpacePin = mapRenderer.TransformLatLonAltToWorldPoint(_selectedCdfData.position);
            
            _buildingsHolder = new GameObject("Map Buildings");
            _buildingsHolder.transform.SetParent(map.transform, false);
            _buildingsHolder.transform.localRotation = Quaternion.Euler(0, rotationAngle, 0);

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

            Vector3 mapUp = map.transform.up;
    
            Vector3 rotatedOffset = Quaternion.Euler(0, rotationAngle, 0) * new Vector3(distanceX, 0, distanceZ);

            Vector3 origin =
                _worldSpacePin +
                map.transform.right * rotatedOffset.x +
                map.transform.forward * rotatedOffset.z +
                mapUp * (10.0f * map.transform.lossyScale.y);

            Ray ray = new(origin, mapUp * -1);

            map.GetComponent<MapRenderer>().Raycast(ray, out MapRendererRaycastHit hitInfo);

            Vector3 pos = _buildingsHolder.transform.InverseTransformVector(hitInfo.Point - _worldSpacePin) * ((float)_metersPerUnit * map.transform.lossyScale.x);

            GameObject building = Instantiate(smallBuilding, _buildingsHolder.transform, false);
            building.name = objectName;

            building.transform.localPosition += pos;
        }
    }
}