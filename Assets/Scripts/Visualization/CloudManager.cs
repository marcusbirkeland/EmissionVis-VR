using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Maps.Unity;
using UnityEngine;
using UnityEngine.Networking;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;

namespace Visualization
{
    public class CloudManager : MonoBehaviour
    {
        public GameObject clouds;
        public string imageDirectory;

        public float heightValueMultiplier = 1000;

        private double baseElevation;

        [Range(0.0f, 1.0f)]
        public float debugElevationSlider = 0.0f;

        [Range(0.0f, 1.0f)]
        public float debugAlphaSlider = 1.0f;
        

        private int _index;
        
        private readonly List<Renderer> _cloudRenderers = new();
        
        // All textures are loaded into this list, and used at runtime.
        private readonly List<CloudMap> _cloudMaps = new();
        public int MapCount => _cloudMaps.Count;
        
        private ArcGISLocationComponent arcGISLocation;
        private MapPin mapPin;

        //More efficient to store these as static variables.
        private static readonly int ColorMapMin = Shader.PropertyToID("_ColorMapMin");
        private static readonly int ColorMapMax = Shader.PropertyToID("_ColorMapMax");
        private static readonly int ColorMapAlpha = Shader.PropertyToID("_ColorMapAlpha");
        private static readonly int TerrainCurvature = Shader.PropertyToID("_Terrain_Curvature");
        private static readonly int Opacity = Shader.PropertyToID("_Opacity");


        void Start()
        {
            // Setup elevation variables.
            mapPin = clouds.GetComponentInParent<MapPin>();
            arcGISLocation = clouds.GetComponentInParent<ArcGISLocationComponent>();
            if(mapPin){
               baseElevation = mapPin.Altitude;
            }
            else if(arcGISLocation){
               baseElevation = arcGISLocation.Position.Z;
            }

            // Set materials and textures
            UnsetMaterials();
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
                LoadTexturesPC();

            if(MapCount <= 0)
                throw new System.IndexOutOfRangeException("No textures found at: " + imageDirectory);

            //StartCoroutine(SetMapsWhenReady());
        }

        // Replace with coroutine when it works:)
        void Update()
        {
            if(_cloudMaps.Count == 4 && _cloudRenderers[0].material.GetTexture(ColorMapMin) == null){
                // Sort by time
                _cloudMaps.Sort((c1,c2) => {
                    return c1.Time -c2.Time;
                });
                SetMaps();
            }
            // Very useful for debugging. Should probably stay for the folks at Sintef:)
            if (Application.isEditor){
                ChangeHeight(debugElevationSlider);
                ChangeOpacity(debugAlphaSlider);
            }

        }
        /* Changes opacity for the clouds-shader */
        public void ChangeOpacity(float value)
        {
            foreach (Renderer ren in _cloudRenderers)
            {
                ren.material.SetFloat(Opacity, value);
            }
        }

        /* Updates the time-alpha in the clouds-material, interpolating between the two loaded textures*/
        public void ChangeTimestep(float time)
        {
            foreach (Renderer ren in _cloudRenderers)
            {
                ren.material.SetFloat(ColorMapAlpha, time % 1);
            }
        }
        
        /* Changes the physical height of the clouds in the scene, interpolating the heightmap more when reaching the baseElevation*/
        public void ChangeHeight(float value){
            if(mapPin){
                mapPin.Altitude = baseElevation + value*heightValueMultiplier;
            }
            else if(arcGISLocation){
                arcGISLocation.Position = new ArcGISPoint(
                    arcGISLocation.Position.X, 
                    arcGISLocation.Position.Y, 
                    baseElevation + value*heightValueMultiplier, 
                    ArcGISSpatialReference.WGS84()
                );
            }
            
            foreach (Renderer ren in _cloudRenderers)
            {
                // To create a steeper curve.
                if ( value >= 0.1f){
                    value*= 1.2f;
                }
                ren.material.SetFloat(TerrainCurvature, Mathf.Clamp01(1-(value+0.25f / 1)));
            }
        }
        
        public void UpdateTime(int steps)
        {
            if(steps >= _cloudMaps.Count)
            {
                throw new System.NullReferenceException("Index out of range. Too many time steps jumped");
            }
            Debug.Log("Update time steps: " + steps);
            if (steps < 0)
            {
                DecrementTime(steps);
            }
            else
                IncrementTime(steps);
            
            Debug.Log($"INDEX MAX: {_index + 1} \nINDEX MIN: {_index}");
            SetMaps();
        }

        private void IncrementTime(int steps)
        {
            if (_index + steps + 1 > _cloudMaps.Count)
                throw new System.NullReferenceException("Index out of range");
            _index += steps;
        }

        private void DecrementTime(int steps)
        {
            if(_index-steps < 0)
                throw new System.IndexOutOfRangeException("Index out of range");
            _index += steps;
        }
        
        
        //TODO: this should probably get changed to not require exactly four timestamps.
        private IEnumerator SetMapsWhenReady()
        {
            yield return new WaitUntil(() =>
                _cloudMaps.Count == 4 && _cloudRenderers[0].material.GetTexture(ColorMapMin) == null);
            
            Debug.Log("Maps have been set");
            
            _cloudMaps.Sort((c1,c2) => c1.Time - c2.Time);
            SetMaps();
        }
        
        private void SetMaps()
        {
            foreach (Renderer ren in _cloudRenderers)
            {
                ren.material.SetTexture(ColorMapMin, _cloudMaps[_index].Texture);
                ren.material.SetTexture(ColorMapMax, _cloudMaps[_index + 1].Texture);
            }
        }

        private void UnsetMaterials(){
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
        }
        
        private void LoadTexturesPC(){
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