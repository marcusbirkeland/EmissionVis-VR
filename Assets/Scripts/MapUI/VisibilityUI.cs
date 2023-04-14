using UnityEngine;
using UnityEngine.UI;

namespace MapUI
{
    public class VisibilityUI : MonoBehaviour
    {
        [SerializeField]
        private Slider visibilitySlider;

        private void Start()
        {
            visibilitySlider.onValueChanged.AddListener(
                MapUI.Instance.cloudManager.ChangeAlpha);
        }
    }
}