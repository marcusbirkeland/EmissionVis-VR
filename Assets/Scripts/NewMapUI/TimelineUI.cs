using System;
using System.Collections;
using Cloud;
using UnityEngine;
using UnityEngine.UI;

namespace NewMapUI
{
    public class TimelineUI : MonoBehaviour
    {
        [SerializeField]
        private Slider slider;
        
        [SerializeField]
        private Button toggleButton;
        
        [SerializeField]
        private Sprite playSprite;
        
        [SerializeField]
        private Sprite pauseSprite;

        [SerializeField]
        private Image buttonIcon;

        [SerializeField] 
        private Text text;
        
        [SerializeField]        
        private float playbackRate = 0.5f;
        
        public CloudManager cloudManager;

        //NOTE: Represents a number of seconds since the playback started, not the database timestamps. 
        private float _currentTime;
        private float _prevTime;

        private bool _isPlaying;

        private void Start()
        {
            slider.onValueChanged.AddListener(OnSliderChange);
            
            toggleButton.onClick.AddListener(TogglePlaying);
            UpdateButtonIcon();

            StartCoroutine(SetSliderValuesWhenReady());
        }

        private void Update()
        {
            if (!_isPlaying) return;

            float maxValue = cloudManager.MapCount - 1.01f;
            slider.maxValue = maxValue;
            _currentTime += playbackRate * Time.deltaTime;

            if (_currentTime >= maxValue)
            {
                _currentTime = 0.0f;
            }

            slider.value = _currentTime;
        }
        
        
        private IEnumerator SetSliderValuesWhenReady()
        {
            yield return new WaitUntil(() => cloudManager.MapCount > 0);
    
            slider.minValue = 0;
            text.text = "0.0";
            slider.maxValue = cloudManager.MapCount - 1.1f;
        }
        
        
        //Runs every time the slider value changes.
        private void OnSliderChange(float value)
        {
            text.text = value.ToString("F2");
            
            ChangeTime(value);
        }

        private void ChangeTime(float value)
        {
            _currentTime = value;

            int nSteps = NumSteps(_prevTime, _currentTime);
            
            if (nSteps != 0)
            {
                cloudManager.UpdateTime(nSteps);
            }
            _prevTime = _currentTime;

            cloudManager.UpdateAlphaForRenderers(_currentTime);
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