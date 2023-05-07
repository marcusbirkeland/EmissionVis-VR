using MapUiComponents;
using UnityEditor;
using UnityEngine;

namespace Editor.SceneManagement
{
    /// <summary>
    /// Provides functionality to manage the MapUI component in the Unity scene.
    /// </summary>
    public static class MapUiManager
    {
        /// <summary>
        /// Sets the miniature and full-scale scene names for the MapUI component in the Unity scene.
        /// </summary>
        /// <param name="mapName">The base name of the map to be used for generating scene names.</param>
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