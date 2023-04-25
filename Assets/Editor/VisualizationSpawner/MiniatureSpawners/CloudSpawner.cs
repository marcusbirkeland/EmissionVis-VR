using System;
using System.IO;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEngine;
using Visualization;
using Object = UnityEngine.Object;

namespace Editor.VisualizationSpawner.MiniatureSpawners
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
            VisualizationHolder.transform.localRotation = Quaternion.Euler(0, RotationAngle, 0);

            MapPin mapPin = VisualizationHolder.AddComponent<MapPin>();

            mapPin.Location = SelectedCdfAttributes.position;
            mapPin.IsLayerSynchronized = true;
            mapPin.UseRealWorldScale = true;
            mapPin.ShowOutsideMapBounds = true;
            mapPin.Altitude = 500;
            mapPin.AltitudeReference = AltitudeReference.Surface;
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

            float scale = SelectedCdfAttributes.size.x / 1000.0f;
            _cloud.transform.localScale = new Vector3(scale, scale, scale);
        }


        private static bool DataFilesExist(string dataPath)
        {
            if (!Directory.Exists(dataPath)) return false;

            return Directory.GetFiles(dataPath, "*.png", SearchOption.TopDirectoryOnly).Length > 0;
        }
    }
}