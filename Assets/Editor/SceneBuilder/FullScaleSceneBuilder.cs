using System;
using Editor.NetCDF;
using Editor.Spawner.BuildingSpawner;
using Editor.Spawner.CloudSpawner;
using Editor.Spawner.RadiationSpawner;
using Esri.ArcGISMapsSDK.Components;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Editor.SceneBuilder
{
    /// <summary>
    /// The FullScaleSceneBuilder is a concrete implementation of the BaseSceneBuilder for dynamically creating full scale data visualizations.
    /// </summary>
    public class FullScaleSceneBuilder : BaseSceneBuilder
    {
        private readonly ArcGISMapComponent _map;
        
        public FullScaleSceneBuilder(string mapName, string buildingCdfPath, string radiationCdfPath, string windSpeedCdfPath) 
            : base(mapName, buildingCdfPath, radiationCdfPath, windSpeedCdfPath)
        {
            _map = FindMapComponent<ArcGISMapComponent>();
        }

        
        /// <summary>
        /// Sets up the map for the full scale scene.
        /// </summary>
        protected override void SetUpMap()
        {
            //NOTE: Using building cdf path as the position baseline, but any of the cdf files should work.
            _map.OriginPosition = AttributeDataGetter.GetCenterPosition(BuildingCdfPath);
            
            _map.MeshCollidersEnabled = true;
        }

        
        /// <summary>
        /// Creates buildings in the full scale scene.
        /// </summary>
        protected override void CreateBuildings()
        {
            FullScaleBuildingSpawner spawner = new(
                MapName,
                BuildingCdfPath, 
                _map.gameObject,
                -3.1f);
             
            spawner.SpawnAndSetupBuildings();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        
        /// <summary>
        /// Creates clouds in the full scale scene.
        /// </summary>
        protected override void CreateClouds()
        {
            const string cloudPrefab = "Cloud Full Scale";

            FullScaleCloudSpawner spawner = new(
                MapName, 
                WindSpeedCdfPath, 
                _map.gameObject,
                cloudPrefab,
                -3.1f,
                130);

            spawner.SpawnAndSetupCloud();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
         
        
        /// <summary>
        /// Creates radiation in the full scale scene.
        /// </summary>
        protected override void CreateRadiation()
        {
            const string radiationPrefab = "Radiation";

            FullScaleRadiationSpawner spawner = new(
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
        /// Displays a progressbar while waiting for the map to load.
        /// </summary>
        /// <param name="onMapLoaded">A callback to be executed once the map has finished loading</param>
        protected override void WaitForMapToLoad(Action onMapLoaded)
        {
            EditorApplication.update += CheckMapLoaded;

            void CheckMapLoaded()
            {
                if (_map.View.Map.Basemap.LoadStatus == Esri.GameEngine.ArcGISLoadStatus.Loaded)
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
    }
}