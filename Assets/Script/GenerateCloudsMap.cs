using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CloudMap {
    
    private Texture2D texture;
    private string timeString;

    public CloudMap(Texture2D tex, string t){
        this.texture = tex;
        this.timeString = t;
    }

    public Texture2D getTexture(){
        return texture;
    }

    public string getTime(){
        return timeString;
    }
}

public class GenerateCloudsMap : MonoBehaviour
{

    public GameObject clouds;
    public string csvPath = "Assets/DATA/emission.csv";

    public string filename = "wspeed";

    private Renderer cloudsRenderer;

    private List<CloudMap> CloudTextureMaps;

    private Texture2D createCloudMap(string [] data, int offset=1){
        // TODO: create texture.
    }

    // Start is called before the first frame update
    void Start()
    {
       cloudsRenderer = clouds.GetComponent<Renderer>();
        TextAsset data = Resources.Load<TextAsset>(filename);
        string [] dataLines = data.text.Split('\n');

        for (int i = 1; i < dataLines.Length; i++){
            //TODO: make stuff
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
