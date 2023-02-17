using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowConstructionDataScript : MonoBehaviour
{
    public GameObject Cube; // Reference to the cube object
    public Toggle ConstructionDataToggle; // Reference to the UI toggle object

    public bool showConstruction = true;

    // Start is called before the first frame update
    void Start()
    {
        // Set the toggle to match the initial state of the cube object
        ConstructionDataToggle.isOn = showConstruction;
    }

    // Update is called once per frame
    void Update()
    {
        ConstructionDataToggle.isOn = showConstruction;
        // Check if the toggle value has changed
        if (ConstructionDataToggle.isOn != Cube.activeSelf)
        {
            // Toggle the visibility of the cube object
            Cube.SetActive(ConstructionDataToggle.isOn);
        }
    }
}
