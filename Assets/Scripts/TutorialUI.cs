using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TutorialSlide {
    public string title = "Title";
    public string description = "Description";
    public Sprite tutorialImage;
}

public class TutorialUI : MonoBehaviour
{
    public TutorialSlide [] tutorialSlides;
    public Text titleText;
    public Text descriptionText;
    public Image tutorialImage;
    public Button tutorialButton;
    public int selectedSlideIndex;
    private TutorialSlide _selectedSlide;
    
    // Start is called before the first frame update
    private void Start()
    {
        _selectedSlide = tutorialSlides[selectedSlideIndex];
        SetSlide();
        tutorialButton.onClick.AddListener(OnTutorialButtonPressed);
    }

    private void SetSlide(){
        titleText.text = _selectedSlide.title;
        descriptionText.text = _selectedSlide.description;
        tutorialImage.sprite = _selectedSlide.tutorialImage;
    }

    private void OnTutorialButtonPressed(){
        if(selectedSlideIndex >= tutorialSlides.Length -1){
            // Hide the tutorial pane when exiting the last slide
            gameObject.SetActive(false);
        } else {
            selectedSlideIndex++;
            _selectedSlide = tutorialSlides[selectedSlideIndex];
            SetSlide();
            if(selectedSlideIndex == tutorialSlides.Length - 1){
                tutorialButton.GetComponentInChildren<Text>().text = "Finish";
            }
        }
    }
}
