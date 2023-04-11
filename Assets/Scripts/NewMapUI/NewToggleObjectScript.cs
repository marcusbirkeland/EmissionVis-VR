using System;
using JetBrains.Annotations;
using Microsoft.Maps.Unity;
using UnityEngine;

namespace NewMapUI
{
    public static class NewToggleObjectScript
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