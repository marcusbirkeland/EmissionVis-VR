using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Maps.Unity;

public class ToggleObjectScript : MonoBehaviour
{
    public MapPin pinToToggle; // Reference to the cube object
    public Toggle toggleUI; // Reference to the UI toggle object

    private bool showObject;

    // Start is called before the first frame update
    void Start()
    {
        // Enable the MapPin script (disabling it hides all game objects associated with it)
        pinToToggle.enabled = showObject;
    }

    // Update is called once per frame
    void Update()
    {
        showObject = toggleUI.isOn;

        // Check if the toggle value has changed
        if (showObject != pinToToggle.enabled)
        {
            // Toggle the visibility of the cube object
            pinToToggle.enabled = showObject;
        }
    }
}
