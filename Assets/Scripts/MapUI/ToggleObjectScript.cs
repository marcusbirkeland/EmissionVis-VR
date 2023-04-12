using System;
using JetBrains.Annotations;
using Microsoft.Maps.Unity;
using UnityEngine;

namespace MapUI
{
    public static class ToggleObjectScript
    {
        public static void ToggleActiveState([NotNull] GameObject map, bool isOn)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            
            MapPin mapPin = map.GetComponent<MapPin>();
            
            if (mapPin)
            {
                mapPin.enabled = isOn;
                return;
            }
            
            map.SetActive(!map.activeSelf);
        }
    }
}