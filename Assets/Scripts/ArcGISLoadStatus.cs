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

public class ArcGISLoadStatus : MonoBehaviour
{
    private ArcGISMapComponent arcGisMap;
    public GameObject loadingScreen;

    // Start is called before the first frame update
    void Start()
    {
        loadingScreen.SetActive(true);
        arcGisMap = gameObject.GetComponent<ArcGISMapComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(arcGisMap.View.Map.Basemap.LoadStatus);
        if (arcGisMap.View.Map.Basemap.LoadStatus == Esri.GameEngine.ArcGISLoadStatus.Loaded)
        {
            loadingScreen.SetActive(false);
            enabled = false;
        }
    }
}
