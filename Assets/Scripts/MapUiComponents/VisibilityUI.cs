using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MapUiComponents
{
    /// <summary>
    /// The VisibilityUI class handles the functionality of adjusting the opacity of the clouds in the scene.
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
        /// Updates the opacity of the clouds in the scene based on the slider value.
        /// </summary>
        /// <param name="value">The slider value representing the desired opacity of the clouds.</param>
        private static void UpdateCloudOpacity(float value)
        {
            MapUI.CloudManager.ChangeOpacity(value);
        }

        /// <summary>
        /// Sets the clouds opacity based on the slider value when swapping scenes.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The scene loading mode.</param>
        private void HandleSceneChange(Scene scene, LoadSceneMode mode)
        {
            UpdateCloudOpacity(visibilitySlider.value);
        }
    }
}