using System;
using Editor.NetCDF;
using Editor.VisualizationSpawner.FullScaleSpawners;
using Esri.ArcGISMapsSDK.Components;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Editor.SceneManagement
{
    public class FullScaleSceneBuilder : SceneBuilder
    {
        public FullScaleSceneBuilder(string mapName, string buildingCdfPath, string radiationCdfPath, string windSpeedCdfPath) 
            : base(mapName, buildingCdfPath, radiationCdfPath, windSpeedCdfPath)
        {
        }

        protected override void SetUpMap()
        {
            GameObject mapObject = FindMap();
            ArcGISMapComponent map = mapObject.GetComponent<ArcGISMapComponent>();
            
            //NOTE: Using building cdf path as the position baseline, but any of the cdf files should work.
            AttributeDataGetter.FileAttributes baseCdfAttributes = AttributeDataGetter.GetFileAttributes(BuildingCdfPath);
            
            Position newPos = Position.GetOffsetPosition(baseCdfAttributes.size.x/2, baseCdfAttributes.size.y/2, baseCdfAttributes.position);
            
            map.OriginPosition = newPos;
            map.MeshCollidersEnabled = true;
        }

        protected override void CreateBuildings()
        {
            BuildingSpawner spawner = new(
                MapName,
                BuildingCdfPath, 
                FindMap(), 
                -3.1f);
             
            spawner.SpawnAndSetupBuildings();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

         
        protected override void CreateClouds()
        {
            const string cloudPrefab = "Cloud Full Scale";
             
            CloudSpawner spawner = new(
                MapName, 
                WindSpeedCdfPath, 
                FindMap(), 
                cloudPrefab,
                -3.1f);

            spawner.SpawnAndSetupCloud();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
         
         
        protected override void CreateRadiation()
        {
            const string radiationPrefab = "Radiation";
             
            RadiationSpawner spawner = new(
                MapName,
                RadiationCdfPath,
                FindMap(), 
                radiationPrefab,
                -3.1f
            );
             
            spawner.SpawnAndSetupRadiation();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }


        protected override void WaitForMapToLoad(Action onMapLoaded)
        {
            EditorApplication.update += CheckMapLoaded;

            void CheckMapLoaded()
            {
                ArcGISMapComponent map = FindMap().GetComponent<ArcGISMapComponent>();

                if (map.View.Map.Basemap.LoadStatus == Esri.GameEngine.ArcGISLoadStatus.Loaded)
                {
                    EditorApplication.update -= CheckMapLoaded;
                    EditorUtility.ClearProgressBar();
                    onMapLoaded?.Invoke();
                }
                else
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Loading", "Waiting for map to load...", -1))
                    {
                        EditorUtility.ClearProgressBar();
                        EditorApplication.update -= CheckMapLoaded;
                    }
                }
            }
        }

        
        
        private static GameObject FindMap()
        {
            ArcGISMapComponent map = Object.FindObjectOfType<ArcGISMapComponent>();
            
            if (!map)
            {
                throw new Exception("The miniature scene is missing a mapRenderer component.");
            }

            return map.gameObject;
        }
    }
}