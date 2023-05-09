using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Visualization;

namespace MapUiComponents
{
    /// <summary>
    /// The TimelineUI class is responsible for managing the UI components of the timeline, including the slider
    /// and play/pause button. It also informs the <see cref="CloudManager"/> whenever the time value changes.
    /// </summary>
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
        
        private float _currentTime;
        private float _prevTime;

        private bool _isPlaying;
        
        /// <summary>
        /// Provides a slightly shorter reference to the CloudManager instance.
        /// </summary>
        private static CloudManager CloudManager => MapUI.CloudManager;


        /// <summary>
        /// Initializes the timeline UI by setting up event listeners and initial values.
        /// </summary>
        private void Start()
        {
            timelineSlider.onValueChanged.AddListener(OnSliderChange);
            
            toggleButton.onClick.AddListener(TogglePlaying);
            UpdateButtonIcon();

            SetSliderValues();

            SceneManager.sceneLoaded += HandleSceneChange;
        }

        
        /// <summary>
        /// Removes event listeners when the object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            timelineSlider.onValueChanged.RemoveListener(OnSliderChange);
            
            toggleButton.onClick.RemoveListener(TogglePlaying);
            SceneManager.sceneLoaded -= HandleSceneChange;
        }

        
        /// <summary>
        /// Updates the timeline during playback.
        /// </summary>
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
        
        
        /// <summary>
        /// Sets the initial values for the timeline slider.
        /// </summary>
        private void SetSliderValues()
        {
            timelineSlider.minValue = 0;
            timelineSlider.value = 0;
            timeValueText.text = "0.0";
            timelineSlider.maxValue = CloudManager.MapCount - 1.1f;
        }
        
        
        /// <summary>
        /// Handles changes to the timeline slider value.
        /// </summary>
        /// <param name="value">The new value of the slider.</param>
        private void OnSliderChange(float value)
        {
            timeValueText.text = value.ToString("F2");
            
            ChangeTime(value);
        }

        
        /// <summary>
        /// Changes the current time and updates the cloud visualization accordingly.
        /// </summary>
        /// <param name="value">The new time value.</param>
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

        
        /// <summary>
        /// Handles scene changes by resetting the previous time and updating the cloud visualization.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The scene loading mode.</param>
        private void HandleSceneChange(Scene scene, LoadSceneMode mode)
        {
            _prevTime = 0;
            
            ChangeTime(timelineSlider.value);
        }
        
        
        /// <summary>
        /// Calculates the number of steps between the previous and current time.
        /// </summary>
        /// <param name="prev">The previous time value.</param>
        /// <param name="current">The current time value.</param>
        /// <returns>The number of steps between the previous and current time.</returns>
        private static int NumSteps(float prev, float current)
        {
            return Mathf.FloorToInt(current) - Mathf.FloorToInt(prev);
        }

        
        /// <summary>
        /// Toggles the playback state between playing and paused.
        /// </summary>
        private void TogglePlaying()
        {
            _isPlaying = !_isPlaying;
            UpdateButtonIcon();
        }

        
        /// <summary>
        /// Updates the play/pause button icon based on the current playback state.
        /// </summary>
        private void UpdateButtonIcon()
        {
            buttonIcon.sprite = _isPlaying ? pauseSprite : playSprite;
        }
    }
}