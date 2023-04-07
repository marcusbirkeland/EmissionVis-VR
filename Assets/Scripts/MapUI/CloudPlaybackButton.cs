using Cloud;
using UnityEngine;
using UnityEngine.UI;

namespace MapUI
{
    public class CloudPlaybackButton : MonoBehaviour
    {
        [SerializeField]
        private Sprite playSprite;
        
        [SerializeField]
        private Sprite pauseSprite;

        [SerializeField]
        private Image buttonIcon;
        
        public CloudSlider cloudSlider;

        
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
