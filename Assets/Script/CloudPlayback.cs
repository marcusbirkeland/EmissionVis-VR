using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudPlayback : MonoBehaviour
{
    private Button button;
    public CloudSlider cloudSlider;

    public Sprite playSprite;
    public Sprite pauseSprite;

    public Image buttonIcon;

    // Start is called before the first frame update
    void Start()
    {   
        button  = GetComponent<Button>();
	    button.onClick.AddListener(HandleClick);
    }

    // Update is called once per frame
    void Update()
    {
        if(cloudSlider.IsPlaying()){
            buttonIcon.sprite = pauseSprite;
        } else{
            buttonIcon.sprite = playSprite;
        }
    }

    private void HandleClick(){
        if(cloudSlider.IsPlaying()){
            cloudSlider.Pause();
        }else{
            cloudSlider.Play();
        }
    }
}
