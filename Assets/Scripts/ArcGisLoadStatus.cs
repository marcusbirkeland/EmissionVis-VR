using System.Collections;
using UnityEngine;

public class ArcGisLoadStatus : MonoBehaviour
{
    public GameObject loadingScreen;

    private void Start()
    {
        loadingScreen.SetActive(true);
        StartCoroutine(Wait(8));
    }

    //TODO: Replace with actual measure of map load status.
    //We have not found a good way of doing this. The built-in ArcGis MapLoadStatus is insufficient. 
    private IEnumerator Wait(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        loadingScreen.SetActive(false);
    }
}
