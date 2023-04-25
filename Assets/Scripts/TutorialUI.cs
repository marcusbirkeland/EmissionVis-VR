using System.Collections;
using System.Collections.Generic;
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
    public int selectedSlideIndex = 0;
    private TutorialSlide selectedSlide;
    // Start is called before the first frame update
    void Start()
    {
        selectedSlide = tutorialSlides[selectedSlideIndex];
        setSlide();
        tutorialButton.onClick.AddListener(OnTutorialButtonPressed);
    }

    private void setSlide(){
        titleText.text = selectedSlide.title;
        descriptionText.text = selectedSlide.description;
        tutorialImage.sprite = selectedSlide.tutorialImage;
    }

    private void OnTutorialButtonPressed(){
        if(selectedSlideIndex >= tutorialSlides.Length -1){
            // Hide the tutorial pane when exiting the last slide
            this.gameObject.SetActive(false);
        } else {
            selectedSlideIndex++;
            selectedSlide = tutorialSlides[selectedSlideIndex];
            setSlide();
            if(selectedSlideIndex == tutorialSlides.Length - 1){
                tutorialButton.GetComponentInChildren<Text>().text = "Finish";
            }
        }
    }
}
