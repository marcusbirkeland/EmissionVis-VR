using System;
using Esri.ArcGISMapsSDK.Components;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Editor.SceneManagement
{
    public class FullScaleSceneBuilder
    {
        private readonly string _mapName;
        private readonly string _jsonFolderPath;
        
        private readonly string _buildingCdfPath;
        private readonly string _radiationCdfPath;
        private readonly string _windSpeedCdfPath;

        private readonly ArcGISMapComponent _arcGisMap;

        public FullScaleSceneBuilder(string mapName, string jsonFolderPath, string buildingCdfPath, string radiationCdfPath, string windSpeedCdfPath)
        {
            _mapName = mapName;
            _jsonFolderPath = jsonFolderPath;
            _buildingCdfPath = buildingCdfPath;
            
            _arcGisMap = Object.FindObjectOfType<ArcGISMapComponent>();

            if (_arcGisMap == null)
            {
                throw new Exception("The scene is missing an arcgis map component");
            }
        }

        public void CreateDataVisualization(Action onDataCreated)
        {
            WaitForMapToLoad(_arcGisMap, () =>
            {
                CreateUnityObjects();
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                onDataCreated?.Invoke();
            });
        }

        private void CreateUnityObjects()
        {
            CreateBuildings();
            CreateRadiation();
            CreateClouds();
        }

        private void CreateBuildings()
        {
            Debug.Log("Creating buildings");

        }

        private void CreateRadiation()
        {
            Debug.Log("Creating radiation");

        }

        private void CreateClouds()
        {
            Debug.Log("Creating clouds");

        }

        private static void WaitForMapToLoad(ArcGISMapComponent map, Action onMapLoaded)
        {
            EditorApplication.update += CheckMapLoaded;

            void CheckMapLoaded()
            {
                if (map.View.Map.Basemap.LoadStatus == Esri.GameEngine.ArcGISLoadStatus.Loaded)
                {
                    EditorApplication.update -= CheckMapLoaded;
                    onMapLoaded?.Invoke();
                    EditorUtility.ClearProgressBar();
                }
                else
                {
                    EditorUtility.DisplayProgressBar("Loading", "Waiting for map to load...", -1);
                }
            }
        }
    }

}