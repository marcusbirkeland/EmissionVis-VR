using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using UnityEngine;
using Visualization;

namespace Editor.VisualizationSpawner.FullScaleSpawners
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

            VisualizationHolder.AddComponent<HPTransform>();

            ArcGISLocationComponent location = VisualizationHolder.AddComponent<ArcGISLocationComponent>();
            location.runInEditMode = true;

            ArcGISPoint pos = new(SelectedCdfAttributes.position.lon, SelectedCdfAttributes.position.lat, 350.0f,
                ArcGISSpatialReference.WGS84());
            
            location.Position = pos;

            location.Rotation = new ArcGISRotation(RotationAngle, 90, 0);
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

            //Prefab base size is 1km
            float scale = SelectedCdfAttributes.size.x / 1000.0f * UnityUnitsPerMeter;
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