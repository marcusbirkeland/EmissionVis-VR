using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloud
{
    public class CloudMapManager : MonoBehaviour
    {
        public GameObject clouds;
        public string imageDirectory;

        private int indexMin = 0;
        private int indexMax = 1;
        
        private readonly List<Renderer> _cloudsRenderers = new();

        // All textures are loaded into this list, and used at runtime.
        private readonly List<CloudMap> _cloudMaps = new();
        
        // Start is called before the first frame update
        void Start()
        {
            LOD[] lods = clouds.GetComponent<LODGroup>().GetLODs();
            foreach (LOD lod in lods)
            {
                foreach (Renderer ren in lod.renderers)
                {
                    _cloudsRenderers.Add(ren);
                    ren.material.SetTexture("_ColorMapMin", null);
                    ren.material.SetTexture("_ColorMapMax", null);
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
                        _cloudMaps.Add(cm);
                    }
                }
            }

            if(_cloudMaps.Count <= 0){
                throw new System.IndexOutOfRangeException("No textures found at: " + imageDirectory);
            }
            //SetMaps();
        }

        
        // Update is called once per frame
        void Update()
        {
            if(_cloudMaps.Count == 4 && _cloudsRenderers[0].material.GetTexture("_ColorMapMin") == null){
                // Sort by time
                _cloudMaps.Sort((c1,c2) => {
                    return c1.Time -c2.Time;
                });
                SetMaps();
            }
        }
        
        
        public int GetMapCount()
        {
            return _cloudMaps.Count;
        }

        private void SetMinMap(int index){
            foreach (Renderer ren in _cloudsRenderers)
            {
                ren.material.SetTexture("_ColorMapMin", _cloudMaps[index].Texture);
            }
        }

        private void SetMaxMap(int index){
            foreach (Renderer ren in _cloudsRenderers)
            {
                ren.material.SetTexture("_ColorMapMax", _cloudMaps[index].Texture);
            }
        }

        private void SetMaps(){
            SetMinMap(indexMin);
            SetMaxMap(indexMax);
        }

        private void IncrementTime(int steps){
            if (indexMax+steps > _cloudMaps.Count){
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
            if(steps >= _cloudMaps.Count){
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
        

        IEnumerator GetAndroidImageFromPath(string fileName)
        {
            // Unity copies any files placed in the folder called StreamingAssets in a Unity Project verbatim to a particular folder on the target machine.
            // To retrieve the folder, use the Application.streamingAssetsPath property.
            // It is not possible to access the StreamingAssets folder on WebGL and Android platforms. Android uses a compressed .apk file.
            // These platforms return a URL. Use the UnityWebRequest class to access the Assets.
            // from: https://docs.unity3d.com/Manual/StreamingAssets.html
            // Place all files you would like to be accessible during runtime inside Assets/StreamingAssets/ Only these will be readable later.

            // skeleton from: https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequestTexture.GetTexture.html
            using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(Application.streamingAssetsPath + "/" + fileName);
        
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                _cloudMaps.Add(new CloudMap(texture, int.Parse(fileName.Split('.')[0])));
            }
        }
    }
}