using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEditor;
using Microsoft.Maps.Unity;
using Microsoft.Geospatial;
using System;

public class BuildingSpawner : EditorWindow
{
    private const string EDITOR_NAME = "Building Spawner";
    private const int PROGRESS_UPDATE_RATE = 1;

    public string dataPath ="Assets/Resources/Bergen/Building/buildings.csv";
    public string elevationDataPath="Assets/Resources/Bergen/Elevation/elevation.csv";  
    //public Vector3 positionOffset;

    public float altitudeOffset = 0;

    public double latitude = 60.35954907032411;
    public double longitude = 5.314180944287559;

    // How many meters on the map go into one coordinate unit in Unity world Space. 
    private double metersPerUnit;
    private Vector3 worldSpacePin;

    public int data_y_index = 0;
    public int data_x_index = 1;
    public int data_height_index = 2;

    public GameObject map;
    public GameObject smallBuilding;
    //public GameObject largeBuilding;
    private GameObject buildings;

    public bool raycasting = true;

    private string[] elevationData;

    [MenuItem("SINTEF / Building Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(BuildingSpawner)); // Inherited from EditorWindow
    }

    private void OnGUI()
    {
        GUILayout.Label("Building generator!");
        //textureFolderPath = EditorGUILayout.TextField("Path to textures folder", textureFolderPath);
        dataPath = EditorGUILayout.TextField("Path to building data: ", dataPath);

        data_y_index = EditorGUILayout.IntField("y-index in data", data_y_index);
        data_x_index = EditorGUILayout.IntField("x-index in data", data_x_index);
        data_height_index = EditorGUILayout.IntField("height-index in data", data_height_index);

        altitudeOffset = EditorGUILayout.FloatField("Altitude offset", altitudeOffset);

        latitude = EditorGUILayout.DoubleField("Latitude", latitude);
        longitude = EditorGUILayout.DoubleField("Longitude", longitude);

        map = EditorGUILayout.ObjectField("Map", map, typeof(GameObject)) as GameObject;
        smallBuilding = EditorGUILayout.ObjectField("Building model", smallBuilding, typeof(GameObject)) as GameObject;

        raycasting = EditorGUILayout.Toggle("Use raycasting", raycasting);

        if (GUILayout.Button("Generate Buildings"))
        {
            SpawnBuildings();
        }

        GUIStyle textStyle = GUI.skin.GetStyle("PR TextField");
        textStyle.wordWrap = true;
        EditorGUILayout.TextArea(@"NOTE:
        1) At lower zoom levels (zoomed out) the altitude of the ray cast hit becomes inaccurate (zoomLevel < ~12). Try to zoom in as far as possible, BUT
        2) at very high zoom levels (zoomed in) the ray casts move outside of the map collider and won't intersect with it. Make sure the map is zoomed out sufficiently enough to include the entire area that will be covered with buildings. ",
        textStyle);
    }



    public void SpawnBuildings(){
        Debug.Log("Spawning buildings...");

        // Instantiate new GameObject as child of map.
        buildings = new GameObject("Map Buildings");

        // SetParent() is the same as the .parent property except that it also lets the Transform keep its local orientation rather than its global orientation. 
        buildings.transform.SetParent(map.transform, false);

        System.IO.StreamReader sr = new System.IO.StreamReader(dataPath);
        if(sr.Peek() < 0){
            throw new System.ArgumentException("The file at " + dataPath + " is empty!");
        }

        //TODO: Expand to automatically set data indecies based on header.

        // Read first line
        sr.ReadLine();

        elevationData = System.IO.File.ReadAllLines(elevationDataPath);

        // Draw a progressbar to show that work is being done
        float progressBar = 0.0f;
        EditorUtility.DisplayCancelableProgressBar(
            "Building Spawner",
            "Parsing building data...",
            progressBar
        );
        long numLines = sr.BaseStream.Length;
        long currentLine = 1;
        long counter = 0; // Counter variable to prevent writing to progress bar too often.

        // Needed for raycasting:
        metersPerUnit = MapScaleRatioExtensions.ComputeUnityToMapScaleRatio(map.GetComponent<MapRenderer>(), new LatLon(latitude, longitude)) / map.transform.lossyScale.x;
        worldSpacePin = MapRendererTransformExtensions.TransformLatLonAltToWorldPoint(map.GetComponent<MapRenderer>(), new LatLonAlt(latitude, longitude, 0.0));

        // Read file line by line
        while (sr.Peek() >= 0){
            string line = sr.ReadLine();
            //Debug.Log("line: " + line);

            string [] data = line.Split(',');
            ParseBuilding(data, currentLine);

            // Update progress-bar
            if(counter >= PROGRESS_UPDATE_RATE){
                string progressStr = "Parsing building data (" + currentLine + "/" + numLines +")";
                float progress = ((float) currentLine/ (float)numLines)*10f;
                //Debug.Log("progress: " + progress);
                EditorUtility.DisplayCancelableProgressBar(EDITOR_NAME, progressStr , progress);
                counter = 0;
            }
            currentLine++;
            counter++;
        }



        Debug.Log("Finished reading file. Closing..");
        sr.Close();
        EditorUtility.ClearProgressBar();
        Debug.Log("Stream successfully closed.");
        Debug.Log("Spawned " + buildings.transform.childCount + " buildings.");

        //Setup map pin component
        MapPin mapPin = buildings.AddComponent<MapPin>();
        mapPin.Location = new Microsoft.Geospatial.LatLon(latitude, longitude);
        mapPin.UseRealWorldScale = true;
        if (!raycasting) mapPin.Altitude = altitudeOffset;
        mapPin.enabled = true;

        // Ellipsoid: "The altitude reference is based on an ellipsoid which is a mathematical approximation of the shape of the Earth."
        // Meaning the map pin is located at "sea level" rather than at the terrain height of the LatLon location of the pin
        mapPin.AltitudeReference = Microsoft.Geospatial.AltitudeReference.Ellipsoid;
    }

    /* Parse building data from a data-line, and handle input.*/
    private void ParseBuilding(string [] data, long currentLine){
        string y = data[data_y_index];
        string x = data[data_x_index];
        string height = data[data_height_index];

        //Debug.Log("x: " + x + " y: " + y + " height: " + height);
        // Spawn building if the current line has a height-property.
        if(height != null && height != "" && height != " "){
            SpawnBuilding(
            float.Parse(x, CultureInfo.InvariantCulture.NumberFormat), 
            float.Parse(y,CultureInfo.InvariantCulture.NumberFormat), 
            float.Parse(height,CultureInfo.InvariantCulture.NumberFormat),
            "Small Building " + (buildings.transform.childCount +1),
            currentLine
            );
        }

    }

    private void SpawnBuilding(float x, float z, float height, string name, long buildingIndex){
        string [] elevationString = elevationData[buildingIndex].Split(',');

        //TODO: Throw error if x and y values mismatch between building-data and elevation-data.

        Vector3 pos;
        if (!raycasting)
        {
            // Elevation data from dataset:
            float y = float.Parse(
                elevationString[data_height_index],
                CultureInfo.InvariantCulture.NumberFormat
            );
            pos = new Vector3(x, y, z);
        }
        else
        {
            // Elevation data from ray casting:

            // Note: Ray casting can only be done in world space, meaning all coordinates have to be converted from local to world space 
            // before ray casting, and back again afterwards.
            // x and z coordinates are provided in meters, which must be converted to Unity coordinate units through division by metersPerUnit.
            Vector3 origin = worldSpacePin + map.transform.right * (x / (float)metersPerUnit) + map.transform.forward * (z / (float)metersPerUnit);

            // Place all points in world space some (10 for example) units away from the top of the map when ray casting towards the map.
            Vector3 originOffset = origin + map.transform.up * (10.0f * map.transform.lossyScale.y);

            // (map.transform.up * -1) will point towards the "top" (side with the texture) of the map from a point "origin",
            // if "origin" is placed at a positive multiple of map.transform.up away from the map. 
            Ray ray = new Ray(originOffset, map.transform.up * -1);

            MapRendererRaycastHit hitInfo;
            map.GetComponent<MapRenderer>().Raycast(ray, out hitInfo);

            // Transform from world space to local space
            pos = map.transform.InverseTransformVector((hitInfo.Point - worldSpacePin)) * (float)metersPerUnit * map.transform.lossyScale.x;
        }


        GameObject building = Instantiate(smallBuilding, buildings.transform, false);
        building.name = name;
        building.transform.localPosition += pos;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
