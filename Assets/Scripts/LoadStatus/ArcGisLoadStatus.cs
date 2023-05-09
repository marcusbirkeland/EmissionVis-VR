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

        
        /// <summary>
        /// Waits for a specified number of seconds before deactivating the loading screen.
        /// This is a temporary solution until a better measure of map load status is implemented.
        /// The built-in ArcGis MapLoadStatus is insufficient.
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
