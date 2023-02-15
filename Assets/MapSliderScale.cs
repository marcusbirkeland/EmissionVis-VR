using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSliderScale : MonoBehaviour
{
    public GameObject target;
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float newScale = slider.value;
        target.transform.localScale = new Vector3(slider.value, slider.value, slider.value);
    }
}
