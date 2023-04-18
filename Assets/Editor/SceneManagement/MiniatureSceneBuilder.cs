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
            Debug.Log("Setting up map");
            
            GameObject mapObject = FindMap();

            MapRenderer map = mapObject.GetComponent<MapRenderer>();
            
            //NOTE: Using building cdf path as the position baseline, but any of the cdf files should work.
            AttributeDataGetter.FileAttributes baseCdfAttributes = AttributeDataGetter.GetFileAttributes(BuildingCdfPath);

            map.Center = Position.GetOffsetPosition(
                baseCdfAttributes.size.x/2, baseCdfAttributes.size.y/2, baseCdfAttributes.position);
        }


        protected override void CreateBuildings()
        {
            Debug.Log("Creating buildingSpawner instance");
            
            BuildingSpawner spawner = new(
                MapName,
                BuildingCdfPath, 
                FindMap(), 
                -3.1f);
             
            Debug.Log("Creating buildings");

            spawner.SpawnAndSetupBuildings();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
        
        
        protected override void CreateClouds()
        {
            Debug.Log("Creating clouds");

            const string cloudPrefab = "Cloud Miniature";
             
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
            Debug.Log("Creating radiation");

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
                    Debug.Log("Finished loading");
                    
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
            MapRenderer renderer = Object.FindObjectOfType<MapRenderer>();
            
            if (!renderer)
            {
                throw new Exception("The miniature scene is missing a mapRenderer component.");
            }

            return renderer.gameObject;
        }
    }
}