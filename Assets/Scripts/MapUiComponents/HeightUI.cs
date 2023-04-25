using UnityEngine;
using UnityEngine.UI;

namespace MapUiComponents
{
    public class HeightUI : MonoBehaviour
    {
        [SerializeField]
        private Slider heightSlider;

        private void Start()
        {
            heightSlider.onValueChanged.AddListener(
                MapUI.CloudManager.ChangeHeight
            );
        }
    }
}