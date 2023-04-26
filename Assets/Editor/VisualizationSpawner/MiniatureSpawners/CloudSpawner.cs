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
        private readonly Texture2D _heightImg;
        private readonly string _cloudPrefabName;
        private readonly string _mapName;
        private readonly double _elevation;

        private GameObject _cloud;

        
        public CloudSpawner(string mapName, string cdfFilePath, GameObject map, string cloudPrefabName, float rotationAngle, double elevation) 
            : base(cdfFilePath, map, rotationAngle)
        {
            _elevation = elevation;
            _heightImg = GetHeightMapImg(mapName);
            _mapName = mapName;
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
            mapPin.Altitude = _elevation;
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
            cloudManager.heightMapImg = _heightImg;

            cloudManager.cloudImages = CloudManagerInitializer.GetImages(_mapName);
            cloudManager.baseElevation = _elevation;
            
            LODGroup lodGroup = _cloud.GetComponent<LODGroup>();
            lodGroup.size = SelectedCdfAttributes.size.x;

            float scale = SelectedCdfAttributes.size.x / 1000.0f;
            _cloud.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}