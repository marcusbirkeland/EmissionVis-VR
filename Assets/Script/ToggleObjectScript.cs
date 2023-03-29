using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Maps.Unity;

public class ToggleObjectScript : MonoBehaviour
{
    [Tooltip("*ONLY for Bing map* (can't toggle active state of game objects containing a map pin component).")]
    public MapPin pinToToggle;

    [Tooltip("*ONLY for ArcGIS map*")]
    public GameObject objectToToggle; 

    public void ToggleObjectActiveState()
    {
        if (pinToToggle) pinToToggle.enabled = gameObject.GetComponent<Toggle>().isOn;
        if (objectToToggle) objectToToggle.SetActive(gameObject.GetComponent<Toggle>().isOn);
    }
}
