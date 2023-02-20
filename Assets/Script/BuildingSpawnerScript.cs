using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEditor;
using Microsoft.Maps.Unity;
using Microsoft.Geospatial;

public class BuildingSpawnerScript : MonoBehaviour
{
    private const string EDITOR_NAME = "Building Spawner";
    private const int PROGRESS_UPDATE_RATE = 1;
    public string dataPath ="Assets/Resources/Bergen/Building/buildings.csv";

    public bool useElevationData = true;
    public string elevationDataPath="Assets/Resources/Bergen/Elevation/elevation.csv";

    public Vector3 positionOffset;

    public float xAngle=0, yAngle=-90, zAngle=0;

    public float altitudeOffset = -250;

    public double latitude = 60.35954907032411;
    public double longitude = 5.314180944287559;

    public int data_y_index = 0;
    public int data_x_index = 1;
    public int data_height_index = 2;

    public GameObject map;
    public GameObject smallBuilding;
    public GameObject largeBuilding;
    private GameObject buildings;

    private string[] elevationData;

    public void SpawnBuildings(){
        Debug.Log("Spawning buildings...");

        // Instantiate new GameObject as child of map.
        buildings = new GameObject("Map Buildings");
        buildings.transform.parent = map.transform;

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

        // Read file line by line
        while(sr.Peek() >= 0){
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

        buildings.transform.Rotate(xAngle, yAngle, zAngle);

        //Setup map pin component
        MapPin mapPin = buildings.AddComponent<MapPin>();
        mapPin.Location = new Microsoft.Geospatial.LatLon(latitude, longitude);
        mapPin.UseRealWorldScale = true;
        mapPin.Altitude = altitudeOffset;
        mapPin.enabled = true;

    }

    /* Parse building data from a data-line, and handle input.*/
    private void ParseBuilding(string [] data, long currentLine){
        string y = data[data_y_index];
        string x = data[data_x_index];
        string height = data[data_height_index];

        Debug.Log("x: " + x + " y: " + y + " height: " + height);
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

        float y = float.Parse(
            elevationString[data_height_index] ,
            CultureInfo.InvariantCulture.NumberFormat
        );

        Vector3 pos = new Vector3(x, y, z);
        GameObject building = Instantiate(smallBuilding, buildings.transform, false);
        building.name = name;
        building.transform.position += pos;
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
