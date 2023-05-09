using System.Collections;
using UnityEngine;

namespace LoadStatus
{
    /// <summary>
    /// Class responsible for displaying the loading screen while waiting for the ArcGisMap to load.
    /// </summary>
    public class ArcGisLoadStatus : MonoBehaviour
    {
        /// <summary>
        /// The loading screen GameObject to display.
        /// </summary>
        public GameObject loadingScreen;

        
        private void Start()
        {
            loadingScreen.SetActive(true);
            StartCoroutine(Wait(8));
        }

        /// <remarks>
        /// We acknowledge that this hard-coded timeout of 8 seconds is a poor implementation, but the underlying 
        /// technology (arcGIS SDK) did not allow us to check when the map has fully finished loading. It was not 
        /// possible to implement a loading mechanism that waited until the map was completely loaded, as all 
        /// documented classes that implemented the Loadable class finished way too early.
        /// </remarks>
        /// <summary>
        /// Waits for a specified number of seconds before deactivating the loading screen.
        /// </summary>
        /// <param name="seconds">The number of seconds to wait before deactivating the loading screen.</param>
        /// <returns>An IEnumerator to be used with a coroutine that controls the WaitForSeconds object.</returns>
        private IEnumerator Wait(int seconds)
        {
            yield return new WaitForSeconds(seconds);
            loadingScreen.SetActive(false);
        }
    }
}
