using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Visualization;

namespace MapUiComponents
{
    public class TimelineUI : MonoBehaviour
    {
        [FormerlySerializedAs("slider")] [SerializeField]
        private Slider timelineSlider;
        
        [SerializeField]
        private Button toggleButton;
        
        [SerializeField]
        private Sprite playSprite;
        
        [SerializeField]
        private Sprite pauseSprite;

        [SerializeField]
        private Image buttonIcon;

        [SerializeField] 
        private Text timeValueText;
        
        [SerializeField]        
        private float playbackRate = 0.5f;
        
        private static CloudManager CloudManager => MapUI.CloudManager;

        //NOTE: Represents a number of seconds since the playback started, not the database timestamps.
        private float _currentTime;
        private float _prevTime;

        private bool _isPlaying;

        private void Start()
        {
            timelineSlider.onValueChanged.AddListener(OnSliderChange);
            
            toggleButton.onClick.AddListener(TogglePlaying);
            UpdateButtonIcon();

            SetSliderValues();
        }

        private void Update()
        {
            if (!_isPlaying) return;

            float maxValue = CloudManager.MapCount - 1.01f;
            timelineSlider.maxValue = maxValue;
            _currentTime += playbackRate * Time.deltaTime;

            if (_currentTime >= maxValue)
            {
                _currentTime = 0.0f;
            }

            timelineSlider.value = _currentTime;
        }
        
        
        private void SetSliderValues()
        {
            timelineSlider.minValue = 0;
            timelineSlider.value = 0;
            timeValueText.text = "0.0";
            timelineSlider.maxValue = CloudManager.MapCount - 1.1f;
        }
        
        
        //Runs every time the slider value changes.
        private void OnSliderChange(float value)
        {
            timeValueText.text = value.ToString("F2");
            
            ChangeTime(value);
        }

        private void ChangeTime(float value)
        {
            _currentTime = value;

            int nSteps = NumSteps(_prevTime, _currentTime);
            
            if (nSteps != 0)
            {
                CloudManager.UpdateTime(nSteps);
            }
            _prevTime = _currentTime;

            CloudManager.ChangeTimeStep(_currentTime);
        }
        
        private static int NumSteps(float prev, float current)
        {
            return Mathf.FloorToInt(current) - Mathf.FloorToInt(prev);
        }

        private void TogglePlaying()
        {
            _isPlaying = !_isPlaying;
            UpdateButtonIcon();
        }

        private void UpdateButtonIcon()
        {
            buttonIcon.sprite = _isPlaying ? pauseSprite : playSprite;
        }
    }
}