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

public class ArcGisLoadStatus : MonoBehaviour
{
    private ArcGISMapComponent _arcGisMap;
    public GameObject loadingScreen;

    private void Start()
    {
        loadingScreen.SetActive(true);
        StartCoroutine(Wait(5));
    }

    private IEnumerator Wait(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        loadingScreen.SetActive(false);
    }
}
