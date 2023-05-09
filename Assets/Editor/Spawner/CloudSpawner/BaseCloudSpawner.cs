using System;
using System.Collections.Generic;
using Editor.NetCDF;
using Editor.NetCDF.Types;
using UnityEditor;
using UnityEngine;
using Visualization;
using Object = UnityEngine.Object;

namespace Editor.Spawner.CloudSpawner
{
    /// <summary>
    /// The BaseCloudSpawner class is an abstract class used for spawning clouds in different types of maps.
    /// </summary>
    public abstract class BaseCloudSpawner
    {
        /// <summary>
        /// The name of the holder GameObject.
        /// </summary>
        protected const string HolderName = "Cloud Holder";

        
        /// <summary>
        /// The holder GameObject.
        /// </summary>
        protected GameObject CloudHolder;

        
        /// <summary>
        /// The scope of the dataset containing the cloud data.
        /// </summary>
        protected readonly DatasetScope SelectedDatasetScope;
        
        /// <summary>
        /// The GameObject that will contain the cloud holder.
        /// </summary>
        protected readonly GameObject Map;
        
        /// <summary>
        /// How many degrees to rotate the data.
        /// It rotates based on the origin position defined in <see cref="SelectedDatasetScope"/>
        /// </summary>
        protected readonly float RotationAngle;

        private readonly GameObject _cloudPrefab;
        private readonly Texture2D _heightImg;
        private readonly string _mapName;

        protected abstract double Elevation { get; }
        protected abstract string PrefabName { get; }

        
        /// <summary>
        /// The mercator projection gets more distorted the further away from the equator it is.
        /// This value accounts for that.
        /// </summary>
        protected virtual float LatDistortionValue =>
            (float) (1 / Math.Cos(Math.PI * SelectedDatasetScope.position.lat / 180.0));

        
        /// <summary>
        /// Initializes a new instance of the BaseCloudSpawner class.
        /// </summary>
        /// <param name="mapName">The name of the map.</param>
        /// <param name="cdfFilePath">The path to the cloud NetCDF file.</param>
        /// <param name="map">The map GameObject.</param>
        /// <param name="rotationAngle">The rotation angle for the cloud holder.</param>
        protected BaseCloudSpawner(string mapName, string cdfFilePath, GameObject map, float rotationAngle)
        {
            SelectedDatasetScope = ScopeDataGetter.GetDatasetScope(cdfFilePath);

            _cloudPrefab = Resources.Load<GameObject>($"Prefabs/{PrefabName}");

            if (_cloudPrefab == null)
            {
                Debug.LogError($"Cloud prefab not found at 'Assets/Resources/Prefabs/{PrefabName}'");
                return;
            }

            Map = map;
            RotationAngle = rotationAngle;
            _mapName = mapName;
            _heightImg = ImageLoader.GetHeightMapImg(mapName);
        }

        
        /// <summary>
        /// Main method.
        /// Spawns and sets up the cloud in the map.
        /// </summary>
        public void SpawnAndSetupCloud()
        {
            DeletePreviousObject();

            CreateCloudHolder();

            CreateCloud();

            Debug.Log("Finished creating the cloud");
        }

        
        /// <summary>
        /// Creates the cloud holder GameObject specific to the type of map.
        /// </summary>
        protected abstract void CreateCloudHolder();

        
        /// <summary>
        /// Creates the cloud GameObject and initializes the CloudManager with the necessary images and data.
        /// </summary>
        private void CreateCloud()
        {
            GameObject cloud = Object.Instantiate(_cloudPrefab, CloudHolder.transform, false);
            cloud.name = "Cloud";

            // Prefab base size is 1km*1km
            float scale = SelectedDatasetScope.size.x / 1000.0f * LatDistortionValue;
            
            cloud.transform.localScale = new Vector3(scale, SelectedDatasetScope.size.x / 1000.0f, scale);

            CloudManager cloudManager = cloud.AddComponent<CloudManager>();

            List<Texture2D> images = ImageLoader.GetCloudImages(_mapName);

            cloudManager.Initialize(images, _heightImg, SelectedDatasetScope.size.x, Elevation);
            
            EditorUtility.SetDirty(cloudManager);
        }

        
        /// <summary>
        /// Deletes the previous cloud holder GameObject if it exists.
        /// </summary>
        private void DeletePreviousObject()
        {
            for (int i = Map.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = Map.transform.GetChild(i);
                if (child.name == HolderName)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}
