using System;
using Microsoft.Maps.Unity;
using UnityEngine;

namespace MapUiComponents
{
    public static class ToggleObjectScript
    {
        public static void ToggleActiveState(GameObject holder, bool isOn)
        {
            if (holder == null) throw new ArgumentNullException(nameof(holder));
            
            MapPin mapPin = holder.GetComponent<MapPin>();
            
            if (mapPin)
            {
                mapPin.enabled = isOn;
                return;
            }
            
            holder.SetActive(isOn);
        }
    }
}