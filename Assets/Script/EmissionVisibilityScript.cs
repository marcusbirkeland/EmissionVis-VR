using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmissionVisibilityScript : MonoBehaviour
{

    private GameObject currentGameObject;
    public float alpha = 1f;
    //Get current material
    private List<Material> currentMats = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        currentGameObject = this.gameObject;
        
        LOD[] lods = currentGameObject.GetComponent<LODGroup>().GetLODs();
        foreach (LOD lod in lods)
        {
            foreach (Renderer ren in lod.renderers)
            {
                currentMats.Add(ren.material);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ChangeAlpha(float alphaVal)
    {
        // Ideally, only the current loaded LOD would have to be altered. 
        // There is no clear way to extract this information.
        foreach (Material mat in currentMats)
        {
            mat.SetFloat("_Opacity", alphaVal);
        }
    }

    public void ChangeAlphaOnValueChange(Slider slider)
    {
        ChangeAlpha(slider.value);
    }
    
}
