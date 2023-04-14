using UnityEngine;
using UnityEngine.UI;

namespace OldMapUI
{
    public class OldCloudPlaybackButton : MonoBehaviour
    {
        [SerializeField]
        private Sprite playSprite;
        
        [SerializeField]
        private Sprite pauseSprite;

        [SerializeField]
        private Image buttonIcon;
        
        public OldCloudSlider cloudSlider;

        
        public void TogglePlaying()
        {
            if(cloudSlider.IsPlaying())
            {
                cloudSlider.Pause();
            }
            else
            {
                cloudSlider.Play();
            }

            UpdateButtonIcon();
        }

        private void UpdateButtonIcon()
        {
            buttonIcon.sprite = cloudSlider.IsPlaying() ? pauseSprite : playSprite;
        }
    }
}