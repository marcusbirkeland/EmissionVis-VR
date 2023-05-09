using System;
using Editor.NetCDF;
using Editor.NetCDF.Types;
using Editor.SceneManagement;
using Editor.Spawner.BuildingSpawner;
using Editor.Spawner.CloudSpawner;
using Editor.Spawner.RadiationSpawner;
using Microsoft.Maps.Unity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.SceneBuilder
{
    /// <summary>
    /// The MiniatureSceneBuilder is a concrete implementation of the BaseSceneBuilder for dynamically creating
    /// miniature scale data visualizations.
    /// It utilizes a <see cref="MapRenderer"/> as its map component.
    /// </summary>
    public class MiniatureSceneBuilder : BaseSceneBuilder<MapRenderer>
    {
        /// <summary>
        /// Private constructor.
        /// Entirely inherited from <see cref="BaseSceneBuilder{T}"/>.
        /// Only used in the CreateScene method.
        /// </summary>
        /// <param name="ncData">The <see cref="NcDataset"/> containing all the user selected data.</param>
        private MiniatureSceneBuilder(NcDataset ncData) : base(ncData)
        {
        }
        
        /// <summary>
        /// Creates a new miniature scene based on the provided NcDataset and calls the provided callback after
        /// scene creation is complete.
        /// </summary>
        /// <param name="ncData">The <see cref="NcDataset"/> containing all the user-selected data.</param>
        /// <param name="onSceneCreated">
        /// An optional <see cref="Action"/> delegate to be executed After scene creation is finished. Default value is null.
        /// </param>
        public static void CreateScene(NcDataset ncData, Action onSceneCreated = null)
        {
            Debug.Log("Creating miniature scene");

            SceneAsset templateScene = GetTemplateScene("Miniature Template");
            if (!SceneDuplicator.CreateAndLoadDuplicateScene(templateScene, ncData.MapName + " Miniature")) return;

            MiniatureSceneBuilder miniBuilder = new(ncData);

            miniBuilder.BuildScene(onSceneCreated);

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
        
        
        /// <summary>
        /// Creates all the data objects using their miniature spawner implementations.
        /// </summary>
        protected override void CreateDataObjects()
        {
            CreateBuildings<MiniatureBuildingSpawner>();
            CreateClouds<MiniatureCloudSpawner>();
            CreateRadiation<MiniatureRadiationSpawner>();
        }

        
        /// <summary>
        /// Sets the center of the map to the center of the dataset.
        /// </summary>
        /// <remarks>
        /// TODO: setup the correct zoom level based on the dataset scope.
        /// </remarks>
        protected override void SetUpMap()
        {
            //NOTE: Using building cdf path as the position baseline, but any of the cdf files should work.
            Map.Center = ScopeDataGetter.GetCenterPosition(NcData.BuildingCdfPath);
        }
        

        /// <summary>
        /// Displays a progressbar while waiting for the map to load in the miniature scale scene.
        /// </summary>
        /// <param name="onMapLoaded">A callback to be executed once the map has finished loading</param>
        /// <remarks>
        /// This method sometimes loads forever unless the user clicks on the scene view. (Presumably to update it).
        /// It is unsure why this happens, and we have not found a workaround for it.
        /// </remarks>
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