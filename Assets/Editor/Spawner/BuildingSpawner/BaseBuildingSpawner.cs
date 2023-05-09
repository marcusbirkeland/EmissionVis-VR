using System;
using System.Collections.Generic;
using Editor.NetCDF;
using Editor.NetCDF.Types;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.Spawner.BuildingSpawner
{
    /// <summary>
    /// BaseBuildingSpawner is an abstract base class for spawning buildings in a scene.
    /// It provides common functionality to load building data, spawn buildings, and manage the buildings holder.
    /// </summary>
    public abstract class BaseBuildingSpawner
    {
        /// <summary>
        /// The name of the holder GameObject.
        /// </summary>
        protected const string HolderName = "Buildings Holder";

        
        /// <summary>
        /// The holder GameObject.
        /// </summary>
        protected GameObject BuildingsHolder;

        
        /// <summary>
        /// The scope of the dataset containing the building data.
        /// </summary>
        protected readonly DatasetScope SelectedDatasetScope;
        
        /// <summary>
        /// The GameObject that will contain the building holder.
        /// </summary>
        protected readonly GameObject Map;
        
        /// <summary>
        /// The prefab used for each house object.
        /// </summary>
        protected readonly GameObject BuildingPrefab;
        
        /// <summary>
        /// How many degrees to rotate the data.
        /// It rotates based on the origin position defined in <see cref="SelectedDatasetScope"/>
        /// </summary>
        protected readonly float RotationAngle;
        

        private readonly List<BuildingData> _buildingDataList;

        
        /// <summary>
        /// Initializes a new instance of the BaseBuildingSpawner class.
        /// </summary>
        /// <param name="mapName">The name of the map.</param>
        /// <param name="cdfFilePath">The path to the building NetCDF file.</param>
        /// <param name="map">The map GameObject.</param>
        /// <param name="rotationAngle">The rotation angle for the buildings.</param>
        protected BaseBuildingSpawner(string mapName, string cdfFilePath, GameObject map, float rotationAngle)
        {
            SelectedDatasetScope = ScopeDataGetter.GetDatasetScope(cdfFilePath);

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

        
        /// <summary>
        /// Main method. Spawns and sets up buildings in the scene.
        /// </summary>
        public void SpawnAndSetupBuildings()
        {
            DeletePreviousObject();
            CreateAndSetupBuildingHolder();
            SpawnAllBuildings();
        }

        
        /// <summary>
        /// Abstract method to create and set up the building holder GameObject.
        /// </summary>
        protected abstract void CreateAndSetupBuildingHolder();
        
        
        /// <summary>
        /// Abstract method to spawn an individual building based on the provided <see cref="BuildingData"/>.
        /// </summary>
        /// <param name="buildingData">The building data for the building to be spawned.</param>
        protected abstract void SpawnBuilding(BuildingData buildingData);


        /// <summary>
        /// Iterates through the <see cref="_buildingDataList"/> and spawns a building for each
        /// <see cref="BuildingData"/> instance.
        /// </summary>
        private void SpawnAllBuildings()
        {
            for (int i = 0; i < _buildingDataList.Count; i++)
            {
                string progressString = $"Parsing building data ({i}/{_buildingDataList.Count})";
                float progress = (float) i / _buildingDataList.Count;

                if (EditorUtility.DisplayCancelableProgressBar("Creating buildings from data", progressString,
                        progress))
                {
                    Debug.Log("Cancelled building spawning");
                    break;
                }

                SpawnBuilding(_buildingDataList[i]);
            }

            EditorUtility.ClearProgressBar();

            Debug.Log($"Spawned {BuildingsHolder.transform.childCount} buildings.");
        }
        
        
        /// <summary>
        /// Deletes the previous building visualization holder object if it exists.
        /// </summary>
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
