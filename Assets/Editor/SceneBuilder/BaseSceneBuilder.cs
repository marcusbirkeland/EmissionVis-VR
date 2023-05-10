using System;
using Editor.EditorWindowComponents;
using Editor.NetCDF.Types;
using Editor.Spawner.BuildingSpawner;
using Editor.Spawner.CloudSpawner;
using Editor.Spawner.RadiationSpawner;
using Editor.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Editor.SceneBuilder
{
    /// <summary>
    /// The BaseSceneBuilder is an abstract base class for creating data visualizations in different types of scenes.
    /// This class is a generic class, which allows it to work with different map components by specifying the type of
    /// map component (T) when creating a derived class.
    /// </summary>
    /// <typeparam name="T">The type of the map component that will be used in the derived class. T must be a subclass of <see cref="Component"/>.</typeparam>
    public abstract class BaseSceneBuilder<T> : ISceneBuilder where T : Component
    {
        /// <summary>
        /// The dataset containing all the user selected values.
        /// </summary>
        protected NcDataset NcData;
        
        /// <summary>
        /// The map component used when adding data to the scene.
        /// </summary>
        protected T Map;
        
        
        /// <summary>
        /// Abstract value representing the scene name.
        /// </summary>
        protected abstract string SceneType { get; }
        
        
        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="ncData"></param>
        protected BaseSceneBuilder(NcDataset ncData)
        {
            NcData = ncData;
        }

        /// <summary>
        /// Main method for scene generation. Creates the scene and adds data visualization.
        /// </summary>
        /// <param name="onSceneBuilt">A callback to be executed once the data visualization is complete.</param>
        public void BuildScene(Action onSceneBuilt = null)
        {
            Debug.Log($"Creating {SceneType} scene");

            SceneAsset templateScene = GetTemplateScene($"{SceneType} Template");
            if (!SceneDuplicator.CreateAndLoadDuplicateScene(templateScene, NcData.MapName + $" {SceneType}")) return;
            
            Map = FindMapComponent();
            
            MapUiManager.SetSceneNames(NcData.MapName);

            SetUpMap();

            WaitForMapToLoad(() =>
            {
                CreateDataObjects();
                
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                onSceneBuilt?.Invoke();
            });
        }
        

        /// <summary>
        /// Creates buildings in the scene using the specified building spawner type.
        /// The method uses generics to allow for different building spawner implementations.
        /// </summary>
        /// <typeparam name="TSpawner">
        /// The type of the building spawner to use. TSpawner must be a subclass of <see cref="BaseBuildingSpawner"/>.
        /// </typeparam>
        /// <remarks>
        /// At the moment, the rotation angle is hardcoded to what worked best for our dataset.
        /// In the future this should be replaced with a used inputted value,
        /// and an advanced tab on the <see cref="CreateScenesWindow"/>.
        /// </remarks>
        protected void CreateBuildings<TSpawner>() where TSpawner : BaseBuildingSpawner
        {
            TSpawner spawner = (TSpawner)Activator.CreateInstance(typeof(TSpawner),
                NcData.MapName,
                NcData.BuildingCdfPath, 
                Map.gameObject,
                -3.1f);
             
            spawner.SpawnAndSetupBuildings();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        
        /// <summary>
        /// Creates clouds in the scene using the specified cloud spawner type.
        /// The method uses generics to allow for different cloud spawner implementations.
        /// </summary>
        /// <typeparam name="TSpawner">
        /// The type of the cloud spawner to use. TSpawner must be a subclass of <see cref="BaseCloudSpawner"/>.
        /// </typeparam>
        /// <remarks>
        /// At the moment, the rotation angle is hardcoded to what worked best for our dataset.
        /// In the future this should be replaced with a used inputted value,
        /// and an advanced tab on the <see cref="CreateScenesWindow"/>.
        /// </remarks>

        protected void CreateClouds<TSpawner>() where TSpawner : BaseCloudSpawner
        {
            TSpawner spawner = (TSpawner)Activator.CreateInstance(typeof(TSpawner),
                NcData.MapName, 
                NcData.WindSpeedCdfPath, 
                Map.gameObject,
                -3.1f);

            spawner.SpawnAndSetupCloud();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
         
        
        /// <summary>
        /// Creates radiation visualizations in the scene using the specified radiation spawner type.
        /// The method uses generics to allow for different radiation spawner implementations.
        /// </summary>
        /// <typeparam name="TSpawner">
        /// The type of the radiation spawner to use. TSpawner must be a subclass of <see cref="BaseRadiationSpawner"/>.
        /// </typeparam>
        /// <remarks>
        /// At the moment, the rotation angle is hardcoded to what worked best for our dataset.
        /// In the future this should be replaced with a used inputted value,
        /// and an advanced tab on the <see cref="CreateScenesWindow"/>.
        /// </remarks>

        protected void CreateRadiation<TSpawner>() where TSpawner : BaseRadiationSpawner
        {
            TSpawner spawner = (TSpawner)Activator.CreateInstance(typeof(TSpawner),
                NcData.MapName,
                NcData.RadiationCdfPath,
                Map.gameObject,
                -3.1f);
             
            spawner.SpawnAndSetupRadiation();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
        
        
        /// <summary>
        /// Retrieves the template scene asset based on the provided scene name.
        /// </summary>
        /// <param name="sceneName">The name of the template scene to be retrieved.</param>
        /// <returns>The SceneAsset object representing the template scene.</returns>
        /// <exception cref="Exception">Thrown when the template scene is not found.</exception>
        protected static SceneAsset GetTemplateScene(string sceneName)
        {
            string scenePath = $"Assets/TemplateScenes/{sceneName}.unity";
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

            if (sceneAsset == null)
            {
                throw new Exception($"The template scene '{sceneName}' is missing");
            }

            return sceneAsset;
        }
        
        
        /// <summary>
        /// Abstract method used to create all the data visualization in the current scene.
        /// </summary>
        protected abstract void CreateDataObjects();
        

        /// <summary>
        /// Abstract method to set up the map component in the scene.
        /// </summary>
        protected abstract void SetUpMap();

        
        /// <summary>
        /// Waits for the map to load and executes the provided callback once loaded.
        /// </summary>
        /// <param name="onMapLoaded">A callback to be executed once the map is loaded.</param>
        protected abstract void WaitForMapToLoad(Action onMapLoaded);
        
        
        /// <summary>
        /// Finds a map component of the specified type in the scene.
        /// </summary>
        /// <typeparam name="T">The type of the map component to find.</typeparam>
        /// <returns>The found map component or throws an exception if not found.</returns>
        private static T FindMapComponent()
        {
            T mapComponent = Object.FindObjectOfType<T>();

            if (!mapComponent)
            {
                throw new Exception($"The scene {SceneManager.GetActiveScene().name} is missing a {typeof(T).Name} component.");
            }

            return mapComponent;
        }
    }
}
