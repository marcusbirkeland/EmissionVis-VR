using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A serializable class representing a single tutorial slide.
/// </summary>
[System.Serializable]
public class TutorialSlide
{
    public string title = "Title";
    public string description = "Description";
    public Sprite tutorialImage;
}

/// <summary>
/// Manages the tutorial UI, displaying a series of tutorial slides.
/// </summary>
public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TutorialSlide[] tutorialSlides;
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private Button tutorialButton;

    private int _selectedSlideIndex;

    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    private void Start()
    {
        SetSlide();
        tutorialButton.onClick.AddListener(OnTutorialButtonPressed);
    }

    /// <summary>
    /// Sets the slide content in the UI elements.
    /// </summary>
    private void SetSlide()
    {
        titleText.text = tutorialSlides[_selectedSlideIndex].title;
        descriptionText.text = tutorialSlides[_selectedSlideIndex].description;
        tutorialImage.sprite = tutorialSlides[_selectedSlideIndex].tutorialImage;
    }

    /// <summary>
    /// Handles the tutorial button click event and updates the slide content.
    /// </summary>
    private void OnTutorialButtonPressed()
    {
        if (_selectedSlideIndex >= tutorialSlides.Length - 1)
        {
            // Hide the tutorial pane when exiting the last slide
            gameObject.SetActive(false);
        }
        else
        {
            _selectedSlideIndex++;
            SetSlide();
            if (_selectedSlideIndex == tutorialSlides.Length - 1)
            {
                tutorialButton.GetComponentInChildren<Text>().text = "Finish";
            }
        }
    }
}