using UnityEngine;
using UnityEngine.UI;

namespace NewMapUI
{
    public class VisibilityUI : MonoBehaviour
    {
        [SerializeField]
        private Slider visibilitySlider;

        private void Start()
        {
            visibilitySlider.onValueChanged.AddListener(
                MapUI.CloudManager.ChangeAlpha);
        }
    }
}