using System;
using System.Collections.Generic;
using Editor.NetCDF;
using UnityEngine;

namespace Editor.VisualizationSpawner.FullScaleSpawners
{

    public class BuildingSpawner : MapVisualizerSpawner
    {
        private readonly GameObject _buildingPrefab;

        private readonly List<BuildingData> _buildingDataList;

        
        public BuildingSpawner(string mapName, string cdfFilePath, GameObject map, float rotationAngle) 
            : base(cdfFilePath, map, rotationAngle)
        {
            _buildingDataList = BuildingDataLoader.GetBuildingData(mapName);
            
            //Sets the building prefab.
            GameObject buildingPrefab = Resources.Load<GameObject>("Prefabs/House");
            if (buildingPrefab == null)
            {
                throw new Exception("Cannot find a building model at: Resources/Prefabs/House");
            }
            _buildingPrefab = buildingPrefab;
        }
        
        
        public void SpawnAndSetupBuildings()
        {
            DeletePreviousObject("Buildings Holder");
            CreateAndSetupBuildingHolder();
            SpawnAllBuildings();
        }

        
        private void CreateAndSetupBuildingHolder()
        {
            throw new NotImplementedException();
        }

        
        private void SpawnAllBuildings()
        {
            
        }
    }
}