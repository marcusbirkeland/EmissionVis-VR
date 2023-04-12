using System;
using System.IO;
using Editor.NetCDF;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.VisualizationSpawner
{
    public class RadiationSpawner
    {
        private readonly AttributeDataGetter.FileAttributes _selectedCdfAttributes;
        private readonly Texture2D _shaderImage;
        private readonly string _radiationPrefabName;
        private readonly GameObject _map;
        private readonly float _rotationAngle;

        private GameObject _radiationHolder;
        
        
        public RadiationSpawner(Texture2D shaderImage, string attributesFilePath, string cdfFilePath, GameObject map, string radiationPrefabName, float rotationAngle)
        {
            _shaderImage = shaderImage;
            _selectedCdfAttributes = AttributeDataGetter.GetFileAttributes(attributesFilePath, cdfFilePath);
            
            MapRenderer mapRenderer = map.GetComponent<MapRenderer>();
            if (mapRenderer == null)
            {
                throw new Exception("The selected Map GameObject does not have a MapRenderer component. Please select a GameObject with the MapRenderer component.");
            }
            _map = map;
            _radiationPrefabName = radiationPrefabName;
            _rotationAngle = rotationAngle;
        }

        public void SpawnAndSetupRadiation()
        {
            DeletePreviousRadiation();
            
            CreateRadiationHolder();
            
            SpawnRadiation();
        }

        private void CreateRadiationHolder()
        {
            _radiationHolder = new GameObject("Radiation Holder");
            _radiationHolder.transform.SetParent(_map.transform, false);
            _radiationHolder.transform.localRotation = Quaternion.Euler(0, 90 + _rotationAngle, 0);
            
            MapUISetup.SetRadiationHolder(_radiationHolder);

            MapPin mapPin = _radiationHolder.AddComponent<MapPin>();

            mapPin.Location = _selectedCdfAttributes.position;
            mapPin.IsLayerSynchronized = true;
            mapPin.UseRealWorldScale = true;
            mapPin.ShowOutsideMapBounds = true;
            mapPin.Altitude = 100;
            mapPin.AltitudeReference = AltitudeReference.Surface;
        }

        private void SpawnRadiation()
        {
            GameObject cloudPrefab = Resources.Load<GameObject>($"Prefabs/{_radiationPrefabName}");
    
            if (cloudPrefab == null)
            {
                Debug.LogError($"Cloud prefab not found at 'Prefabs/{_radiationPrefabName}'");
                return;
            }

            GameObject rad = Object.Instantiate(cloudPrefab, _radiationHolder.transform, false);

            rad.name = "Radiation";

            LODGroup lodGroup = rad.GetComponent<LODGroup>();
            lodGroup.size = _selectedCdfAttributes.size.x;

            float scale = _selectedCdfAttributes.size.x / 1000.0f;
            rad.transform.localScale = new Vector3(scale, scale, scale);
        }

        private void DeletePreviousRadiation()
        {
            for (int i = _map.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = _map.transform.GetChild(i);
                if (child.name == "Radiation Holder")
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}