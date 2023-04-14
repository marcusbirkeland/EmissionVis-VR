using System;
using System.IO;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEngine;
using Visualization;
using Object = UnityEngine.Object;

namespace Editor.VisualizationSpawner
{
    public class CloudSpawner : MapVisualizerSpawner
    {
        private readonly string _imgDataPath;
        private readonly string _cloudPrefabName;
        private readonly string _cloudManagerPrefabName;

        private GameObject _cloud;


        public CloudSpawner(string imageFolderPath, string attributesFilePath, string cdfFilePath, GameObject map, string cloudPrefabName, string cloudManagerPrefabName, float rotationAngle) 
            : base(attributesFilePath, cdfFilePath, map, rotationAngle)
        {
            _imgDataPath = imageFolderPath + "WindSpeed/";

            if (!DataFilesExist(_imgDataPath))
            {
                throw new Exception($"The directory {_imgDataPath} does not contain any image files on the .png format, or does not exist.");
            }
            
            _cloudPrefabName = cloudPrefabName;
            _cloudManagerPrefabName = cloudManagerPrefabName;
        }

        
        public void SpawnAndSetupCloud()
        {
            DeletePreviousObject("Cloud Holder");
            DeletePreviousObject("Cloud Manager");
            
            CreateCloudHolder();
            
            SpawnCloud();
            
            SetupCloudManager();
            
            Debug.Log("Finished creating the cloud");
        }

        
        private void CreateCloudHolder()
        {
            VisualizerHolder = new GameObject("Cloud Holder");
            VisualizerHolder.transform.SetParent(Map.transform, false);
            VisualizerHolder.transform.localRotation = Quaternion.Euler(0, 90 + RotationAngle, 0);

            MapPin mapPin = VisualizerHolder.AddComponent<MapPin>();

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

            _cloud = Object.Instantiate(cloudPrefab, VisualizerHolder.transform, false);

            _cloud.name = "Cloud";

            LODGroup lodGroup = _cloud.GetComponent<LODGroup>();
            lodGroup.size = SelectedCdfAttributes.size.x;

            float scale = SelectedCdfAttributes.size.x / 1000.0f;
            _cloud.transform.localScale = new Vector3(scale, scale, scale);
        }


        private void SetupCloudManager()
        {
            GameObject cloudManagerPrefab  = Resources.Load<GameObject>($"Prefabs/{_cloudManagerPrefabName}");

            if (cloudManagerPrefab == null)
            {
                Debug.LogError($"Cloud manager prefab not found at 'Prefabs/{_cloudManagerPrefabName}'");
                return;
            }

            // Instantiate the cloudManagerObject
            GameObject cloudManagerObject = Object.Instantiate(cloudManagerPrefab);

            cloudManagerObject.name = "Cloud Manager";
            
            CloudManager cloudManager = cloudManagerObject.GetComponent<CloudManager>();
            
            cloudManager.clouds = _cloud;
            cloudManager.imageDirectory = _imgDataPath;

            MapUISetup.SetCloudManager(cloudManager);
        }

        
        private static bool DataFilesExist(string dataPath)
        {
            if (!Directory.Exists(dataPath))
            {
                return false;
            }

            string[] pngFiles = Directory.GetFiles(dataPath, "*.png", SearchOption.TopDirectoryOnly);

            return pngFiles.Length > 0;
        }
    }
}