using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleObjectScript : MonoBehaviour
{
    public GameObject objectToToggle; // Reference to the cube object
    public Toggle toggle; // Reference to the UI toggle object

    public bool showObject = true;

    // Start is called before the first frame update
    void Start()
    {
        // Set the toggle to match the initial state of the cube object
        toggle.isOn = showObject;
        objectToToggle.SetActive(showObject);
    }

    // Update is called once per frame
    void Update()
    {
        showObject = toggle.isOn;
        // Check if the toggle value has changed
        if (showObject != objectToToggle.activeSelf)
        {
            // Toggle the visibility of the cube object
            objectToToggle.SetActive(showObject);
        }
    }
}
