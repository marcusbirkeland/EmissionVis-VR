using System.Collections;
using Editor.BuildingsSpawner;
using Editor.CloudsSpawner;
using Microsoft.Maps.Unity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.SceneManagement
 {
     public class MiniatureSceneBuilder
     {
         private readonly SceneAsset _templateScene;
         private readonly string _mapName;
         private readonly string _jsonFolderPath;
         private readonly string _cdfPath;


         public MiniatureSceneBuilder(SceneAsset templateScene, string mapName, string jsonFolderPath, string cdfPath)
         {
             _templateScene = templateScene;
             _mapName = mapName;
             _jsonFolderPath = jsonFolderPath;
             _cdfPath = cdfPath;
         }
         
         
         public void CreateMiniatureScene()
         {
             if (!SceneDuplicator.CreateAndLoadDuplicateScene(_templateScene, _mapName + " Miniature")) return;
             
             bool saveSuccess = EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

             if (saveSuccess)
             {
                 Debug.Log("Updated scene saved successfully.");
             }
             else
             {
                 Debug.LogError("Failed to save the updated scene.");
             }
         }

         
         public void CreateData(System.Action onDataCreated)
         {
             MapRenderer mapRenderer = Object.FindObjectOfType<MapRenderer>();

             if (mapRenderer == null)
             {
                 Debug.LogError("The scene is missing a mapRenderer component.");
                 return;
             }
             
             
             WaitForMapToLoad(mapRenderer, () =>
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
             MapRenderer mapRenderer = Object.FindObjectOfType<MapRenderer>();

             if (mapRenderer == null)
             {
                 Debug.LogError("The scene is missing a mapRenderer component.");
                 return;
             }
             
             GameObject houseModel = Resources.Load<GameObject>("Prefabs/House");

             if (houseModel == null)
             {
                 Debug.LogError("Invalid house model");
                 return;
             }

             BuildingSpawner spawner = new(
                 $"{_jsonFolderPath}/{_mapName}/", 
                 $"{_jsonFolderPath}/attributes.json", 
                 _cdfPath, 
                 mapRenderer.gameObject, 
                 houseModel, 
                 -3.1f);
             
             Debug.Log("Creating buildings");
             spawner.SpawnAllBuildings();
             Debug.Log("Finished creating buildings");
             EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
         }

         
         private void CreateRadiation()
         {
             Debug.Log("Creating radiation");
         }

         
         private void CreateClouds()
         {
             Debug.Log("Creating clouds");
             MapRenderer mapRenderer = Object.FindObjectOfType<MapRenderer>();

             if (mapRenderer == null)
             {
                 Debug.LogError("The scene is missing a mapRenderer component.");
                 return;
             }

             const string cloudPrefab = "1KM_CLOUD";
             const string cloudManagerPrefab = "CloudManager";

             CloudSpawner spawner = new(
                 $"{_jsonFolderPath}/{_mapName}/", 
                 $"{_jsonFolderPath}/attributes.json", 
                 _cdfPath, 
                 mapRenderer.gameObject, 
                 cloudPrefab,
                 cloudManagerPrefab,
                 -3.1f);

             spawner.SpawnAndSetupCloud();

             EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
         }
         
         
         //This method sometimes gets stuck for a while, and I can't figure out why.
         //Keep in mind, this part of the code needs to let the scene view run in the background and should not take
         //control of the main thread. Otherwise the map never loads.
         //TODO: fix
         private static void WaitForMapToLoad(MapRenderer mapRenderer, System.Action onMapLoaded)
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
     }
 }