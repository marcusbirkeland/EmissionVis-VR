using MapUiComponents;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class MapUiManager
    {
        public static void SetSceneNames(string mapName)
        {
            MapUI mapUIInstance = Object.FindObjectOfType<MapUI>();
            
            if (mapUIInstance == null)
            {
                Debug.LogWarning("No MapUI instance found in the scene");
                return;
            }

            mapUIInstance.miniatureSceneName = $"{mapName} Miniature";
            mapUIInstance.fullScaleSceneName = $"{mapName} Full Scale";
            
            EditorUtility.SetDirty(mapUIInstance);
        }
    }
}