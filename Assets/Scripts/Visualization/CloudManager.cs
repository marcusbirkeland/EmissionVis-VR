using System.Collections;
using System.Collections.Generic;
using System.IO;
using MapUI;
using Microsoft.Maps.Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace Visualization
{
    public class CloudManager : MonoBehaviour
    {
        public GameObject clouds;
        public string imageDirectory;

        public float heightValueMultiplier = 1000;

        private double baseElevation;
        
        private int _index;
        
        private readonly List<Renderer> _cloudRenderers = new();
        
        // All textures are loaded into this list, and used at runtime.
        private readonly List<CloudMap> _cloudMaps = new();

        public int MapCount => _cloudMaps.Count;


        //More efficient to store these as static variables.
        private static readonly int ColorMapMin = Shader.PropertyToID("_ColorMapMin");
        private static readonly int ColorMapMax = Shader.PropertyToID("_ColorMapMax");
        private static readonly int ColorMapAlpha = Shader.PropertyToID("_ColorMapAlpha");
        private static readonly int TerrainCurvature = Shader.PropertyToID("_Terrain_Curvature");
        private static readonly int Opacity = Shader.PropertyToID("_Opacity");


        void Start()
        {

            MapPin mapPin = clouds.GetComponentInParent<MapPin>();
            
            if(mapPin){
               baseElevation = mapPin.Altitude;
            }
            LOD[] lods = clouds.GetComponent<LODGroup>().GetLODs();
            foreach (LOD lod in lods)
            {
                foreach (Renderer ren in lod.renderers)
                {
                    _cloudRenderers.Add(ren);
                    ren.material.SetTexture(ColorMapMin, null);
                    ren.material.SetTexture(ColorMapMax, null);
                }
            }

            // Android has a different file reading mechanism
            if (Application.platform == RuntimePlatform.Android)
            {
                // TODO: change to find all png files and their associated time/name
                for (int i = 0; i < 4; i++)
                {
                    string fileName = i + ".png";
                    StartCoroutine(GetAndroidImageFromPath(fileName));
                }
            } 
            else
            {
                DirectoryInfo info = new (imageDirectory);
                FileInfo[] fileInfo = info.GetFiles();

                // Load all pngs from folder into the CloudMaps list.
                foreach (FileInfo file in fileInfo)
                {
                    if (!file.Extension.ToLower().Equals(".png")) continue;
                    
                    Texture2D texture = new(1, 1);
                    Debug.Log("FOUND TEXTURE: " + file.FullName);
                    byte[] bytes = File.ReadAllBytes(file.FullName);

                    texture.LoadImage(bytes);
                    Debug.Log("Filename: " + file.Name);
                    int seconds = int.Parse(file.Name.Split('.')[0]);
                    CloudMap cm = new(texture, seconds);
                    _cloudMaps.Add(cm);  
                }
            }

            if(MapCount <= 0)
            {
                throw new System.IndexOutOfRangeException("No textures found at: " + imageDirectory);
            }

            //StartCoroutine(SetMapsWhenReady());
        }

        public void ChangeAlpha(float value)
        {
            foreach (Renderer ren in _cloudRenderers)
            {
                ren.material.SetFloat(Opacity, value);
            }
        }

        public void ChangeHeight(float value){
            float maxValue = 1;
             // TODO: fix so this works in fullscale too
            MapPin mapPin = clouds.GetComponentInParent<MapPin>();
            
            if(mapPin){
                mapPin.Altitude = value*heightValueMultiplier + baseElevation;
            }

            foreach (Renderer ren in _cloudRenderers)
            {
                ren.material.SetFloat(TerrainCurvature, 1-(value / maxValue));
            }
        }
        
        
        public void UpdateAlphaForRenderers(float time)
        {
            foreach (Renderer ren in _cloudRenderers)
            {
                ren.material.SetFloat(ColorMapAlpha, time % 1);
            }
        }

        public void UpdateTime(int steps)
        {
            if(steps >= _cloudMaps.Count)
            {
                throw new System.NullReferenceException("Index out of range. Too many time steps jumped");
            }

            Debug.Log("Update time steps: " + steps);

            switch (steps)
            {
                case < 0:
                    DecrementTime(steps);
                    break;
                case > 0:
                    IncrementTime(steps);
                    break;
            }
            
            Debug.Log($"INDEX MAX: {_index + 1} \nINDEX MIN: {_index}");
            SetMaps();
        }
        
        
        //TODO: this should probably get changed to not require exactly four timestamps.
        //I also dont think the texture check is necessary.
        private IEnumerator SetMapsWhenReady()
        {
            yield return new WaitUntil(() =>
                _cloudMaps.Count == 4 && _cloudRenderers[0].material.GetTexture(ColorMapMin) == null);
            
            Debug.Log("Maps have been set");
            
            _cloudMaps.Sort((c1,c2) => c1.Time - c2.Time);
            SetMaps();
        }
        
        //Replace with coroutine above when it is functional 
        void Update()
        {
            if(_cloudMaps.Count == 4 && _cloudRenderers[0].material.GetTexture(ColorMapMin) == null){
                // Sort by time
                _cloudMaps.Sort((c1,c2) => {
                    return c1.Time -c2.Time;
                });
                SetMaps();
            }
        }
        
        private void SetMaps()
        {
            foreach (Renderer ren in _cloudRenderers)
            {
                ren.material.SetTexture(ColorMapMin, _cloudMaps[_index].Texture);
                ren.material.SetTexture(ColorMapMax, _cloudMaps[_index + 1].Texture);
            }
        }

        private void IncrementTime(int steps)
        {
            if (_index + steps + 1 > _cloudMaps.Count)
            {
                throw new System.NullReferenceException("Index out of range");
            }
            _index += steps;
        }

        private void DecrementTime(int steps)
        {
            if(_index-steps < 0)
            {
                throw new System.IndexOutOfRangeException("Index out of range");
            }
            _index += steps;
        }
        

        private IEnumerator GetAndroidImageFromPath(string fileName)
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