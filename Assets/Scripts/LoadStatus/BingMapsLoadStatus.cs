using System.Collections;
using Microsoft.Maps.Unity;
using UnityEngine;

namespace LoadStatus
{
    /// <summary>
    /// Manages the loading status of Bing Maps by displaying a loading screen
    /// while the map is loading and hiding it once the map is fully loaded.
    /// </summary>
    public class BingMapsLoadStatus : MonoBehaviour
    {
        /// <summary>
        /// The loading screen GameObject to be displayed during map loading.
        /// </summary>
        public GameObject loadingScreen;

        /// <summary>
        /// Reference to the MapRendererBase component.
        /// </summary>
        private MapRendererBase _mapRendererBase;

        
        /// <summary>
        /// Called before the first frame update.
        /// </summary>
        private void Start()
        {
            loadingScreen.SetActive(true);
            _mapRendererBase = gameObject.GetComponent<MapRendererBase>();
            StartCoroutine(WaitForMapLoad());
        }

        
        /// <summary>
        /// Coroutine to wait for the map to load completely.
        /// </summary>
        /// <returns>IEnumerator to be used by StartCoroutine.</returns>
        private IEnumerator WaitForMapLoad()
        {
            yield return new WaitForMapLoaded(_mapRendererBase);
            loadingScreen.SetActive(false);
        }
    }
}