using System;
using Editor.NetCDF;
using Editor.NetCDF.Types;
using UnityEngine;
using Visualization;
using Object = UnityEngine.Object;

namespace Editor.Spawner.RadiationSpawner
{
    public abstract class BaseRadiationSpawner
    {
        protected const string HolderName = "Radiation Holder";

        protected readonly FileAttributes SelectedCdfAttributes;
        protected readonly GameObject Map;
        protected readonly float RotationAngle;
        private readonly Texture2D _radiationImage;
        private readonly Texture2D _heightMap;
        private readonly string _radiationPrefabName;

        protected GameObject RadiationHolder;


        /// <summary>
        /// The mercator projection gets more distorted the further away from the equator it is.
        /// This value accounts for that.
        /// </summary>
        protected virtual float LatDistortionValue => (float) (1 / Math.Cos(Math.PI * SelectedCdfAttributes.position.lat / 180.0));

        protected BaseRadiationSpawner(string mapName, string cdfFilePath, GameObject map, string radiationPrefabName, float rotationAngle)
        {
            SelectedCdfAttributes = AttributeDataGetter.GetFileAttributes(cdfFilePath);

            Map = map;
            RotationAngle = rotationAngle;

            _radiationImage = LoadFirstRadiationImage(mapName);
            _heightMap = ImageLoader.GetHeightMapImg(mapName);
            _radiationPrefabName = radiationPrefabName;
        }
        
        
        public void SpawnAndSetupRadiation()
        {
            DeletePreviousHolder();
            CreateRadiationHolder();
            
            SpawnRadiation();
            
            Debug.Log("Finished creating radiation");
        }

        
        protected abstract void CreateRadiationHolder();

        
        private void SpawnRadiation()
        {
            GameObject radiationPrefab = Resources.Load<GameObject>($"Prefabs/{_radiationPrefabName}");
    
            if (radiationPrefab == null)
            {
                Debug.LogError($"Cloud prefab not found at 'Prefabs/{_radiationPrefabName}'");
                return;
            }

            GameObject rad = Object.Instantiate(radiationPrefab, RadiationHolder.transform, false);
            rad.name = "Radiation";

            RadiationManager manager = rad.GetComponent<RadiationManager>();
            manager.radiationImage = _radiationImage;
            manager.heightMapImg = _heightMap;
            
            LODGroup lodGroup = rad.GetComponent<LODGroup>();
            lodGroup.size = SelectedCdfAttributes.size.x;

            //Prefab base size is 1km
            float scale = SelectedCdfAttributes.size.x / 1000.0f * LatDistortionValue;
            rad.transform.localScale = new Vector3(scale, scale, scale);
        }
        
        
        private void DeletePreviousHolder()
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
        

        //TODO: replace radiation display with ability to show all images.
        private static Texture2D LoadFirstRadiationImage(string mapName) => ImageLoader.GetRadiationImages(mapName)[0];
    }
}
