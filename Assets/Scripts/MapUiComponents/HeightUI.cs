using UnityEngine;
using UnityEngine.UI;
using Microsoft.Maps.Unity;



namespace MapUI
{
    public class HeightUI : MonoBehaviour
    {
        [SerializeField]
        private Slider heightSlider;

        private void Start()
        {
            heightSlider.onValueChanged.AddListener(
                MapUI.Instance.cloudManager.ChangeHeight
            );
        }
    }
}