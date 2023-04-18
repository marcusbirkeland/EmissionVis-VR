using System;
using System.IO;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using UnityEngine;
using Visualization;
using Object = UnityEngine.Object;

namespace Editor.VisualizationSpawner.FullScaleSpawners
{
    public class CloudSpawner : MapVisualizerSpawner
    {
        private readonly string _cloudImagesPath;
        private readonly Texture2D _heightImg;
        private readonly string _cloudPrefabName;

        private GameObject _cloud;

        
        public CloudSpawner(string mapName, string cdfFilePath, GameObject map, string cloudPrefabName, float rotationAngle) 
            : base(cdfFilePath, map, rotationAngle)
        {
            _cloudImagesPath = $"{Application.dataPath}/Resources/MapData/{mapName}/WindSpeed/";
            _heightImg = GetHeightMapImg(mapName);

            if (!DataFilesExist(_cloudImagesPath))
            {
                throw new Exception($"The directory {_cloudImagesPath} does not contain any image files on the .png format, or does not exist.");
            }

            _cloudPrefabName = cloudPrefabName;
        }
        
        
        public void SpawnAndSetupCloud()
        {
            DeletePreviousObject("Cloud Holder");
            
            CreateCloudHolder();
            
            SpawnCloud();

            Debug.Log("Finished creating the cloud");
        }

        
        private void CreateCloudHolder()
        {
            VisualizationHolder = new GameObject("Cloud Holder");
            VisualizationHolder.transform.SetParent(Map.transform, false);

            VisualizationHolder.AddComponent<HPTransform>();

            ArcGISLocationComponent location = VisualizationHolder.AddComponent<ArcGISLocationComponent>();
            location.runInEditMode = true;

            ArcGISPoint pos = new(SelectedCdfAttributes.position.lon, SelectedCdfAttributes.position.lat, 800.0f,
                ArcGISSpatialReference.WGS84());
            
            location.Position = pos;

            location.Rotation = new ArcGISRotation(RotationAngle, 90, 0);
        }

        
        private void SpawnCloud()
        {
            GameObject cloudPrefab = Resources.Load<GameObject>($"Prefabs/{_cloudPrefabName}");
    
            if (cloudPrefab == null)
            {
                Debug.LogError($"Cloud prefab not found at 'Prefabs/{_cloudPrefabName}'");
                return;
            }

            _cloud = Object.Instantiate(cloudPrefab, VisualizationHolder.transform, false);
            _cloud.name = "Cloud";
            
            CloudManager cloudManager = _cloud.GetComponent<CloudManager>();
            cloudManager.imageDirectory = _cloudImagesPath;
            cloudManager.heightMapImg = _heightImg;

            LODGroup lodGroup = _cloud.GetComponent<LODGroup>();
            lodGroup.size = SelectedCdfAttributes.size.x;
            
            //Prefab base size is 1km
            float scale = SelectedCdfAttributes.size.x/1000.0f * UnityUnitsPerMeter;
            _cloud.transform.localScale = new Vector3(scale, scale, scale);
        }


        private static bool DataFilesExist(string dataPath)
        {
            if (!Directory.Exists(dataPath)) return false;

            return Directory.GetFiles(dataPath, "*.png", SearchOption.TopDirectoryOnly).Length > 0;
        }    
    }
}