using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.VisualizationSpawner
{
    public class RadiationSpawner : MapVisualizerSpawner
    {
        private readonly Texture2D _shaderImage;
        private readonly string _radiationPrefabName;
        
        
        public RadiationSpawner(Texture2D shaderImage, string attributesFilePath, string cdfFilePath, GameObject map, string radiationPrefabName, float rotationAngle) 
            : base(attributesFilePath, cdfFilePath, map, rotationAngle)
        {
            _shaderImage = shaderImage;
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
            VisualizerHolder = new GameObject("Radiation Holder");
            VisualizerHolder.transform.SetParent(Map.transform, false);
            VisualizerHolder.transform.localRotation = Quaternion.Euler(0, 90 + RotationAngle, 0);
            
            MapUISetup.SetRadiationHolder(VisualizerHolder);

            MapPin mapPin = VisualizerHolder.AddComponent<MapPin>();

            mapPin.Location = SelectedCdfAttributes.position;
            mapPin.IsLayerSynchronized = true;
            mapPin.UseRealWorldScale = true;
            mapPin.ShowOutsideMapBounds = true;
            mapPin.Altitude = 100;
            mapPin.AltitudeReference = AltitudeReference.Surface;
        }

        
        //TODO: Implement setting the image/images correctly
        private void SpawnRadiation()
        {
            GameObject cloudPrefab = Resources.Load<GameObject>($"Prefabs/{_radiationPrefabName}");
    
            if (cloudPrefab == null)
            {
                Debug.LogError($"Cloud prefab not found at 'Prefabs/{_radiationPrefabName}'");
                return;
            }

            GameObject rad = Object.Instantiate(cloudPrefab, VisualizerHolder.transform, false);

            rad.name = "Radiation";

            LODGroup lodGroup = rad.GetComponent<LODGroup>();
            lodGroup.size = SelectedCdfAttributes.size.x;

            float scale = SelectedCdfAttributes.size.x / 1000.0f;
            rad.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}