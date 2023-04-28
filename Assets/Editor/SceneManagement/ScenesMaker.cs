using System;
using Editor.EditorWindowComponents;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.SceneManagement
{
    public static class ScenesMaker
    {
        public static void CreateBothScenes(AllVariablesSelector allVariablesSelector)
        {
            Debug.Log("Started scene creation");

            CreateMiniatureScene(() =>
            {
                CreateFullScaleScene(null, allVariablesSelector);
            }, allVariablesSelector);
        }


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

            miniBuilder.CreateDataVisualization(onSceneCreated);

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        
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

            fullScaleBuilder.CreateDataVisualization(onSceneCreated);

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        
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