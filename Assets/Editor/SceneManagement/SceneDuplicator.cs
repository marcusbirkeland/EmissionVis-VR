﻿using System.IO;
using Microsoft.Maps.Unity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.SceneManagement
{
    public static class SceneDuplicator
    {
        public static bool CreateAndLoadDuplicateScene(SceneAsset templateSceneAsset, string mapName)
        {
            //No assets selected
            if (templateSceneAsset == null)
            {
                Debug.LogError("Invalid SceneAsset provided.");
                return false;
            }
            
            string templateScenePath = AssetDatabase.GetAssetPath(templateSceneAsset);

            
            // Check if the user wants to save the current scene
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.Log("Scene creation cancelled by user.");
                return false;
            }
            
            if (!HasMapRenderer(templateScenePath))
            {
                Debug.LogError("The selected scene does not contain a GameObject with a MapRenderer component. Please select a different scene.");
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

            // Open the new scene
            Scene newScene = EditorSceneManager.OpenScene(newScenePath, OpenSceneMode.Single);
            
            // Mark the new scene as dirty
            EditorSceneManager.MarkSceneDirty(newScene);
            
            // Save the new scene
            bool saveSuccess = EditorSceneManager.SaveScene(newScene, newScenePath);

            if (!saveSuccess)
            {
                Debug.LogError("Failed to save the new scene.");
                return false;
            }
            
            Debug.Log($"Scene '{mapName}' created and saved at '{newScenePath}'.");
            return true;
        }


        // Check if the templateScene has a mapRenderer component, returns false, and loads the original scene if it doesnt.
        private static bool HasMapRenderer(string scenePath)
        {
            Scene originalScene = SceneManager.GetActiveScene();
            string originalScenePath = originalScene.path;

            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            MapRenderer mapRenderer = Object.FindObjectOfType<MapRenderer>();

            if (mapRenderer != null) return true;
            
            EditorSceneManager.OpenScene(originalScenePath, OpenSceneMode.Single);
            return false;
        }
    }
}