using System;
using Editor.NetCDF;
using Editor.NetCDF.Types;
using Editor.Spawner.BuildingSpawner;
using Editor.Spawner.CloudSpawner;
using Editor.Spawner.RadiationSpawner;
using Esri.ArcGISMapsSDK.Components;
using UnityEditor;

namespace Editor.SceneBuilder
{
    /// <summary>
    /// The FullScaleSceneBuilder is a concrete implementation of the BaseSceneBuilder for dynamically creating full scale data visualizations.
    /// </summary>
    public class FullScaleSceneBuilder : BaseSceneBuilder<ArcGISMapComponent>
    {
        
        public FullScaleSceneBuilder(NcDataset ncData) : base(ncData)
        {
        }


        protected override void CreateDataObjects()
        {
            CreateBuildings<FullScaleBuildingSpawner>();
            CreateClouds<FullScaleCloudSpawner>();
            CreateRadiation<FullScaleRadiationSpawner>();
        }


        /// <summary>
        /// Sets up the map for the full scale scene.
        /// </summary>
        protected override void SetUpMap()
        {
            //NOTE: Using building cdf path as the position baseline, but any of the cdf files should work.
            Map.OriginPosition = AttributeDataGetter.GetCenterPosition(NcData.BuildingCdfPath);
            
            Map.MeshCollidersEnabled = true;
        }


        /// <summary>
        /// Displays a progressbar while waiting for the map to load.
        /// </summary>
        /// <param name="onMapLoaded">A callback to be executed once the map has finished loading</param>
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