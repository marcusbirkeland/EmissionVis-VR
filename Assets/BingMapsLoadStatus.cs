using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Maps.Unity;

public class BingMapsLoadStatus : MonoBehaviour
{
    public GameObject loadingScreen;
    private MapRendererBase mapRendererBase;
    // Start is called before the first frame update
    void Start()
    {
        loadingScreen.SetActive(true);
        mapRendererBase = gameObject.GetComponent<MapRendererBase>();
        StartCoroutine(WaitForMapLoad());
    }

    IEnumerator WaitForMapLoad()
    {
        yield return new WaitForMapLoaded(mapRendererBase);
        loadingScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
