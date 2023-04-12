using System;
using Editor.EditorWindowComponents;
using Editor.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    //Main editor window application
    public class CreateScenesWindow : EditorWindow
    {
        private AllVariablesSelector _allVariablesSelector;
        
        private string _jsonFolderPath;


        [MenuItem("Sintef/Create Scenes")]
        private static void ShowWindow()
        {
            CreateScenesWindow window = GetWindow<CreateScenesWindow>("Generate scenes from netCDF data");
            window.minSize = new Vector2(500, 300);
        }


        private void OnEnable()
        {
            _jsonFolderPath = $"{Application.dataPath}/Resources/MapData";
            
            _allVariablesSelector = new AllVariablesSelector(_jsonFolderPath);
            _allVariablesSelector.OnDataComplete += CreateBothScenes;
        }

        
        private void OnGUI()
        {
            _allVariablesSelector.Draw();

            if (!_allVariablesSelector.DataRetrieved) return;
            
            _allVariablesSelector.LoadVariablesButton();
        }

        
        private void CreateBothScenes()
        {
            Debug.Log("Creating both scenes");

            CreateMiniatureScene(() => CreateFullScaleScene(null));
        }

        
        private void CreateMiniatureScene(Action onSceneCreated)
        {
            Debug.Log("Creating miniature scene");

            SceneAsset templateScene = GetTemplateScene("Miniature Template");
            SceneDuplicator.CreateAndLoadDuplicateScene(templateScene, _allVariablesSelector.MapName + " Miniature");
            
            MiniatureSceneBuilder miniBuilder = new(
                _allVariablesSelector.MapName, 
                _jsonFolderPath, 
                _allVariablesSelector.BuildingCdfPath,
                _allVariablesSelector.RadiationCdfPath,
                _allVariablesSelector.WindSpeedCdfPath);
            
            miniBuilder.CreateDataVisualization(onSceneCreated);
            
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        
        private void CreateFullScaleScene(Action onSceneCreated)
        {
            Debug.Log("Creating full scale scene");
            
            SceneAsset templateScene = GetTemplateScene("Full Scale Template");
            SceneDuplicator.CreateAndLoadDuplicateScene(templateScene, _allVariablesSelector.MapName + " Full Scale");

            FullScaleSceneBuilder fullScaleBuilder = new(
                _allVariablesSelector.MapName, 
                _jsonFolderPath, 
                _allVariablesSelector.BuildingCdfPath,
                _allVariablesSelector.RadiationCdfPath,
                _allVariablesSelector.WindSpeedCdfPath);
            
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