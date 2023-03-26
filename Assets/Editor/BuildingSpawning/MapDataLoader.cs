using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Geospatial;
using UnityEngine;

namespace Editor.BuildingSpawning
{
    public static class MapDataLoader
    {
        [Serializable]
        public struct Position
        {
            public double lat;
            public double lon;

            public static implicit operator LatLon(Position position)
            {
                return new LatLon(position.lat, position.lon);
            }

            public static implicit operator LatLonAlt(Position position)
            {
                return new LatLonAlt(position.lat, position.lon, 0.0);
            }
        }

        [Serializable]
        public struct CdfData
        {
            public string filePath;
            public Position position;
        }

        [Serializable]
        private struct CdfDataListWrapper : IEnumerable<CdfData>
        {
            public List<CdfData> data;
            
            public IEnumerator<CdfData> GetEnumerator()
            {
                return data.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static CdfData LoadMapData(string jsonFilePath, string cdfFilePath)
        {
            if (!File.Exists(jsonFilePath))
            {
                Debug.LogError("JSON file not found.");
                return default;
            }

            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                CdfDataListWrapper cdfDataList = JsonUtility.FromJson<CdfDataListWrapper>("{\"data\":" + jsonContent + "}");

                foreach (CdfData data in cdfDataList)
                {
                    if (data.filePath == cdfFilePath)
                    {
                        return data;
                    }
                }

                Debug.LogError("Invalid JSON format.");
            }
            catch (Exception e) when (e is ArgumentException or InvalidOperationException)
            {
                Debug.LogError($"Error parsing JSON content: {e.Message}");
            }

            return default;
        }
    }
}