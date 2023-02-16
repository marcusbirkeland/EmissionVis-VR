using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class CloudMap {
    
    private Texture2D texture;
    private int timeSeconds;

    public CloudMap(Texture2D tex, int t){
        this.texture = tex;
        this.timeSeconds = t;
    }

    public Texture2D GetTexture(){
        return texture;
    }

    public int GetTime(){
        return timeSeconds;
    }
}

public class CloudMapManager : MonoBehaviour
{

    public GameObject clouds;
    public string imageDirectory = "Assets/Resources/ImageData/Bergen/Emission";

    private int indexMin = 0;
    private int indexMax = 1;
    private Renderer cloudsRenderer;

    // All textures are loaded into this list, and used at runtime.
    private List<CloudMap> cloudMaps;

    private void SetMinMap(int index){
        cloudsRenderer.material.SetTexture("_ColorMapMin", cloudMaps[index].GetTexture());
    }

    private void SetMaxMap(int index){
        cloudsRenderer.material.SetTexture("_ColorMapMax", cloudMaps[index].GetTexture());
    }

    private void SetMaps(){
        SetMinMap(indexMin);
        SetMaxMap(indexMax);
    }

    private void IncrementTime(int steps){
        if (indexMax+steps > cloudMaps.Count){
            throw new System.NullReferenceException("Index out of range");
        }
        indexMin+= steps;
        indexMax+= steps;
    }

    private void DecrementTime(int steps){
        if(indexMin-steps < 0){
            throw new System.IndexOutOfRangeException("Index out of range");
        }
        indexMin+= steps;
        indexMax+= steps;
    }

    public void UpdateTime(int steps){
        if(steps >= cloudMaps.Count){
            throw new System.NullReferenceException("Index out of range. Too many timesteps jumped");
        }

        Debug.Log("Update time steps: " + steps);

        if(steps < 0){
            DecrementTime(steps);
        }

        else if(steps > 0){
            IncrementTime(steps);
        }
        Debug.Log("INDEX MAX: " + indexMax + "\nINDEX MIN: " + indexMin);
        SetMaps();
    }


    // Start is called before the first frame update
    void Start()
    {
        cloudsRenderer = clouds.GetComponent<Renderer>();
        cloudMaps = new List<CloudMap>();
        DirectoryInfo info = new DirectoryInfo(imageDirectory);
        FileInfo [] fileInfo = info.GetFiles();

        // Load all png's from folder into the CloudMaps list.
        foreach(FileInfo file in fileInfo){
            if(file.Extension.Equals(".png") || file.Extension.Equals(".PNG")){
                Texture2D texture = new Texture2D(1,1);
                Debug.Log("FOUND TEXUTRE: " + file.FullName);
                byte [] bytes = File.ReadAllBytes(file.FullName);
                
                texture.LoadImage(bytes);
                Debug.Log("Filename: " + file.Name);
                int seconds = int.Parse(file.Name.Split('.')[0]);
                CloudMap cm = new CloudMap(texture, seconds);
                cloudMaps.Add(cm);
            }
        }

        if(cloudMaps.Count <= 0){
            throw new System.IndexOutOfRangeException("No textures found at: " + imageDirectory);
        }

        if(cloudMaps.Count == 1){
            indexMax = 0;
        }

        // Sort by time
        cloudMaps.Sort((c1,c2) => {
            return c1.GetTime() -c2.GetTime();
        });

        SetMaps();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
