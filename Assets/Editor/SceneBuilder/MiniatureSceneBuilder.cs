using System;
using Editor.NetCDF;
using Editor.Spawner.BuildingSpawner;
using Editor.Spawner.CloudSpawner;
using Editor.Spawner.RadiationSpawner;
using Microsoft.Maps.Unity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.SceneBuilder
{
    /// <summary>
    /// The MiniatureSceneBuilder is a concrete implementation of the BaseSceneBuilder for dynamically creating miniature scale data visualizations.
    /// </summary>
    public class MiniatureSceneBuilder : BaseSceneBuilder
    {
        private readonly MapRenderer _map; 
        
        public MiniatureSceneBuilder(string mapName, string buildingCdfPath, string radiationCdfPath,
            string windSpeedCdfPath)
            : base(mapName, buildingCdfPath, radiationCdfPath, windSpeedCdfPath)
        {
            _map = FindMapComponent<MapRenderer>();
        }

        /// <summary>
        /// Sets up the map for the miniature scale scene.
        /// </summary>
        protected override void SetUpMap()
        {
            //NOTE: Using building cdf path as the position baseline, but any of the cdf files should work.
            _map.Center = AttributeDataGetter.GetCenterPosition(BuildingCdfPath);
        }

        /// <summary>
        /// Creates buildings in the miniature scale scene.
        /// </summary>
        protected override void CreateBuildings()
        {
            MiniatureBuildingSpawner spawner = new(
                MapName,
                BuildingCdfPath,
                _map.gameObject,
                -3.1f);

            spawner.SpawnAndSetupBuildings();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        /// <summary>
        /// Creates clouds in the miniature scale scene.
        /// </summary>
        protected override void CreateClouds()
        {
            const string cloudPrefab = "Cloud Miniature";

            MiniatureCloudSpawner spawner = new(
                MapName,
                WindSpeedCdfPath,
                _map.gameObject,
                cloudPrefab,
                -3.1f,
                -130);

            spawner.SpawnAndSetupCloud();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        /// <summary>
        /// Creates radiation in the miniature scale scene.
        /// </summary>
        protected override void CreateRadiation()
        {
            const string radiationPrefab = "Radiation";

            MiniatureRadiationSpawner spawner = new(
                MapName,
                RadiationCdfPath,
                _map.gameObject,
                radiationPrefab,
                -3.1f
            );

            spawner.SpawnAndSetupRadiation();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        /// <summary>
        /// Displays a progressbar while waiting for the map to load in the miniature scale scene.
        /// </summary>
        /// <param name="onMapLoaded">A callback to be executed once the map has finished loading</param>
        protected override void WaitForMapToLoad(Action onMapLoaded)
        {
            EditorApplication.update += CheckMapLoaded;

            void CheckMapLoaded()
            {
                if (_map.IsLoaded)
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
    }
}