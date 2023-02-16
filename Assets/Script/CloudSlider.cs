using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSlider : MonoBehaviour
{

    public GameObject cloudManager;
    public GameObject clouds;

    [Range(0.0f, 2.99f)]
    public float time =  0;

    private float prevTime = 0;
    private Renderer cloudRenderer;
    private CloudMapManager mapManager;




    int numSteps(float prev, float current){
        return Mathf.FloorToInt(current) - Mathf.FloorToInt(prev);
    }

    // Start is called before the first frame update
    void Start()
    {
        mapManager = cloudManager.GetComponent<CloudMapManager>();
        cloudRenderer = clouds.GetComponent<Renderer>();
                
        cloudRenderer.material.SetFloat("_ColorMapAlpha", time%1);
    }

    // Update is called once per frame
    void Update()
    {
        if(prevTime != time){
            int nSteps = numSteps(prevTime, time);
            //Debug.Log("NSteps: " + nSteps);
            if(nSteps != 0){
                
                mapManager.UpdateTime(nSteps);
            }
            prevTime = time;
        }

        cloudRenderer.material.SetFloat("_ColorMapAlpha", time%1);
    }
}
