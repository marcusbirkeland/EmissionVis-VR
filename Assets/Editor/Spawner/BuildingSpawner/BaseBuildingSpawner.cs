using System;
using System.Collections.Generic;
using Editor.NetCDF;
using Editor.NetCDF.Types;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.Spawner.BuildingSpawner
{
    public abstract class BaseBuildingSpawner
    {
        protected const string HolderName = "Buildings Holder";
        
        protected readonly FileAttributes SelectedCdfAttributes;
        protected readonly GameObject Map;
        protected readonly GameObject BuildingPrefab;
        protected readonly float RotationAngle;
        protected readonly float UnityUnitsPerMeter;
        
        protected GameObject VisualizationHolder;

        private readonly List<BuildingData> _buildingDataList;

        protected BaseBuildingSpawner(string mapName, string cdfFilePath, GameObject map, float rotationAngle)
        {
            SelectedCdfAttributes = AttributeDataGetter.GetFileAttributes(cdfFilePath);
            UnityUnitsPerMeter = (float)(1 / Math.Cos(Math.PI * SelectedCdfAttributes.position.lat / 180.0));

            Map = map;
            RotationAngle = rotationAngle;

            _buildingDataList = BuildingDataLoader.GetBuildingData(mapName);

            GameObject buildingPrefab = Resources.Load<GameObject>("Prefabs/House");
            if (buildingPrefab == null)
            {
                throw new Exception("Cannot find a building model at: Resources/Prefabs/House");
            }
            BuildingPrefab = buildingPrefab;
        }
        
        
        public void SpawnAndSetupBuildings()
        {
            DeletePreviousObject();
            CreateAndSetupBuildingHolder();
            SpawnAllBuildings();
        }

        
        protected abstract void CreateAndSetupBuildingHolder();

        
        private void SpawnAllBuildings()
        {
            for (int i = 0; i < _buildingDataList.Count; i++)
            {
                string progressString = $"Parsing building data ({i}/{_buildingDataList.Count})";
                float progress = (float)i / _buildingDataList.Count;

                if (EditorUtility.DisplayCancelableProgressBar("Creating buildings from data", progressString, progress))
                {
                    Debug.Log("Cancelled building spawning");
                    break;
                }

                SpawnBuilding(_buildingDataList[i]);
            }

            EditorUtility.ClearProgressBar();

            Debug.Log($"Spawned {VisualizationHolder.transform.childCount} buildings.");
        }

        
        protected abstract void SpawnBuilding(BuildingData buildingData);
        
        
        private void DeletePreviousObject()
        {
            for (int i = Map.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = Map.transform.GetChild(i);
                if (child.name == HolderName)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}
