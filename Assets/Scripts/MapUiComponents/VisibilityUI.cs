using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MapUiComponents
{
    /// <summary>
    /// The VisibilityUI class handles the UI responsible for adjusting the opacity of the clouds in the scene.
    /// </summary>
    public class VisibilityUI : MonoBehaviour
    {
        [SerializeField]
        private Slider visibilitySlider;

        
        private void Start()
        {
            visibilitySlider.onValueChanged.AddListener(
                UpdateCloudOpacity
            );

            SceneManager.sceneLoaded += HandleSceneChange;
        }

        
        /// <summary>
        /// Tells the CloudManager to update the opacity based on current slider value.
        /// </summary>
        /// <param name="value">The slider value representing the desired opacity of the clouds.</param>
        private static void UpdateCloudOpacity(float value)
        {
            MapUI.CloudManager.ChangeOpacity(value);
        }
        

        /// <summary>
        /// Triggers on scene change. Updates the opacity to match the current slider value.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The scene loading mode.</param>
        private void HandleSceneChange(Scene scene, LoadSceneMode mode)
        {
            UpdateCloudOpacity(visibilitySlider.value);
        }
    }
}