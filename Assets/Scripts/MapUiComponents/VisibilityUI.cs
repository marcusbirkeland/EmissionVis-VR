using UnityEngine;
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
        }

        /// <summary>
        /// Updates the opacity of the clouds in the scene based on the slider value.
        /// </summary>
        /// <param name="value">The slider value representing the desired opacity of the clouds.</param>
        private static void UpdateCloudOpacity(float value)
        {
            MapUI.CloudManager.ChangeOpacity(value);
        }
    }
}