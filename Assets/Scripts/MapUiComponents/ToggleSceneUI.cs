using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MapUiComponents
{
    /// <summary>
    /// The ToggleSceneUI class handles the functionality of switching between miniature and full-scale scenes.
    /// </summary>
    public class ToggleSceneUI : MonoBehaviour
    {
        [SerializeField]
        private Button toggleSceneButton;
        
        
        private void Awake()
        {
            toggleSceneButton.onClick.AddListener(SwitchScene);
        }

        
        private void OnDestroy()
        {
            toggleSceneButton.onClick.RemoveListener(SwitchScene);
        }

        
        /// <summary>
        /// Switches between the miniature and full-scale scenes.
        /// </summary>
        /// <remarks>
        /// TODO: Replace with better way of scene switching.
        /// Using the scene name as a string seems doomed to fail at some point.
        /// </remarks>
        private static void SwitchScene()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            string mini = MapUI.Instance.miniatureSceneName;
            string fs = MapUI.Instance.fullScaleSceneName;
            
            string targetSceneName;

            if (currentScene == mini)
            {
                targetSceneName = fs;
            }
            else if (currentScene == fs)
            {
                targetSceneName = mini;
            }
            else
            {
                throw new Exception("Invalid scene assignments");
            }

            // Remove the layer mask containing all objects
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Default"));
            SceneManager.LoadSceneAsync(targetSceneName);
        }
    }
}