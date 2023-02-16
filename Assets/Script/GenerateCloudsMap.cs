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

    public Texture2D getTexture(){
        return texture;
    }

    public int getTime(){
        return timeSeconds;
    }
}

public class GenerateCloudsMap : MonoBehaviour
{

    public GameObject clouds;
    public string imageDirectory = "ImageData/Bergen/Emission";


    public int index = 0;

    private int prevIndex = 0;

    private Renderer cloudsRenderer;

    private List<CloudMap> CloudMaps;

    // Start is called before the first frame update
    void Start()
    {
        cloudsRenderer = clouds.GetComponent<Renderer>();
        CloudMaps = new List<CloudMap>();
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
                CloudMaps.Add(cm);
            }
        }
        // Sort mayb.

        cloudsRenderer.material.SetTexture("_ColorMap", CloudMaps[index].getTexture());
    }

    // Update is called once per frame
    void Update()
    {
        if(index != prevIndex){
            cloudsRenderer.material.SetTexture("_ColorMap", CloudMaps[index].getTexture());
            prevIndex = index;
        }
    }
}
