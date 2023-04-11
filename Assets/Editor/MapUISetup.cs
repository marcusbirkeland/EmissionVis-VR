using UnityEngine;
using NewMapUI;

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
            GameObject mapUIGameObject = GameObject.Find("MapUI");
            if (mapUIGameObject == null)
            {
                Debug.LogError("MapUI GameObject not found in the scene.");
                return null;
            }

            // Get the MapUI component
            NewMapUI.MapUI mapUI = mapUIGameObject.GetComponent<NewMapUI.MapUI>();
            if (mapUI == null)
            {
                Debug.LogError("MapUI component not found on the MapUI GameObject.");
                return null;
            }

            return mapUI;
        }
    }
    
    
}