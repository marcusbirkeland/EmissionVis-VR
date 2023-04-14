using System;
using Editor.NetCDF;
using Microsoft.Maps.Unity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.VisualizationSpawner
{
    public abstract class MapVisualizerSpawner
    {
        protected readonly AttributeDataGetter.FileAttributes SelectedCdfAttributes;
        protected readonly GameObject Map;
        protected readonly float RotationAngle;
        
        protected GameObject VisualizerHolder;

        
        protected MapVisualizerSpawner(string attributesFilePath, string cdfFilePath, GameObject map, float rotationAngle)
        {
            SelectedCdfAttributes = AttributeDataGetter.GetFileAttributes(attributesFilePath, cdfFilePath);

            MapRenderer mapRenderer = map.GetComponent<MapRenderer>();
            if (mapRenderer == null)
            {
                throw new Exception("The selected Map GameObject does not have a MapRenderer component. Please select a GameObject with the MapRenderer component.");
            }
            Map = map;
            RotationAngle = rotationAngle;
        }

        
        protected void DeletePreviousObject(string holderName)
        {
            for (int i = Map.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = Map.transform.GetChild(i);
                if (child.name == holderName)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}