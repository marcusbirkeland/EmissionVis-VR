using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class BuildingSpawner : EditorWindow
    {
        [Serializable]
        public class Position
        {
            public double lat;
            public double lon;
        }

        [Serializable]
        public class MapData
        {
            public string filePath;
            public Position position;
        }

        [Serializable]
        public class MapDataList
        {
            public List<MapData> mapData;
        }

        private const string EditorName = "Building Spawner";
        private const int ProgressUpdateRate = 1;
        
        public string dataPath = "Assets/Resources/MapData/Trondheim/BuildingData/buildingData.csv";

        private double _latitude;
        private double _longitude;

        private double _metersPerUnit;
        private Vector3 _worldSpacePin;

        public GameObject map;
        public GameObject smallBuilding;
        
        private GameObject _buildingsHolder;
        
        private MapData _selectedMapData;
    
    
        private void OnEnable()
        {
            LoadMapData();
        }

    
        private void LoadMapData()
        {
            const string jsonFilePath = "Assets/Resources/MapData/attributes.json";
            if (!File.Exists(jsonFilePath))
            {
                Debug.LogError("JSON file not found.");
                return;
            }

            try
            {
                var jsonContent = File.ReadAllText(jsonFilePath);
                var mapDataList = JsonUtility.FromJson<MapDataList>("{\"mapData\":" + jsonContent + "}");
                if (mapDataList.mapData.Count > 0)
                {
                    _selectedMapData = mapDataList.mapData[0]; // Select the first MapData object by default
                }
                else
                {
                    Debug.LogError("Invalid JSON format.");
                }
            }
            catch (Exception e) when (e is ArgumentException or InvalidOperationException)
            {
                Debug.LogError($"Error parsing JSON content: {e.Message}");
            }
        }

    
        [MenuItem("SINTEF/Building Generator")]
        public static void ShowWindow()
        {
            GetWindow(typeof(BuildingSpawner));
        }

    
        private void OnGUI()
        {
            GUILayout.Label("Building generator!");

            map = EditorGUILayout.ObjectField("Map", map, typeof(GameObject), true) as GameObject;
            smallBuilding = EditorGUILayout.ObjectField("Building model", smallBuilding, typeof(GameObject), true) as GameObject;

            if (GUILayout.Button("Generate Buildings"))
            {
                if (map == null)
                {
                    EditorUtility.DisplayDialog("Error", "Map GameObject is not assigned. Please assign a Map GameObject.", "Ok");
                }
                else if (smallBuilding == null)
                {
                    EditorUtility.DisplayDialog("Error", "Building model is not assigned. Please assign a Building GameObject.", "Ok");
                }
                else if (_selectedMapData == null)
                {
                    EditorUtility.DisplayDialog("Error", "No valid MapData loaded. Please check the JSON file.", "Ok");
                }
                else
                {
                    _latitude = _selectedMapData.position.lat;
                    _longitude = _selectedMapData.position.lon;

                    SpawnBuildings();
                }
            }
        }

    
        private void SpawnBuildings()
        {
            Debug.Log("Spawning buildings...");
        
            // Check if the MapRenderer component is missing
            MapRenderer mapRenderer = map.GetComponent<MapRenderer>();
            if (mapRenderer == null)
            {
                EditorUtility.DisplayDialog("Error", "The selected Map GameObject does not have a MapRenderer component. Please select a GameObject with the MapRenderer component.", "Ok");
                return;
            }

            _buildingsHolder = new GameObject("Map Buildings");
            _buildingsHolder.transform.SetParent(map.transform, false);

            StreamReader sr = new StreamReader(dataPath);

            if (sr.Peek() < 0)
            {
                throw new ArgumentException("The file at " + dataPath + " is empty!");
            }

            float progressBar = 0.0f;
            EditorUtility.DisplayCancelableProgressBar(
                EditorName,
                "Parsing building data...",
                progressBar
            );

            long numLines = sr.BaseStream.Length;
            long currentLine = 0;
            long counter = 0;

            _metersPerUnit = mapRenderer.ComputeUnityToMapScaleRatio(new LatLon(_latitude, _longitude)) / map.transform.lossyScale.x;
            _worldSpacePin = mapRenderer.TransformLatLonAltToWorldPoint(new LatLonAlt(_latitude, _longitude, 0.0));

            while (sr.Peek() >= 0)
            {
                string line = sr.ReadLine();
            
                string[] data = line.Split(',');

                ParseBuilding(data, currentLine);

                if (counter >= ProgressUpdateRate)
                {
                    string progressStr = $"Parsing building data ({currentLine}/{numLines})";
                    float progress = (float)currentLine / numLines * 10f;
                    EditorUtility.DisplayCancelableProgressBar(EditorName, progressStr, progress);
                    counter = 0;
                }
                currentLine++;
                counter++;
            }

            Debug.Log("Finished reading file. Closing...");
            sr.Close();
            EditorUtility.ClearProgressBar();
            Debug.Log("Stream successfully closed.");
            Debug.Log($"Spawned {_buildingsHolder.transform.childCount} buildings.");

            MapPin mapPin = _buildingsHolder.AddComponent<MapPin>();
            mapPin.Location = new LatLon(_latitude, _longitude);
            mapPin.UseRealWorldScale = true;
            mapPin.enabled = true;

            mapPin.AltitudeReference = AltitudeReference.Ellipsoid;
        }

    
        private void ParseBuilding(string[] data, long currentLine)
        {
            if (data.Length < 2)
            {
                Debug.LogError($"Insufficient data at line {currentLine}. Make sure the input data is formatted correctly. Current data: {string.Join(", ", data)}");
                return;
            }

            try
            {
                SpawnBuilding(
                    float.Parse(data[1], CultureInfo.InvariantCulture.NumberFormat),
                    float.Parse(data[0], CultureInfo.InvariantCulture.NumberFormat),
                    $"Small Building {_buildingsHolder.transform.childCount + 1}"
                );
            }
            catch (FormatException e)
            {
                Debug.LogError($"Invalid data format at line {currentLine}: {e.Message}. Make sure the input data contains valid float values. Current data: {string.Join(", ", data)}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Unexpected error in ParseBuilding at line {currentLine}: {e.Message}. Current data: {string.Join(", ", data)}");
            }
        }


        private void SpawnBuilding(float x, float z, string objectName)
        {
            Vector3 origin = _worldSpacePin + map.transform.right * (x / (float)_metersPerUnit) + map.transform.forward * (z / (float)_metersPerUnit);
            Vector3 originOffset = origin + map.transform.up * (10.0f * map.transform.lossyScale.y);
            Ray ray = new Ray(originOffset, map.transform.up * -1);

            map.GetComponent<MapRenderer>().Raycast(ray, out var hitInfo);

            var pos = map.transform.InverseTransformVector(hitInfo.Point - _worldSpacePin) * (float)_metersPerUnit * map.transform.lossyScale.x;

            GameObject building = Instantiate(smallBuilding, _buildingsHolder.transform, false);
            building.name = objectName;
            building.transform.localPosition += pos;
        }
    }
}