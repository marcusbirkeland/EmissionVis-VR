using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Maps.Unity;

//NOTE: Never used. Consider deleting.
public class MapSliderScale : MonoBehaviour
{
    public MapRenderer target;
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        // Set to the zoom level to enable easier tweaking in editor.
        slider.value = target.ZoomLevel;
    }

    // Update is called once per frame
    void Update()
    {
        target.ZoomLevel = slider.value;
    }
}
