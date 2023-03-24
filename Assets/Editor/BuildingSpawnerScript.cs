using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEditor;
using Microsoft.Maps.Unity;
using Microsoft.Geospatial;
using System;
using System.IO;

public class BuildingSpawner : EditorWindow
{
    private const string EditorName = "Building Spawner";
    private const int ProgressUpdateRate = 1;

    public string dataPath = "Assets/Resources/Bergen/Building/buildings.csv";
    public string elevationDataPath = "Assets/Resources/Bergen/Elevation/elevation.csv";
    public float altitudeOffset = 0;

    public double latitude = 60.35954907032411;
    public double longitude = 5.314180944287559;

    private double metersPerUnit;
    private Vector3 worldSpacePin;

    public int dataYIndex = 0;
    public int dataXIndex = 1;
    public int dataHeightIndex = 2;

    public GameObject map;
    public GameObject smallBuilding;
    private GameObject buildings;

    public bool raycasting = true;

    private string[] elevationData;

    [MenuItem("SINTEF/Building Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(BuildingSpawner));
    }

    private void OnGUI()
    {
        GUILayout.Label("Building generator!");
        dataPath = EditorGUILayout.TextField("Path to building data: ", dataPath);

        dataYIndex = EditorGUILayout.IntField("y-index in data", dataYIndex);
        dataXIndex = EditorGUILayout.IntField("x-index in data", dataXIndex);
        dataHeightIndex = EditorGUILayout.IntField("height-index in data", dataHeightIndex);

        altitudeOffset = EditorGUILayout.FloatField("Altitude offset", altitudeOffset);

        latitude = EditorGUILayout.DoubleField("Latitude", latitude);
        longitude = EditorGUILayout.DoubleField("Longitude", longitude);

        map = EditorGUILayout.ObjectField("Map", map, typeof(GameObject), true) as GameObject;
        smallBuilding = EditorGUILayout.ObjectField("Building model", smallBuilding, typeof(GameObject), true) as GameObject;

        raycasting = EditorGUILayout.Toggle("Use raycasting", raycasting);

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
            else if (string.IsNullOrEmpty(dataPath) || !File.Exists(dataPath))
            {
                EditorUtility.DisplayDialog("Error", "Invalid path to building data. Please provide a valid path.", "Ok");
            }
            else
            {
                SpawnBuildings();
            }
        }


        GUIStyle textStyle = GUI.skin.GetStyle("PR TextField");
        textStyle.wordWrap = true;
        EditorGUILayout.TextArea(@"NOTE:
        1) At lower zoom levels (zoomed out) the altitude of the ray cast hit becomes inaccurate (zoomLevel < ~12). Try to zoom in as far as possible, BUT
        2) at very high zoom levels (zoomed in) the ray casts move outside of the map collider and won't intersect with it. Make sure the map is zoomed out sufficiently enough to include the entire area that will be covered with buildings. ",
        textStyle);
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

        buildings = new GameObject("Map Buildings");
        buildings.transform.SetParent(map.transform, false);

        StreamReader sr = new StreamReader(dataPath);

        if (sr.Peek() < 0)
        {
            throw new ArgumentException("The file at " + dataPath + " is empty!");
        }

        sr.ReadLine();
        elevationData = File.ReadAllLines(elevationDataPath);

        float progressBar = 0.0f;
        EditorUtility.DisplayCancelableProgressBar(
            EditorName,
            "Parsing building data...",
            progressBar
        );

        long numLines = sr.BaseStream.Length;
        long currentLine = 1;
        long counter = 0;

        metersPerUnit = MapScaleRatioExtensions.ComputeUnityToMapScaleRatio(mapRenderer, new LatLon(latitude, longitude)) / map.transform.lossyScale.x;
        worldSpacePin = MapRendererTransformExtensions.TransformLatLonAltToWorldPoint(mapRenderer, new LatLonAlt(latitude, longitude, 0.0));

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
        Debug.Log($"Spawned {buildings.transform.childCount} buildings.");

        MapPin mapPin = buildings.AddComponent<MapPin>();
        mapPin.Location = new LatLon(latitude, longitude);
        mapPin.UseRealWorldScale = true;
        if (!raycasting) mapPin.Altitude = altitudeOffset;
        mapPin.enabled = true;

        mapPin.AltitudeReference = AltitudeReference.Ellipsoid;
    }

    private void ParseBuilding(string[] data, long currentLine)
    {
        string y = data[dataYIndex];
        string x = data[dataXIndex];
        string height = data[dataHeightIndex];

        if (!string.IsNullOrEmpty(height))
        {
            SpawnBuilding(
                float.Parse(x, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(y, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(height, CultureInfo.InvariantCulture.NumberFormat),
                $"Small Building {buildings.transform.childCount + 1}",
                currentLine
            );
        }
    }

    private void SpawnBuilding(float x, float z, float height, string name, long buildingIndex)
    {
        string[] elevationString = elevationData[buildingIndex].Split(',');

        Vector3 pos;
        if (!raycasting)
        {
            float y = float.Parse(elevationString[dataHeightIndex], CultureInfo.InvariantCulture.NumberFormat);
            pos = new Vector3(x, y, z);
        }
        else
        {
            Vector3 origin = worldSpacePin + map.transform.right * (x / (float)metersPerUnit) + map.transform.forward * (z / (float)metersPerUnit);
            Vector3 originOffset = origin + map.transform.up * (10.0f * map.transform.lossyScale.y);
            Ray ray = new Ray(originOffset, map.transform.up * -1);

            MapRendererRaycastHit hitInfo;
            map.GetComponent<MapRenderer>().Raycast(ray, out hitInfo);

            pos = map.transform.InverseTransformVector((hitInfo.Point - worldSpacePin)) * (float)metersPerUnit * map.transform.lossyScale.x;
        }

        GameObject building = Instantiate(smallBuilding, buildings.transform, false);
        building.name = name;
        building.transform.localPosition += pos;
    }
}

       
