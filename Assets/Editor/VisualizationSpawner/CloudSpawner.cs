using System;
using System.IO;
using Editor.NetCDF;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using NewMapUI;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.VisualizationSpawner
{
    public class CloudSpawner
    {
        private readonly AttributeDataGetter.FileAttributes _selectedCdfAttributes;
        private readonly string _imgDataPath;
        private readonly string _cloudPrefabName;
        private readonly string _cloudManagerPrefabName;
        private readonly GameObject _map;
        private readonly float _rotationAngle;

        private GameObject _cloud;
        private GameObject _cloudHolder;


        public CloudSpawner(string imageFolderPath, string attributesFilePath, string cdfFilePath, GameObject map, string cloudPrefabName, string cloudManagerPrefabName, float rotationAngle)
        {
            _imgDataPath = imageFolderPath + "WindSpeed/";

            if (!DataFilesExist(_imgDataPath))
            {
                throw new Exception($"The directory {_imgDataPath} does not contain any image files on the .png format, or does not exist.");
            }
            
            _selectedCdfAttributes = AttributeDataGetter.GetFileAttributes(attributesFilePath, cdfFilePath);
            
            MapRenderer mapRenderer = map.GetComponent<MapRenderer>();
            if (mapRenderer == null)
            {
                throw new Exception("The selected Map GameObject does not have a MapRenderer component. Please select a GameObject with the MapRenderer component.");
            }
            _map = map;
            
            _cloudPrefabName = cloudPrefabName;
            _cloudManagerPrefabName = cloudManagerPrefabName;

            _rotationAngle = rotationAngle;
        }

        
        public void SpawnAndSetupCloud()
        {
            if (!DataFilesExist(_imgDataPath))
            {
                Debug.LogError($"The directory '{_imgDataPath}' does not exist.");
                return;
            }

            MapRenderer mapRenderer = _map.GetComponent<MapRenderer>();
            if (mapRenderer == null) return;

            EditorUtility.DisplayProgressBar("Creating clouds", "Deleting previous clouds", -1);
            DeletePreviousCloud();
            
            EditorUtility.DisplayProgressBar("Creating clouds", "Creating clouds...", -1);
            CreateCloudHolder();
            SpawnCloud();
            
            EditorUtility.DisplayProgressBar("Creating clouds", "Setting up cloud manager...", -1);
            SetupCloudManager();
            
            EditorUtility.ClearProgressBar();
            
            Debug.Log("Finished creating the cloud");
        }

        
        private void CreateCloudHolder()
        {
            _cloudHolder = new GameObject("Cloud Holder");
            _cloudHolder.transform.SetParent(_map.transform, false);
            _cloudHolder.transform.localRotation = Quaternion.Euler(0, 90 + _rotationAngle, 0);

            MapPin mapPin = _cloudHolder.AddComponent<MapPin>();

            mapPin.Location = _selectedCdfAttributes.position;
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

            _cloud = Object.Instantiate(cloudPrefab, _cloudHolder.transform, false);

            _cloud.name = "Cloud";

            LODGroup lodGroup = _cloud.GetComponent<LODGroup>();
            lodGroup.size = _selectedCdfAttributes.size.x;

            float scale = _selectedCdfAttributes.size.x / 1000.0f;
            _cloud.transform.localScale = new Vector3(scale, scale, scale);
        }

        
        private void DeletePreviousCloud()
        {
            for (int i = _map.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = _map.transform.GetChild(i);
                if (child.name == "Cloud Holder" || child.name == "Cloud Manager")
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
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