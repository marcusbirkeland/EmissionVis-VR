using System;
using Microsoft.Maps.Unity;
using UnityEngine;

namespace MapUiComponents
{
    /// <summary>
    /// The ToggleObjectScript class provides a static method to toggle the active state of a given game object.
    /// </summary>
    public static class ToggleObjectScript
    {
        /// <summary>
        /// Toggles the active state of a given game object.
        /// Used for the data visualizations holder objects.
        /// </summary>
        /// <param name="holder">The game object to toggle.</param>
        /// <param name="isOn">True to activate the game object, false to deactivate it.</param>
        /// <remarks>
        /// Bing maps <see cref="MapRenderer"/> component functions differently from other GameObjects.
        /// It gets toggled via the MapRenderer.enabled property, instead of the regular GameObject.SetActive() method.
        /// </remarks>
        public static void ToggleActiveState(GameObject holder, bool isOn)
        {
            if (holder == null) throw new ArgumentNullException(nameof(holder));
            
            MapPin mapPin = holder.GetComponent<MapPin>();
            
            // If the holder has a MapPin component, toggle its enabled state
            if (mapPin)
            {
                mapPin.enabled = isOn;
                return;
            }
            
            // Otherwise, toggle the active state of the game object
            holder.SetActive(isOn);
        }
    }
}