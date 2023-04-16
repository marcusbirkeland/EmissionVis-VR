using System;
using Editor.NetCDF;
using Editor.VisualizationSpawner.MiniatureSpawners;
using Microsoft.Maps.Unity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Editor.SceneManagement
{
    public class MiniatureSceneBuilder : SceneBuilder
    {
        public MiniatureSceneBuilder(string mapName, string buildingCdfPath, string radiationCdfPath, string windSpeedCdfPath) 
            : base(mapName,  buildingCdfPath, radiationCdfPath, windSpeedCdfPath)
        {
        }
        
        
        protected override void SetUpMap()
        {
            GameObject mapObject = FindMap();

            MapRenderer map = mapObject.GetComponent<MapRenderer>();
            
            //NOTE: Using building cdf path as the position baseline, but any of the cdf files should work.
            AttributeDataGetter.FileAttributes baseCdfAttributes = AttributeDataGetter.GetFileAttributes(BuildingCdfPath);


            map.Center = AttributeDataGetter.Position.GetOffsetPosition(
                baseCdfAttributes.size.x/2, baseCdfAttributes.size.y/2, baseCdfAttributes.position);
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
            const string cloudPrefab = "1KM_CLOUD";
             
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


        //This method sometimes gets stuck for a while, and I can't figure out why.
        //Keep in mind, this part of the code needs to let the scene view run in the background and must not take
        //control of the main thread. Otherwise the map never loads.
        //TODO: fix
        protected override void WaitForMapToLoad(Action onMapLoaded)
        {
            EditorApplication.update += CheckMapLoaded;
             
            void CheckMapLoaded()
            {
                MapRenderer renderer = FindMap().GetComponent<MapRenderer>();

                if (renderer.IsLoaded)
                {
                    EditorApplication.update -= CheckMapLoaded;
                    onMapLoaded?.Invoke();
                    EditorUtility.ClearProgressBar();
                }
                else
                {
                    EditorUtility.DisplayProgressBar("Loading", "Waiting for map to load...", -1);
                }
            }
        }
        
        
        private GameObject FindMap()
        {
            MapRenderer renderer = Object.FindObjectOfType<MapRenderer>();
            
            if (!renderer)
            {
                throw new Exception("The miniature scene is missing a mapRenderer component.");
            }

            return renderer.gameObject;
        }
    }
}