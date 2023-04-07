using System;
using Editor.EditorWindowComponents;
using Editor.NetCDF;
using Editor.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    //Main editor window application
    public class CreateScenesWindow : EditorWindow
    {
        private VariablesSelector _variablesSelector;

        private SceneAsset _selectedMiniatureScene;
        private SceneAsset _selectedFullScaleScene;

        private MiniatureSceneBuilder _miniBuilder;
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
            
            _variablesSelector = new VariablesSelector(_jsonFolderPath);
            _variablesSelector.OnDataComplete += CreateBothScenes;
        }

        
        private void OnGUI()
        {
            _variablesSelector.Draw();

            if (!_variablesSelector.DataRetrieved) return;
            
            _selectedMiniatureScene = (SceneAsset)EditorGUILayout.ObjectField("Select miniature scene", _selectedMiniatureScene, typeof(SceneAsset), false);
            _selectedFullScaleScene = (SceneAsset)EditorGUILayout.ObjectField("Select full scale scene", _selectedFullScaleScene, typeof(SceneAsset), false);

            _variablesSelector.LoadVariablesButton();
        }

        
        private void CreateBothScenes()
        {
            Debug.Log("Creating both scenes");

            CreateMiniatureScene(CreateFullScaleScene);
        }

        
        private void CreateMiniatureScene(Action onSceneCreated)
        {
            Debug.Log("Creating miniature scene");
            _miniBuilder = new MiniatureSceneBuilder(_selectedMiniatureScene, _variablesSelector.MapName, _jsonFolderPath, _variablesSelector.BuildingCdfPath);
            _miniBuilder.CreateMiniatureScene();
            
            _miniBuilder.CreateData(onSceneCreated);
        }

        
        private void CreateFullScaleScene()
        {
            Debug.Log("Creating full scale scene");
        }
    }
}