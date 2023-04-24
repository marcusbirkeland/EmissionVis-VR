using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MapUiComponents
{
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