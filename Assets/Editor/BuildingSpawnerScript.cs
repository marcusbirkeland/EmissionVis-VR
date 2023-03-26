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
        
        public string dataPath = "Assets/Resources/MapData/Trondheim/BuildingData/buildingData.csv";
        
        
        public GameObject map;
        public GameObject smallBuilding;
        public float xOffSet = -60.0f;
        public float zOffSet = 130f;

        public float rotationAngle = 0.0f;
        
        private double _metersPerUnit;
        private Vector3 _worldSpacePin;

        private GameObject _buildingsHolder;
        
        private CdfData _selectedCdfData;
        
    
    
        //Runs LoadMapData when window is opened.
        private void OnEnable()
        {
            LoadMapData();
        }


        [MenuItem("SINTEF/Building Generator")]
        public static void ShowWindow()
        {
            GetWindow<BuildingSpawner>("Building Generator!");
        }

    
        //GUI for the EditorWindow. Runs the SpawnBuildings method on click using the selected map and building model. 
        private void OnGUI()
        {
            map = EditorGUILayout.ObjectField("Map", map, typeof(GameObject), true) as GameObject;
            smallBuilding = EditorGUILayout.ObjectField("Building model", smallBuilding, typeof(GameObject), false) as GameObject;

            xOffSet = EditorGUILayout.FloatField("X Offset", xOffSet);
            zOffSet = EditorGUILayout.FloatField("Z Offset", zOffSet);
            
            rotationAngle = EditorGUILayout.FloatField("Rotation Angle", rotationAngle);
            
            bool canGenerateBuildings = smallBuilding != null && map != null;
            
            EditorGUI.BeginDisabledGroup(!canGenerateBuildings);
            if (GUILayout.Button("Generate Buildings"))
            {
                SpawnAllBuildings();
            }
            EditorGUI.EndDisabledGroup();
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


        //Main method for spawning the buildings
        private void SpawnAllBuildings()
        {
            // Check if the data file exists
            if (!File.Exists(dataPath)) throw new ArgumentException("The file at " + dataPath + " does not exist!");
            
            // Check if the MapRenderer component is missing
            MapRenderer mapRenderer = map.GetComponent<MapRenderer>();
            if (mapRenderer == null)
            {
                EditorUtility.DisplayDialog("Error", "The selected Map GameObject does not have a MapRenderer component. Please select a GameObject with the MapRenderer component.", "Ok");
                return;
            }
            
            
            _metersPerUnit = mapRenderer.ComputeUnityToMapScaleRatio(_selectedCdfData.position) / map.transform.lossyScale.x;
            _worldSpacePin = mapRenderer.TransformLatLonAltToWorldPoint(_selectedCdfData.position);
            
            //Deletes previous buildings
            for (int i = map.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = map.transform.GetChild(i);
                if (child.name == "Map Buildings")
                {
                    DestroyImmediate(child.gameObject);
                }
            }
            
            
            
            //Sets up the building holder
            _buildingsHolder = new GameObject("Map Buildings");
            _buildingsHolder.transform.SetParent(map.transform, false);
            _buildingsHolder.transform.localRotation = Quaternion.Euler(0, rotationAngle, 0);

            MapPin mapPin = _buildingsHolder.AddComponent<MapPin>();
            mapPin.Location = _selectedCdfData.position;
            mapPin.UseRealWorldScale = true;
            mapPin.AltitudeReference = AltitudeReference.Ellipsoid;

            
            using StreamReader streamReader = new(dataPath);

            long numLines = streamReader.BaseStream.Length;
            long currentLine = 0;

            while (streamReader.Peek() >= 0)
            {
                float[] data = AssertDataFormat(streamReader.ReadLine(), currentLine);

                SpawnBuilding(data);

                string progressStr = $"Parsing building data ({currentLine}/{numLines})";
                float progress = (float)currentLine / numLines * 10f;

                if (EditorUtility.DisplayCancelableProgressBar("Creating buildings from data", progressStr, progress))
                {
                    Debug.Log("Cancelled building spawning");
                    break;
                }

                currentLine++;
            }

            EditorUtility.ClearProgressBar();
            
            Debug.Log($"Spawned {_buildingsHolder.transform.childCount} buildings.");
        }

    
        private void SpawnBuilding(float[] data)
        {
            float distanceX = (float)((data[1] + xOffSet) / _metersPerUnit);
            float distanceZ = (float)((data[0] + zOffSet) / _metersPerUnit);
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

            Vector3 pos = map.transform.InverseTransformVector(hitInfo.Point - _worldSpacePin) * ((float)_metersPerUnit * map.transform.lossyScale.x);

            GameObject building = Instantiate(smallBuilding, _buildingsHolder.transform, false);
            building.name = objectName;

            building.transform.localPosition += pos;
        }
        

        // Converts a line of filedata to a float array containing an x and y value in meters. Throws exceptions if the data format is invalid.
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