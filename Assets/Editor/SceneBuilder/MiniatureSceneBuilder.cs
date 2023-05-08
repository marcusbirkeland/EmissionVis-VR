using System;
using Editor.NetCDF;
using Editor.NetCDF.Types;
using Editor.Spawner.BuildingSpawner;
using Editor.Spawner.CloudSpawner;
using Editor.Spawner.RadiationSpawner;
using Microsoft.Maps.Unity;
using UnityEditor;
using UnityEngine;

namespace Editor.SceneBuilder
{
    /// <summary>
    /// The MiniatureSceneBuilder is a concrete implementation of the BaseSceneBuilder for dynamically creating miniature scale data visualizations.
    /// </summary>
    public class MiniatureSceneBuilder : BaseSceneBuilder<MapRenderer>
    {
        public MiniatureSceneBuilder(NcDataset ncData) : base(ncData)
        {
        }
        
        
        protected override void CreateDataObjects()
        {
            CreateBuildings<MiniatureBuildingSpawner>();
            CreateClouds<MiniatureCloudSpawner>();
            CreateRadiation<MiniatureRadiationSpawner>();
        }

        /// <summary>
        /// Sets up the map for the miniature scale scene.
        /// </summary>
        protected override void SetUpMap()
        {
            //NOTE: Using building cdf path as the position baseline, but any of the cdf files should work.
            Map.Center = AttributeDataGetter.GetCenterPosition(NcData.BuildingCdfPath);
        }
        

        /// <summary>
        /// Displays a progressbar while waiting for the map to load in the miniature scale scene.
        /// </summary>
        /// <param name="onMapLoaded">A callback to be executed once the map has finished loading</param>
        protected override void WaitForMapToLoad(Action onMapLoaded)
        {
            EditorApplication.update += CheckMapLoaded;

            void CheckMapLoaded()
            {
                if (Map.IsLoaded)
                {
                    Debug.Log("Finished loading");

                    EditorApplication.update -= CheckMapLoaded;
                    EditorUtility.ClearProgressBar();
                    onMapLoaded?.Invoke();
                }
                else
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Loading", "Waiting for map to load...", -1))
                    {
                        EditorUtility.ClearProgressBar();
                        EditorApplication.update -= CheckMapLoaded;
                    }
                }
            }
        }
    }
}