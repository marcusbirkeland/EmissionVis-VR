using System;
using Editor.NetCDF;
using Editor.NetCDF.Types;
using UnityEngine;
using Visualization;
using Object = UnityEngine.Object;

namespace Editor.Spawner.CloudSpawner
{
    public abstract class BaseCloudSpawner
    {
        protected const string HolderName = "Cloud Holder";

        protected readonly FileAttributes SelectedCdfAttributes;
        protected readonly GameObject Map;
        
        protected readonly float RotationAngle;
        protected readonly double Elevation;
        
        private readonly Texture2D _heightImg;
        private readonly string _mapName;
        private GameObject _cloud;
        private readonly GameObject _prefab;

        protected GameObject CloudHolder;

        
        /// <summary>
        /// The mercator projection gets more distorted the further away from the equator it is.
        /// This value accounts for that.
        /// </summary>
        protected virtual float LatDistortionValue => (float) (1 / Math.Cos(Math.PI * SelectedCdfAttributes.position.lat / 180.0));
        
        protected BaseCloudSpawner(string mapName, string cdfFilePath, GameObject map, string prefabName, float rotationAngle, double elevation)
        {
            SelectedCdfAttributes = AttributeDataGetter.GetFileAttributes(cdfFilePath);

            _prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");

            if (_prefab == null)
            {
                Debug.LogError($"Cloud prefab not found at 'Assets/Resources/Prefabs/{prefabName}'");
                return;
            }
            
            Map = map;
            RotationAngle = rotationAngle;
            _mapName = mapName;
            Elevation = elevation;
            _heightImg = ImageLoader.GetHeightMapImg(mapName);
        }
        
        
        public void SpawnAndSetupCloud()
        {
            DeletePreviousObject();

            CreateCloudHolder();

            CreateCloud();

            Debug.Log("Finished creating the cloud");
        }

        
        protected abstract void CreateCloudHolder();

        
        private void CreateCloud()
        {
            _cloud = Object.Instantiate(_prefab, CloudHolder.transform, false);
            _cloud.name = "Cloud";

            CloudManager cloudManager = _cloud.GetComponent<CloudManager>();
            cloudManager.heightMapImg = _heightImg;

            cloudManager.cloudImages = ImageLoader.GetCloudImages(_mapName);
            cloudManager.baseElevation = Elevation;

            LODGroup lodGroup = _cloud.GetComponent<LODGroup>();
            lodGroup.size = SelectedCdfAttributes.size.x;
            
            float scale = SelectedCdfAttributes.size.x / 1000.0f * LatDistortionValue;
            _cloud.transform.localScale = new Vector3(scale, scale, scale);
        }
        

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
