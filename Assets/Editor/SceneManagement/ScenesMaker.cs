using System;
using Editor.EditorWindowComponents;
using Editor.SceneBuilder;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.SceneManagement
{
    /// <summary>
    /// The ScenesMaker class provides functionality to create two types of scenes: Miniature and Full Scale scenes.
    /// These scenes are created based on template scenes and use the provided data from the AllVariablesSelector.
    /// </summary>
    public static class ScenesMaker
    {
        /// <summary>
        /// Creates both Miniature and Full Scale scenes.
        /// </summary>
        /// <param name="allVariablesSelector">The AllVariablesSelector object containing necessary data for scene creation.</param>
        public static void CreateBothScenes(AllVariablesSelector allVariablesSelector)
        {
            Debug.Log("Started scene creation");

            CreateMiniatureScene(() =>
            {
                CreateFullScaleScene(null, allVariablesSelector);
            }, allVariablesSelector);
        }

        /// <summary>
        /// Creates a Miniature scene based on the provided data in the AllVariablesSelector object.
        /// </summary>
        /// <param name="onSceneCreated">An optional callback to be invoked when the scene is created.</param>
        /// <param name="allVariablesSelector">The AllVariablesSelector object containing necessary data for scene creation.</param>
        private static void CreateMiniatureScene(Action onSceneCreated, AllVariablesSelector allVariablesSelector)
        {
            Debug.Log("Creating miniature scene");

            SceneAsset templateScene = GetTemplateScene("Miniature Template");
            SceneDuplicator.CreateAndLoadDuplicateScene(templateScene, allVariablesSelector.MapName + " Miniature");

            MiniatureSceneBuilder miniBuilder = new(
                allVariablesSelector.MapName,
                allVariablesSelector.BuildingCdfPath,
                allVariablesSelector.RadiationCdfPath,
                allVariablesSelector.WindSpeedCdfPath);

            miniBuilder.BuildScene(onSceneCreated);

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        /// <summary>
        /// Creates a Full Scale scene based on the provided data in the AllVariablesSelector object.
        /// </summary>
        /// <param name="onSceneCreated">An optional callback to be invoked when the scene is created.</param>
        /// <param name="allVariablesSelector">The AllVariablesSelector object containing necessary data for scene creation.</param>
        private static void CreateFullScaleScene(Action onSceneCreated, AllVariablesSelector allVariablesSelector)
        {
            Debug.Log("Creating full scale scene");

            SceneAsset templateScene = GetTemplateScene("Full Scale Template");
            SceneDuplicator.CreateAndLoadDuplicateScene(templateScene, allVariablesSelector.MapName + " Full Scale");

            FullScaleSceneBuilder fullScaleBuilder = new(
                allVariablesSelector.MapName,
                allVariablesSelector.BuildingCdfPath,
                allVariablesSelector.RadiationCdfPath,
                allVariablesSelector.WindSpeedCdfPath);

            fullScaleBuilder.BuildScene(onSceneCreated);

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        /// <summary>
        /// Retrieves the template scene asset based on the provided scene name.
        /// </summary>
        /// <param name="sceneName">The name of the template scene to be retrieved.</param>
        /// <returns>The SceneAsset object representing the template scene.</returns>
        /// <exception cref="Exception">Thrown when the template scene is not found.</exception>
        private static SceneAsset GetTemplateScene(string sceneName)
        {
            string scenePath = $"Assets/TemplateScenes/{sceneName}.unity";
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

            if (sceneAsset == null)
            {
                throw new Exception($"The template scene '{sceneName}' is missing");
            }

            return sceneAsset;
        }
    }
}