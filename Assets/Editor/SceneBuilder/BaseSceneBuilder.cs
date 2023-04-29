using System;
using Editor.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Visualization;
using Object = UnityEngine.Object;

namespace Editor.SceneBuilder
{
    /// <summary>
    /// The BaseSceneBuilder is an abstract base class for creating data visualizations in different types of scenes.
    /// </summary>
    public abstract class BaseSceneBuilder
    {
        protected readonly string MapName;

        protected readonly string BuildingCdfPath;
        protected readonly string RadiationCdfPath;
        protected readonly string WindSpeedCdfPath;

        /// <summary>
        /// Initializes a new instance of the BaseSceneBuilder class.
        /// </summary>
        /// <param name="mapName">The name of the map.</param>
        /// <param name="buildingCdfPath">The path to the building NetCDF file.</param>
        /// <param name="radiationCdfPath">The path to the radiation NetCDF file.</param>
        /// <param name="windSpeedCdfPath">The path to the wind speed NetCDF file.</param>
        protected BaseSceneBuilder(string mapName, string buildingCdfPath, string radiationCdfPath, string windSpeedCdfPath)
        {
            MapName = mapName;
            MapUiManager.SetSceneNames(mapName);

            BuildingCdfPath = buildingCdfPath;
            RadiationCdfPath = radiationCdfPath;
            WindSpeedCdfPath = windSpeedCdfPath;
        }

        /// <summary>
        /// Main method for the class. Creates the data visualization and saves the scene.
        /// </summary>
        /// <param name="onDataCreated">A callback to be executed once the data visualization is complete.</param>
        public void CreateDataVisualization(Action onDataCreated)
        {
            SetUpMap();

            WaitForMapToLoad(() =>
            {
                CreateBuildings();
                CreateClouds();
                CreateRadiation();

                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                onDataCreated?.Invoke();
            });
        }

        /// <summary>
        /// Finds a map component of the specified type in the scene.
        /// </summary>
        /// <typeparam name="T">The type of the map component to find.</typeparam>
        /// <returns>The found map component or throws an exception if not found.</returns>
        protected static T FindMapComponent<T>() where T : Component
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
        /// Abstract method to create buildings.
        /// </summary>
        protected abstract void CreateBuildings();

        /// <summary>
        /// Abstract method to set up the cloud gameObject and its accompanying <see cref="CloudManager"/>.
        /// </summary>
        protected abstract void CreateClouds();

        /// <summary>
        /// Abstract method to create buildings in the scene.
        /// </summary>
        protected abstract void CreateRadiation();

        /// <summary>
        /// Waits for the map to load and executes the provided callback once loaded.
        /// </summary>
        /// <param name="onMapLoaded">A callback to be executed once the map is loaded.</param>
        protected abstract void WaitForMapToLoad(Action onMapLoaded);
    }
}
