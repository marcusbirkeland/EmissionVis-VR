using System;
using System.IO;
using Cloud;
using Editor.NetCDF;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.CloudSpawner
{
    public class CloudSpawner
    {
        private readonly MapDataLoader.CdfData _selectedCdfData;
        private readonly string _imgDataPath;
        private readonly string _cloudPrefabName;
        private readonly GameObject _map;
        private readonly float _rotationAngle;

        private GameObject _cloud;
        private GameObject _cloudHolder;


        public CloudSpawner(string jsonDataPath, string attributesFilePath, string cdfFilePath, GameObject map, string cloudPrefabName, float rotationAngle)
        {
            _imgDataPath = jsonDataPath + "WindSpeed/";

            if (!DataFilesExist(_imgDataPath))
            {
                throw new Exception($"The directory {_imgDataPath} does not contain any image files on the .png format, or does not exist.");
            }

            _selectedCdfData = MapDataLoader.LoadMapData(attributesFilePath, cdfFilePath);

            MapRenderer mapRenderer = _map.GetComponent<MapRenderer>();
            if (mapRenderer == null)
            {
                throw new Exception("The selected Map GameObject does not have a MapRenderer component. Please select a GameObject with the MapRenderer component.");
            }
            _map = map;
            
            _cloudPrefabName = cloudPrefabName;

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
            _cloudHolder = new GameObject("Cloud");
            _cloudHolder.transform.SetParent(_map.transform, false);
            _cloudHolder.transform.localRotation = Quaternion.Euler(0, _rotationAngle, 0);

            MapPin mapPin = _cloudHolder.AddComponent<MapPin>();

            mapPin.Location = _selectedCdfData.position;
            mapPin.IsLayerSynchronized = true;
            mapPin.UseRealWorldScale = true;
            mapPin.ShowOutsideMapBounds = true;
            mapPin.AltitudeReference = AltitudeReference.Ellipsoid;
        }

        
        private void SpawnCloud()
        {
            _cloud = Resources.Load<GameObject>($"Prefabs/{_cloudPrefabName}");
            _cloud.transform.SetParent(_cloudHolder.transform, false);
            _cloudHolder.transform.localRotation = Quaternion.Euler(0, 90 + _rotationAngle, 0);
            
            LODGroup lodGroup = _cloud.GetComponent<LODGroup>();
            
            //TODO: replace with dynamic value from attributes.json
            lodGroup.size = 3072;
        }

        
        private void DeletePreviousCloud()
        {
            for (int i = _map.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = _map.transform.GetChild(i);
                if (child.name == "Cloud")
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }

        
        private void SetupCloudManager()
        {
            CloudMapManager cloudManager = _map.GetComponent<CloudMapManager>();

            cloudManager.clouds = _cloud;
            cloudManager.imageDirectory = _imgDataPath;
            
            CloudSlider slider = _cloud.GetComponent<CloudSlider>();
            slider.cloudManager = cloudManager.gameObject;
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