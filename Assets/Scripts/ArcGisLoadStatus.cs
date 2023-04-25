using System.Collections;
using UnityEngine;

public class ArcGisLoadStatus : MonoBehaviour
{
    public GameObject loadingScreen;

    private void Start()
    {
        loadingScreen.SetActive(true);
        StartCoroutine(Wait(5));
    }

    //TODO: Should be replaced with an check for when the Arcgis map has finished loading in.
    //So far, we have been unable to find a way of doing this.
    private IEnumerator Wait(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        loadingScreen.SetActive(false);
    }
}