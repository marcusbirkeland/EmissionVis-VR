using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmissionVisibilityScript : MonoBehaviour
{

    private  GameObject currentGameObject;
    public float alpha = 1f;
    //Get current material
    private Material currentMat;

    // Start is called before the first frame update
    void Start()
    {
        currentGameObject = gameObject;
        currentMat = currentGameObject.GetComponent<Renderer>().material;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void ChangeAlpha(Material mat, float alphaVal)
    {
        mat.SetFloat("_Opacity", alphaVal);
    }

    public void ChangeAlphaOnValueChange(Slider slider)
    {
        ChangeAlpha(currentMat, slider.value);
    }
    
}
