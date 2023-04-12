using System;
using UnityEngine;
using NewMapUI;
using Object = UnityEngine.Object;

namespace Editor
{
    public static class MapUISetup
    {
        
        public static void SetBuildingHolder(GameObject holder)
        {
            NewMapUI.MapUI ui = FindMapUIInScene();

            ui.buildingHolder = holder;
        }

        public static void SetRadiationHolder(GameObject holder)
        {
            NewMapUI.MapUI ui = FindMapUIInScene();

            ui.radiationHolder = holder;
        }

        public static void SetCloudManager(CloudManager manager)
        {
            NewMapUI.MapUI ui = FindMapUIInScene();

            ui.cloudManager = manager;
        }
        
        private static NewMapUI.MapUI FindMapUIInScene()
        {
            NewMapUI.MapUI mapUI = Object.FindObjectOfType<NewMapUI.MapUI>();
            if (mapUI == null)
            {
                throw new Exception("There is no MapUI in the scene!");
            }
            return mapUI;
        }
    }
}