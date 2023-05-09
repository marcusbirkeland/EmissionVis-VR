using System.IO;
using Esri.ArcGISMapsSDK.Components;
using Microsoft.Maps.Unity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.SceneManagement
{
    /// <summary>
    /// Static class responsible for creating duplicate scenes based on existing scenes in unity.
    /// </summary>
    public static class SceneDuplicator
    {
        /// <summary>
        /// Creates a new scene based on a template scene and loads it.
        /// </summary>
        /// <param name="templateSceneAsset">The SceneAsset used as a template.</param>
        /// <param name="mapName">The name of the new scene.</param>
        /// <returns>True if the scene is successfully created and saved, false otherwise.</returns>
        public static bool CreateAndLoadDuplicateScene(SceneAsset templateSceneAsset, string mapName)
        {
            // No assets selected
            if (templateSceneAsset == null)
            {
                EditorUtility.DisplayDialog("Invalid SceneAsset", "Invalid SceneAsset provided.", "OK");
                return false;
            }
            
            string templateScenePath = AssetDatabase.GetAssetPath(templateSceneAsset);

            // Check if the user wants to save the current scene
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.Log("Scene creation cancelled by user.");
                return false;
            }
            
            if (!HasMapComponent(templateScenePath))
            {
                EditorUtility.DisplayDialog("Invalid Template Scene", "The selected scene does not contain a GameObject with a MapRenderer or an ArcGis Map component. Please verify the template scenes.", "OK");
                return false;
            }

            string newScenePath = Path.Combine("Assets", mapName + ".unity");

            if (File.Exists(newScenePath))
            {
                bool replace = EditorUtility.DisplayDialog(
                    "Replace Existing Scene?",
                    $"A scene with the name '{mapName}' already exists. Do you want to replace it?",
                    "Replace",
                    "Cancel"
                );

                if (!replace)
                {
                    Debug.Log("Scene creation cancelled by user.");
                    return false;
                }

                // Delete the existing scene file
                AssetDatabase.DeleteAsset(newScenePath);
            }

            // Duplicate the template
            AssetDatabase.CopyAsset(templateScenePath, newScenePath);
            AssetDatabase.Refresh();

            Scene newScene = EditorSceneManager.OpenScene(newScenePath, OpenSceneMode.Single);
            EditorSceneManager.MarkSceneDirty(newScene);
            
            // Save the new scene
            bool saveSuccess = EditorSceneManager.SaveScene(newScene, newScenePath);

            if (!saveSuccess)
            {
                EditorUtility.DisplayDialog("Failed to Save Scene", "Failed to save the new scene.", "OK");
                return false;
            }
            
            Debug.Log($"Scene '{mapName}' created and saved at '{newScenePath}'.");
            return true;
        }

        
        /// <summary>
        /// Checks if the specified scene has a MapRenderer or ArcGISMapComponent attached to any GameObject.
        /// </summary>
        /// <param name="scenePath">The path of the scene to check.</param>
        /// <returns>True if the scene contains a MapRenderer or ArcGISMapComponent, false otherwise.</returns>
        private static bool HasMapComponent(string scenePath)
        {
            Scene originalScene = SceneManager.GetActiveScene();
            string originalScenePath = originalScene.path;

            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            MapRenderer mapRenderer = Object.FindObjectOfType<MapRenderer>();
            ArcGISMapComponent gisMapComponent = Object.FindObjectOfType<ArcGISMapComponent>();

            if (mapRenderer != null || gisMapComponent != null) return true;
            
            EditorSceneManager.OpenScene(originalScenePath, OpenSceneMode.Single);
            return false;
        }
    }
}
