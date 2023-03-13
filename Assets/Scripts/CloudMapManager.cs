using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using UnityEngine.Networking;

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
    private List<Renderer> cloudsRenderers = new List<Renderer>();

    // All textures are loaded into this list, and used at runtime.
    private List<CloudMap> cloudMaps = new List<CloudMap>();

    public int GetMapCount()
    {
        return cloudMaps.Count;
    }

    private void SetMinMap(int index){
        foreach (Renderer ren in cloudsRenderers)
        {
            ren.material.SetTexture("_ColorMapMin", cloudMaps[index].GetTexture());
        }
    }

    private void SetMaxMap(int index){
        foreach (Renderer ren in cloudsRenderers)
        {
            ren.material.SetTexture("_ColorMapMax", cloudMaps[index].GetTexture());
        }
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
        LOD[] lods = clouds.GetComponent<LODGroup>().GetLODs();
        foreach (LOD lod in lods)
        {
            foreach (Renderer ren in lod.renderers)
            {
                cloudsRenderers.Add(ren);
            }

        }
        //cloudsRenderers = clouds.GetComponent<Renderer>();


        // Android has a different file reading mechanism
        if (Application.platform == RuntimePlatform.Android)
        {
            // TODO: change to find all png files and their associated time/name
            for (int i = 0; i < 4; i++)
            {
                string fileName = i.ToString() + ".png";
                StartCoroutine(GetAndroidImageFromPath(fileName));
            }
                //SetMaps();
        } 
        else
        {
            DirectoryInfo info = new DirectoryInfo(imageDirectory);
            FileInfo[] fileInfo = info.GetFiles();

            // Load all png's from folder into the CloudMaps list.
            foreach (FileInfo file in fileInfo)
            {
                if (file.Extension.Equals(".png") || file.Extension.Equals(".PNG"))
                {
                    Texture2D texture = new Texture2D(1, 1);
                    Debug.Log("FOUND TEXUTRE: " + file.FullName);
                    byte[] bytes = File.ReadAllBytes(file.FullName);

                    texture.LoadImage(bytes);
                    Debug.Log("Filename: " + file.Name);
                    int seconds = int.Parse(file.Name.Split('.')[0]);
                    CloudMap cm = new CloudMap(texture, seconds);
                    cloudMaps.Add(cm);
                }
            }
        }

        if(cloudMaps.Count <= 0){
            throw new System.IndexOutOfRangeException("No textures found at: " + imageDirectory);
        }
        //SetMaps();
    }

    // Update is called once per frame
    void Update()
    {
        if(cloudMaps.Count == 4 && cloudsRenderers[0].material.GetTexture("_ColorMapMin") == null){
            // Sort by time
            cloudMaps.Sort((c1,c2) => {
                return c1.GetTime() -c2.GetTime();
            });
            SetMaps();
        }
    }

    IEnumerator GetAndroidImageFromPath(string fileName)
    {

        // Unity copies any files placed in the folder called StreamingAssets in a Unity Project verbatim to a particular folder on the target machine.
        // To retrieve the folder, use the Application.streamingAssetsPath property.
        // It is not possible to access the StreamingAssets folder on WebGL and Android platforms. Android uses a compressed .apk file.
        // These platforms return a URL. Use the UnityWebRequest class to access the Assets.
        // from: https://docs.unity3d.com/Manual/StreamingAssets.html
        // Place all files you would like to be accessible during runtime inside Assets/StreamingAssets/ Only these will be readable later.

        // skeleton from: https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequestTexture.GetTexture.html
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(Application.streamingAssetsPath + "/" + fileName))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                cloudMaps.Add(new CloudMap(texture, int.Parse(fileName.Split('.')[0])));
            }
        }
    }
}
