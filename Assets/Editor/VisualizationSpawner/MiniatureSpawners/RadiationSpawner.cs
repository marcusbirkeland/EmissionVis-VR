using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEngine;
using Visualization;
using Object = UnityEngine.Object;

namespace Editor.VisualizationSpawner.MiniatureSpawners
{
    public class RadiationSpawner : MapVisualizerSpawner
    {
        private readonly Texture2D _radiationImage;
        private readonly Texture2D _heightMap;
        private readonly string _radiationPrefabName;
        
        
        public RadiationSpawner(string mapName, string cdfFilePath, GameObject map, string radiationPrefabName, float rotationAngle) 
            : base(cdfFilePath, map, rotationAngle)
        {
            _radiationImage = LoadFirstRadiationImage(mapName);
            _heightMap = GetHeightMapImg(mapName);
            _radiationPrefabName = radiationPrefabName;
        }

        
        public void SpawnAndSetupRadiation()
        {
            DeletePreviousObject("Radiation Holder");
            CreateRadiationHolder();
            
            SpawnRadiation();
            
            Debug.Log("Finished creating radiation");
        }

        
        private void CreateRadiationHolder()
        {
            VisualizationHolder = new GameObject("Radiation Holder");
            VisualizationHolder.transform.SetParent(Map.transform, false);
            VisualizationHolder.transform.localRotation = Quaternion.Euler(0, RotationAngle, 0);
            
            MapPin mapPin = VisualizationHolder.AddComponent<MapPin>();

            mapPin.Location = SelectedCdfAttributes.position;
            mapPin.IsLayerSynchronized = true;
            mapPin.UseRealWorldScale = true;
            mapPin.ShowOutsideMapBounds = true;
            mapPin.Altitude = 100;
            mapPin.AltitudeReference = AltitudeReference.Surface;
        }

        
        private void SpawnRadiation()
        {
            GameObject radiationPrefab = Resources.Load<GameObject>($"Prefabs/{_radiationPrefabName}");
    
            if (radiationPrefab == null)
            {
                Debug.LogError($"Cloud prefab not found at 'Prefabs/{_radiationPrefabName}'");
                return;
            }

            GameObject rad = Object.Instantiate(radiationPrefab, VisualizationHolder.transform, false);
            rad.name = "Radiation";

            RadiationManager manager = rad.GetComponent<RadiationManager>();
            manager.radiationImage = _radiationImage;
            manager.heightMapImg = _heightMap;
            
            LODGroup lodGroup = rad.GetComponent<LODGroup>();
            lodGroup.size = SelectedCdfAttributes.size.x;

            float scale = SelectedCdfAttributes.size.x / 1000.0f;
            rad.transform.localScale = new Vector3(scale, scale, scale);
        }
        
        
        //TODO: replace radiation display with ability to show all images.
        private static Texture2D LoadFirstRadiationImage(string mapName)
        {
            string folderPath = $"MapData/{mapName}/Radiation";
             
            Texture2D[] textures = Resources.LoadAll<Texture2D>(folderPath);
            
            if (textures.Length == 0)
            {
                Debug.LogError("No textures found in Resources at: " + folderPath);
                return null;
            }

            return textures[0];
        }
    }
}