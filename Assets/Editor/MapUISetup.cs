using System;
using MapUI;
using UnityEngine;
using Visualization;
using Object = UnityEngine.Object;

namespace Editor
{
    public static class MapUISetup
    {
        
        public static void SetBuildingHolder(GameObject holder)
        {
            MapUI.MapUI ui = FindMapUIInScene();

            ui.buildingHolder = holder;
        }

        public static void SetRadiationHolder(GameObject holder)
        {
            MapUI.MapUI ui = FindMapUIInScene();

            ui.radiationHolder = holder;
        }

        public static void SetCloudManager(CloudManager manager)
        {
            MapUI.MapUI ui = FindMapUIInScene();

            ui.cloudManager = manager;
        }
        
        private static MapUI.MapUI FindMapUIInScene()
        {
            MapUI.MapUI mapUI = Object.FindObjectOfType<MapUI.MapUI>();
            if (mapUI == null)
            {
                throw new Exception("There is no MapUI in the scene!");
            }
            return mapUI;
        }
    }
}