using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Samples.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Extent;
using Esri.GameEngine.Geometry;
using Esri.Unity;
using Esri.GameEngine;
using System;

public class ArcGISLoadStatus : MonoBehaviour
{
    private ArcGISMapComponent arcGisMap;
    public GameObject loadingScreen;

    // Start is called before the first frame update
    void Start()
    {
        loadingScreen.SetActive(true);
        arcGisMap = gameObject.GetComponent<ArcGISMapComponent>();
        //arcGisMap.View.Map.DoneLoading += DoneLoading; // <- stops too soon
        StartCoroutine(Wait(8));
    }

    IEnumerator Wait(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        loadingScreen.SetActive(false);
    }

    /*private void DoneLoading(Exception e)
    {
        Debug.Log("FIRST LOAD DONE!");
    }*/


    // Update is called once per frame
    void Update()
    {

    }
}
