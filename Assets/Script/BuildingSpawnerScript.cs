using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEditor;

public class BuildingSpawnerScript : MonoBehaviour
{
    private const string EDITOR_NAME = "Building Spawner";
    private const int PROGRESS_UPDATE_RATE = 1;
    public string dataPath ="Assets/Resources/Bergen/Building/buildings.csv";

    public int data_y_index = 0;
    public int data_x_index = 1;
    public int data_height_index = 2;

    public GameObject map;
    // Buildings to spawn
    public GameObject smallBuilding;
    public GameObject largeBuilding;

    private GameObject buildings;

    public void SpawnBuildings(){
        Debug.Log("Spawning buildings...");

        // Instantiate new GameObject as child of map.
        buildings = new GameObject("Map Buildings");
        buildings.transform.parent = map.transform;

        System.IO.StreamReader sr = new System.IO.StreamReader(dataPath);
        if(sr.Peek() < 0){
            throw new System.ArgumentException("The file at " + dataPath + " is empty!");
        }

        //TODO: Can expand to automatically set data indecies based on header.
        // Read first line
        sr.ReadLine();

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
        Debug.Log("Num lines: " + numLines);
        // Read file line by line
        while(sr.Peek() >= 0){
            string line = sr.ReadLine();
            //Debug.Log("line: " + line);

            string [] data = line.Split(',');
            ParseBuilding(data);

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

    }

    /* Parse building data from a data-line, and handle input.*/
    private void ParseBuilding(string [] data){
        string y = data[data_y_index];
        string x = data[data_x_index];
        string height = data[data_height_index];

        Debug.Log("x: " + x + " y: " + y + " height: " + height);
        // Spawn building if the current line has a height-property.
        if(height != null && height != "" && height != " "){
            SpawnBuilding(
            float.Parse(x, CultureInfo.InvariantCulture.NumberFormat), 
            float.Parse(y,CultureInfo.InvariantCulture.NumberFormat), 
            float.Parse(height,CultureInfo.InvariantCulture.NumberFormat)
            );
        }

    }

    private void SpawnBuilding(float x, float z, float height){
        Vector3 pos = new Vector3(x, height, z);
        GameObject building = Instantiate(smallBuilding, buildings.transform, false);
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
