using System;
using Editor.NetCDF;
using Editor.NetCDF.Types;
using Editor.SceneManagement;
using Editor.Spawner.BuildingSpawner;
using Editor.Spawner.CloudSpawner;
using Editor.Spawner.RadiationSpawner;
using Esri.ArcGISMapsSDK.Components;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.SceneBuilder
{
    /// <summary>
    /// The FullScaleSceneBuilder is a concrete implementation of the BaseSceneBuilder for dynamically creating
    /// full scale data visualizations.
    /// It utilizes an <see cref="ArcGISMapComponent"/> as its map component.
    /// </summary>
    public class FullScaleSceneBuilder : BaseSceneBuilder<ArcGISMapComponent>
    {
        /// <summary>
        /// Private constructor.
        /// Entirely inherited from <see cref="BaseSceneBuilder{T}"/>.
        /// Only used in the static CreateScene method.
        /// </summary>
        /// <param name="ncData">The <see cref="NcDataset"/> containing all the user selected data.</param>
        private FullScaleSceneBuilder(NcDataset ncData) : base(ncData)
        {
        }
        
        
        /// <summary>
        /// Creates a new full scale scene based on the provided NcDataset and calls the provided callback after
        /// scene creation is complete.
        /// </summary>
        /// <param name="ncData">The <see cref="NcDataset"/> containing all the user-selected data.</param>
        /// <param name="onSceneCreated">
        /// An optional <see cref="Action"/> delegate to be executed After scene creation is finished. Default value is null.
        /// </param>
        public static void CreateScene(NcDataset ncData, Action onSceneCreated = null)
        {
            Debug.Log("Creating full scale scene");

            SceneAsset templateScene = GetTemplateScene("Full Scale Template");
            if (!SceneDuplicator.CreateAndLoadDuplicateScene(templateScene, ncData.MapName + " Full Scale")) return;

            FullScaleSceneBuilder fullScaleBuilder = new(ncData);

            fullScaleBuilder.BuildScene(onSceneCreated);

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }


        /// <summary>
        /// Creates all the data objects using their full scale spawner implementations.
        /// </summary>
        protected override void CreateDataObjects()
        {
            CreateBuildings<FullScaleBuildingSpawner>();
            CreateClouds<FullScaleCloudSpawner>();
            CreateRadiation<FullScaleRadiationSpawner>();
        }


        /// <summary>
        /// Sets the arcgis origin position to the center of the dataset, and enables mesh colliders.
        /// </summary>
        protected override void SetUpMap()
        {
            //NOTE: Using building cdf path as the position baseline, but any of the cdf files should work.
            Map.OriginPosition = ScopeDataGetter.GetCenterPosition(NcData.BuildingCdfPath);
            
            Map.MeshCollidersEnabled = true;
        }


        /// <summary>
        /// Displays a progressbar while waiting for the map to load.
        /// </summary>
        /// <param name="onMapLoaded">A callback to be executed once the map has finished loading</param>
        /// <remarks>
        /// The ArcGis load status returns true far too early, and is unreliable for an practical purpose.
        /// The current implementation effectively checks whether the arcgis api key is setup correctly,
        /// and whether the device is connected to the internet.
        /// </remarks>
        protected override void WaitForMapToLoad(Action onMapLoaded)
        {
            EditorApplication.update += CheckMapLoaded;

            void CheckMapLoaded()
            {
                if (Map.View.Map.Basemap.LoadStatus == Esri.GameEngine.ArcGISLoadStatus.Loaded)
                {
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