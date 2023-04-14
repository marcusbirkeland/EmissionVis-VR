using System;
using System.IO;
using Editor.VisualizationSpawner;
using Microsoft.Maps.Unity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Editor.SceneManagement
 {
     public class MiniatureSceneBuilder
     {
         private readonly string _mapName;
         private readonly string _jsonFolderPath;
         
         private readonly string _buildingCdfPath;
         private readonly string _radiationCdfPath;
         private readonly string _windSpeedCdfPath;

         private readonly MapRenderer _renderer;


         public MiniatureSceneBuilder(string mapName, string jsonFolderPath, string buildingCdfPath, string radiationCdfPath, string windSpeedCdfPath)
         {
             _mapName = mapName;
             _jsonFolderPath = jsonFolderPath;
             
             _buildingCdfPath = buildingCdfPath;
             _radiationCdfPath = radiationCdfPath;
             _windSpeedCdfPath = windSpeedCdfPath;

             _renderer = Object.FindObjectOfType<MapRenderer>();

             if (_renderer == null)
             {
                 throw new Exception("The scene is missing a mapRenderer component");
             }
         }


         public void CreateDataVisualization(Action onDataCreated)
         {
             WaitForMapToLoad(_renderer, () =>
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

             GameObject houseModel = Resources.Load<GameObject>("Prefabs/House");

             if (houseModel == null)
             {
                 Debug.LogError("Invalid house model");
                 return;
             }

             BuildingSpawner spawner = new(
                 $"{_jsonFolderPath}/{_mapName}/", 
                 $"{_jsonFolderPath}/attributes.json", 
                 _buildingCdfPath, 
                 _renderer.gameObject, 
                 houseModel, 
                 -3.1f);
             
             spawner.SpawnAllBuildings();
             Debug.Log("Finished creating buildings");
             EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
         }

         
         private void CreateRadiation()
         {
             Debug.Log("Creating radiation");
             
             Texture2D img = LoadFirstRadiationImage($"{_jsonFolderPath}/{_mapName}/Radiation/");
             const string radiationPrefab = "Radiation";

             RadiationSpawner spawner = new(
                    img,
                    $"{_jsonFolderPath}/attributes.json",
                    _radiationCdfPath,
                    _renderer.gameObject,
                    radiationPrefab,
                    -3.1f
             );
             
             spawner.SpawnAndSetupRadiation();

             EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
         }

         
         private void CreateClouds()
         {
             Debug.Log("Creating clouds");

             const string cloudPrefab = "1KM_CLOUD";
             const string cloudManagerPrefab = "CloudManager";

             CloudSpawner spawner = new(
                 $"{_jsonFolderPath}/{_mapName}/", 
                 $"{_jsonFolderPath}/attributes.json", 
                 _windSpeedCdfPath, 
                 _renderer.gameObject, 
                 cloudPrefab,
                 cloudManagerPrefab,
                 -3.1f);

             spawner.SpawnAndSetupCloud();

             EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
         }
         
         
         //This method sometimes gets stuck for a while, and I can't figure out why.
         //Keep in mind, this part of the code needs to let the scene view run in the background and must not take
         //control of the main thread. Otherwise the map never loads.
         //TODO: fix
         private static void WaitForMapToLoad(MapRenderer mapRenderer, Action onMapLoaded)
         {
             EditorApplication.update += CheckMapLoaded;
             
             void CheckMapLoaded()
             {
                 if (mapRenderer.IsLoaded)
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
         
         
         //TODO: replace radiation display with ability to show all images.
         private static Texture2D LoadFirstRadiationImage(string folderPath)
         {
             string[] subfolders = Directory.GetDirectories(Path.Combine("Assets", "Resources", folderPath));
             if (subfolders.Length == 0)
             {
                 Debug.LogError("No subfolders found.");
                 return null;
             }

             string firstSubfolder = subfolders[0];
             string[] pngFiles = Directory.GetFiles(firstSubfolder, "*.png");
             if (pngFiles.Length == 0)
             {
                 Debug.LogError("No .png files found in the first subfolder.");
                 return null;
             }

             string firstPngFile = pngFiles[0];
             string resourcePath = firstPngFile.Replace(Path.Combine("Assets", "Resources") + Path.DirectorySeparatorChar, "").Replace(".png", "");
             resourcePath = resourcePath.Replace(Path.DirectorySeparatorChar, '/');
             Texture2D texture = Resources.Load<Texture2D>(resourcePath);

             return texture;
         }
     }
 }