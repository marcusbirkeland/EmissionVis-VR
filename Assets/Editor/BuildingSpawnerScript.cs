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
        private struct Position
        {
            public double lat;
            public double lon;

            public static implicit operator LatLon(Position position)
            {
                return new LatLon(position.lat, position.lon);
            }
            
            public static implicit operator LatLonAlt(Position position)
            {
                return new LatLonAlt(position.lat, position.lon, 0.0);
            }
        }

        [Serializable]
        private struct CdfData
        {
            public string filePath;
            public Position position;
        }

        [Serializable]
        private struct CdfDataListWrapper
        {
            public List<CdfData> data;
        }


        private const string EditorName = "Building Spawner";
        
        public string dataPath = "Assets/Resources/MapData/Trondheim/BuildingData/buildingData.csv";

        private double _metersPerUnit;
        private Vector3 _worldSpacePin;

        public GameObject map;
        public GameObject smallBuilding;
        
        private GameObject _buildingsHolder;
        
        private CdfData _selectedCdfData;
    
    
        //Runs LoadMapData when window is opened.
        private void OnEnable()
        {
            LoadMapData();
        }

    
        // Sets the _selectedCdfData variable to the first element in the JSON file.
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
                string jsonContent = File.ReadAllText(jsonFilePath);
                CdfDataListWrapper cdfDataList = JsonUtility.FromJson<CdfDataListWrapper>("{\"data\":" + jsonContent + "}");
                if (cdfDataList.data.Count > 0)
                {
                    _selectedCdfData = cdfDataList.data[0]; // Select the first MapData object by default
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

    
        //GUI for the EditorWindow. Runs the SpawnBuildings method on click using the selected map and building model. 
        private void OnGUI()
        {
            GUILayout.Label("Building generator!");

            map = EditorGUILayout.ObjectField("Map", map, typeof(GameObject), true) as GameObject;
            smallBuilding = EditorGUILayout.ObjectField("Building model", smallBuilding, typeof(GameObject), true) as GameObject;

            bool canGenerateBuildings = smallBuilding != null && map != null;
            
            
            EditorGUI.BeginDisabledGroup(!canGenerateBuildings);
            if (GUILayout.Button("Generate Buildings"))
            {
                SpawnBuildings();
            }
            EditorGUI.EndDisabledGroup();
        }


        //Main method for spawning the buildings
        private void SpawnBuildings()
        {
            // Check if the MapRenderer component is missing
            MapRenderer mapRenderer = map.GetComponent<MapRenderer>();
            if (mapRenderer == null)
            {
                EditorUtility.DisplayDialog("Error", "The selected Map GameObject does not have a MapRenderer component. Please select a GameObject with the MapRenderer component.", "Ok");
                return;
            }
            
            
            _metersPerUnit = mapRenderer.ComputeUnityToMapScaleRatio(_selectedCdfData.position) / map.transform.lossyScale.x;
            _worldSpacePin = mapRenderer.TransformLatLonAltToWorldPoint(_selectedCdfData.position);

            
            //Sets up the building holder
            _buildingsHolder = new GameObject("Map Buildings");
            _buildingsHolder.transform.SetParent(map.transform, false);
            MapPin mapPin = _buildingsHolder.AddComponent<MapPin>();
            mapPin.Location = _selectedCdfData.position;
            mapPin.UseRealWorldScale = true;
            mapPin.AltitudeReference = AltitudeReference.Ellipsoid;

            
            //Reads the csv file, and loads the building positions.
            StreamReader streamReader = new(dataPath);
            if (streamReader.Peek() < 0) throw new ArgumentException("The file at " + dataPath + " is empty!");
            
            
            long numLines = streamReader.BaseStream.Length;
            long currentLine = 0;

            while (streamReader.Peek() >= 0)
            {
                float[] data = AssertDataFormat(streamReader.ReadLine(), currentLine);

                SpawnBuilding(data);

                string progressStr = $"Parsing building data ({currentLine}/{numLines})";
                float progress = (float)currentLine / numLines * 10f;

                if (EditorUtility.DisplayCancelableProgressBar(EditorName, progressStr, progress))
                {
                    Debug.Log("Cancelled building spawning");
                    break;
                }
                
                currentLine++;

                //System.Threading.Thread.Sleep(10);
                
            }
            EditorUtility.ClearProgressBar();

            
            streamReader.Close();
            
            Debug.Log($"Spawned {_buildingsHolder.transform.childCount} buildings.");
        }

    
        private void SpawnBuilding(float[] data)
        {
            float x = data[1];
            float z = data[0];
            string objectName = $"Small Building {_buildingsHolder.transform.childCount + 1}";

            Vector3 origin = _worldSpacePin + map.transform.right * (x / (float)_metersPerUnit) + map.transform.forward * (z / (float)_metersPerUnit);
            Vector3 originOffset = origin + map.transform.up * (10.0f * map.transform.lossyScale.y);
            Ray ray = new(originOffset, map.transform.up * -1);

            map.GetComponent<MapRenderer>().Raycast(ray, out var hitInfo);

            Vector3 pos = map.transform.InverseTransformVector(hitInfo.Point - _worldSpacePin) * (float)_metersPerUnit * map.transform.lossyScale.x;

            GameObject building = Instantiate(smallBuilding, _buildingsHolder.transform, false);
            building.name = objectName;
            building.transform.localPosition += pos;
        }

        
        // Gets the 
        private static float[] AssertDataFormat(string data, long line)
        {
            string[] stringValues = data.Split(',');

            if (stringValues.Length != 2)
            {
                throw new ArgumentException(
                    $"Invalid data format at line: {line}. There should only be two columns of data values, but there are: {stringValues.Length}");
            }
            
            float[] floatArray = new float[2];

            for (int i = 0; i < stringValues.Length; i++)
            {
                if (!float.TryParse(stringValues[i], out floatArray[i]))
                {
                    throw new ArgumentException(
                        $"Invalid data format at line: {line}, and column: {i + 1}. Make sure the input data contains valid float values. Current value: {stringValues[i]}");
                }
            }
            
            return floatArray;
        }
    }
}