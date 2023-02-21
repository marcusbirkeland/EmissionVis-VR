using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmissionVisibilityScript : MonoBehaviour
{

    public GameObject currentGameObject;
    public float alpha = 0.5f;//half transparency
    //Get current material
    private Material currentMat;

    // Start is called before the first frame update
    void Start()
    {
        currentGameObject = gameObject;
        currentMat = currentGameObject.GetComponent<Renderer>().material;

    }

/*
    // Update is called once per frame
    void Update()
    {
        ChangeAlpha(currentMat, alpha);
    }

*/

    void ChangeAlpha(Material mat, float alphaVal)
    {
        Color oldColor = mat.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaVal);
        mat.SetColor("_Color", newColor);

    }

    public void ChangeAlphaOnValueChange(Slider slider)
    {
        ChangeAlpha(currentMat, slider.value);
    }
    
}
