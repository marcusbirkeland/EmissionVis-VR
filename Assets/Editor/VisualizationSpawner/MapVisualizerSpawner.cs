using System;
using Editor.NetCDF;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.VisualizationSpawner
{
    public abstract class MapVisualizerSpawner
    {
        protected readonly AttributeDataGetter.FileAttributes SelectedCdfAttributes;
        protected readonly GameObject Map;
        protected readonly float RotationAngle;
        protected readonly float UnityUnitsPerMeter;
        
        protected GameObject VisualizationHolder;

        
        protected MapVisualizerSpawner(string cdfFilePath, GameObject map, float rotationAngle)
        {
            SelectedCdfAttributes = AttributeDataGetter.GetFileAttributes(cdfFilePath);
            UnityUnitsPerMeter = (float) (1 / Math.Cos(Math.PI * SelectedCdfAttributes.position.lat / 180.0));

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
        
        
        protected static Texture2D GetHeightMapImg(string mapName)
        {
            string path = $"MapData/{mapName}/HeightMap/heightMap";
            Texture2D img = Resources.Load<Texture2D>(path);

            if (img == null)
            {
                Debug.LogError("Texture not found in Resources at: " + path);
            }

            return img;
        }
    }
}