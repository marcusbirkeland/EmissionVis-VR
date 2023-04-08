using UnityEngine;
using UnityEngine.UI;

namespace NewMapUI
{
    public class VisibilitySlider : MonoBehaviour
    {
        public Slider Slider;

        private void Start()
        {
            Slider.minValue = 0;
            Slider.maxValue = 1;
            Slider.onValueChanged.AddListener(OnVisibilitySliderChange);
        }

        public void OnVisibilitySliderChange(float value)
        {
            //MapUI.Instance.CloudModel.Visibility = value;
        }
    }
}