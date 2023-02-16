using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowRadiationScript : MonoBehaviour
{
    public GameObject RadiationCube; // Reference to the cube object
    public Toggle RadiationToggle; // Reference to the UI toggle object

    // Start is called before the first frame update
    void Start()
    {
        // Set the toggle to match the initial state of the cube object
        RadiationToggle.isOn = RadiationCube.activeSelf;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the toggle value has changed
        if (RadiationToggle.isOn != RadiationCube.activeSelf)
        {
            // Toggle the visibility of the cube object
            RadiationCube.SetActive(RadiationToggle.isOn);
        }
    }
}
