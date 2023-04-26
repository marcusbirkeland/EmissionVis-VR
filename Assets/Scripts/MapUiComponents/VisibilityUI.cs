using UnityEngine;
using UnityEngine.UI;

namespace MapUiComponents
{
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

        private static void UpdateCloudOpacity(float value)
        {
            MapUI.CloudManager.ChangeOpacity(value);
            
            
        }
    }
}