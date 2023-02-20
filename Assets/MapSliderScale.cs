using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Maps.Unity;

public class MapSliderScale : MonoBehaviour
{
    public MapRenderer target;
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        target.ZoomLevel = slider.value;
    }
}
