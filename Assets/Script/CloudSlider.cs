using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudSlider : MonoBehaviour
{

    public GameObject cloudManager;
    public Slider slider;

    [Range(0.0f, 2.99f)]
    public float floatSlider = 0;
    public Text text;

    private float time =  0;

    private float prevTime = 0;
    private List<Renderer> cloudRenderers = new List<Renderer>();
    private CloudMapManager mapManager;

    int numSteps(float prev, float current){
        return Mathf.FloorToInt(current) - Mathf.FloorToInt(prev);
    }

    // Start is called before the first frame update
    void Start()
    {
        mapManager = cloudManager.GetComponent<CloudMapManager>();

        LOD[] lods = this.gameObject.GetComponent<LODGroup>().GetLODs();
        foreach (LOD lod in lods)
        {
            foreach (Renderer ren in lod.renderers)
            {
                cloudRenderers.Add(ren);
                ren.material.SetFloat("_ColorMapAlpha", time % 1);
            }
        }

        slider.minValue = 0;
        slider.maxValue = mapManager.GetMapCount() - 1.01f;
        slider.value = 0;
        slider.onValueChanged.AddListener(delegate {OnSliderChange ();});
        text.text = slider.value.ToString("F2");

        foreach (Renderer ren in cloudRenderers)
        {
            ren.material.SetFloat("_ColorMapAlpha", time % 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(slider.maxValue < 1){
            slider.maxValue = mapManager.GetMapCount() - 1.01f;
        }
    }

    public void OnSliderChange(){
        Debug.Log("Slider value: " + slider.value);
        ChangeTime(slider.value);
        text.text = slider.value.ToString("F2");
    }

    private void ChangeTime(float value)
    {
        time = value; 
        if (prevTime != time)
        {
            int nSteps = numSteps(prevTime, time);
            //Debug.Log("NSteps: " + nSteps);
            if (nSteps != 0)
            {
                mapManager.UpdateTime(nSteps);
            }
            prevTime = time;
        }

        foreach (Renderer ren in cloudRenderers)
        {
            ren.material.SetFloat("_ColorMapAlpha", time % 1);
        }
    }
}
