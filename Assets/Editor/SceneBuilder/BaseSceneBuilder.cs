using System;
using Editor.SceneManagement;
using Editor.Spawner.BuildingSpawner;
using Editor.Spawner.CloudSpawner;
using Editor.Spawner.RadiationSpawner;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Editor.SceneBuilder
{
    /// <summary>
    /// The BaseSceneBuilder is an abstract base class for creating data visualizations in different types of scenes.
    /// This class is a generic class, which allows it to work with different map components by specifying the type of
    /// map component (T) when creating a derived class. This provides flexibility and extensibility when working with
    /// various map components while still reusing the common functionality provided by the BaseSceneBuilder class.
    /// </summary>
    /// <typeparam name="T">The type of the map component that will be used in the derived class. T must be a subclass of Component.</typeparam>
    public abstract class BaseSceneBuilder<T> where T : Component
    {
        private readonly string _mapName;

        protected readonly string BuildingCdfPath;
        protected readonly string RadiationCdfPath;
        protected readonly string WindSpeedCdfPath;

        protected T Map;

        /// <summary>
        /// Initializes a new instance of the BaseSceneBuilder class.
        /// </summary>
        /// <param name="mapName">The name of the map.</param>
        /// <param name="buildingCdfPath">The path to the building NetCDF file.</param>
        /// <param name="radiationCdfPath">The path to the radiation NetCDF file.</param>
        /// <param name="windSpeedCdfPath">The path to the wind speed NetCDF file.</param>
        protected BaseSceneBuilder(string mapName, string buildingCdfPath, string radiationCdfPath, string windSpeedCdfPath)
        {
            _mapName = mapName;
            MapUiManager.SetSceneNames(mapName);

            BuildingCdfPath = buildingCdfPath;
            RadiationCdfPath = radiationCdfPath;
            WindSpeedCdfPath = windSpeedCdfPath;
        }

        /// <summary>
        /// Main method for the class. Creates the data visualization and saves the scene.
        /// </summary>
        /// <param name="onSceneBuilt">A callback to be executed once the data visualization is complete.</param>
        public void BuildScene(Action onSceneBuilt)
        {
            SetUpMap();

            WaitForMapToLoad(() =>
            {
                CreateDataObjects();
                
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                onSceneBuilt?.Invoke();
            });
        }

        protected abstract void CreateDataObjects();
        

        /// <summary>
        /// Finds a map component of the specified type in the scene.
        /// </summary>
        /// <typeparam name="T">The type of the map component to find.</typeparam>
        /// <returns>The found map component or throws an exception if not found.</returns>
        protected T FindMapComponent()
        {
            T mapComponent = Object.FindObjectOfType<T>();

            if (!mapComponent)
            {
                throw new Exception($"The scene is missing a {typeof(T).Name} component.");
            }

            return mapComponent;
        }

        /// <summary>
        /// Abstract method to set up the map component in the scene.
        /// </summary>
        protected abstract void SetUpMap();

        /// <summary>
        /// Creates buildings in the full scale scene.
        /// </summary>
        protected void CreateBuildings<TSpawner>() where TSpawner : BaseBuildingSpawner
        {
            TSpawner spawner = (TSpawner)Activator.CreateInstance(typeof(TSpawner),
                _mapName,
                BuildingCdfPath, 
                Map.gameObject,
                -3.1f);
             
            spawner.SpawnAndSetupBuildings();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        
        /// <summary>
        /// Creates clouds in the full scale scene.
        /// </summary>
        protected void CreateClouds<TSpawner>() where TSpawner : BaseCloudSpawner
        {
            TSpawner spawner = (TSpawner)Activator.CreateInstance(typeof(TSpawner),
                _mapName, 
                WindSpeedCdfPath, 
                Map.gameObject,
                -3.1f
                );

            spawner.SpawnAndSetupCloud();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
         
        
        /// <summary>
        /// Creates radiation in the full scale scene.
        /// </summary>
        protected void CreateRadiation<TSpawner>() where TSpawner : BaseRadiationSpawner
        {
            TSpawner spawner = (TSpawner)Activator.CreateInstance(typeof(TSpawner),
                _mapName,
                RadiationCdfPath,
                Map.gameObject,
                -3.1f
            );
             
            spawner.SpawnAndSetupRadiation();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        /// <summary>
        /// Waits for the map to load and executes the provided callback once loaded.
        /// </summary>
        /// <param name="onMapLoaded">A callback to be executed once the map is loaded.</param>
        protected abstract void WaitForMapToLoad(Action onMapLoaded);
    }
}
