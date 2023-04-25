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
                MapUI.CloudManager.ChangeOpacity
            );
        }
    }
}