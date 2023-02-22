using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudSlider : MonoBehaviour
{

    public GameObject cloudManager;
    public Slider slider;
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
    }

    // Update is called once per frame
    void Update()
    {
        slider.minValue = 0;
        slider.maxValue = mapManager.GetMapCount() - 1.01f;
        text.text = mapManager.GetMapCount().ToString();
    }

    public void ChangeTime()
    {
        time = slider.value;
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
