using System;
using Editor.NetCDF;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Editor.SceneManagement
 {
     public abstract class SceneBuilder
     {
         protected readonly string MapName;
         
         protected readonly string BuildingCdfPath;
         protected readonly string RadiationCdfPath;
         protected readonly string WindSpeedCdfPath;
         

         protected SceneBuilder(string mapName, string buildingCdfPath, string radiationCdfPath, string windSpeedCdfPath)
         {
             MapName = mapName;
             
             BuildingCdfPath = buildingCdfPath;
             RadiationCdfPath = radiationCdfPath;
             WindSpeedCdfPath = windSpeedCdfPath;
         }


         public void CreateDataVisualization(Action onDataCreated)
         {
             WaitForMapToLoad(() =>
             {
                 SetUpMap();
                 CreateBuildings();
                 CreateClouds();
                 CreateRadiation();
                 
                 EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                 onDataCreated?.Invoke();
             });
         }


         protected abstract void SetUpMap();

         
         protected abstract void CreateBuildings();


         protected abstract void CreateClouds();


         protected abstract void CreateRadiation();
         
         
         protected abstract void WaitForMapToLoad(Action onMapLoaded);
     }
 }