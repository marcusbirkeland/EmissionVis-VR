using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Maps.Unity;

public class BingMapsLoadStatus : MonoBehaviour
{
    public GameObject loadingScreen;
    private MapRendererBase _mapRendererBase;

    private void Start()
    {
        loadingScreen.SetActive(true);
        _mapRendererBase = gameObject.GetComponent<MapRendererBase>();
        StartCoroutine(WaitForMapLoad());
    }

    private IEnumerator WaitForMapLoad()
    {
        yield return new WaitForMapLoaded(_mapRendererBase);
        loadingScreen.SetActive(false);
    }
}
