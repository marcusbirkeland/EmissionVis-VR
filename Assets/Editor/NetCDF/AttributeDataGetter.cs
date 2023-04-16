using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Esri.GameEngine.Geometry;
using Microsoft.Geospatial;
using UnityEngine;

namespace Editor.NetCDF
{
    public static class AttributeDataGetter
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

            public static implicit operator ArcGISPoint(Position position)
            {
                return new ArcGISPoint(position.lon, position.lat);
            }
            
            public static Position GetOffsetPosition(double latOffsetMeters, double lonOffsetMeters, Position originalPosition)
            {
                const double earthRadiusMeters = 6371000;

                double latOffsetDegrees = (latOffsetMeters / earthRadiusMeters) * (180 / Math.PI);
                double lonOffsetDegrees = (lonOffsetMeters / (earthRadiusMeters * Math.Cos(originalPosition.lat * (Math.PI / 180)))) * (180 / Math.PI);

                double newLat = originalPosition.lat + latOffsetDegrees;
                double newLon = originalPosition.lon + lonOffsetDegrees;

                return new Position { lat = newLat, lon = newLon };
            }
        }

        [Serializable]
        public struct Size
        {
            public int x;
            public int y;
        }

        [Serializable]
        public struct FileAttributes
        {
            public string filePath;
            public Position position;
            public Size size;
        }

        [Serializable]
        private struct FileDataListWrapper : IEnumerable<FileAttributes>
        {
            public List<FileAttributes> data;

            public IEnumerator<FileAttributes> GetEnumerator()
            {
                return data.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static FileAttributes GetFileAttributes(string cdfFilePath)
        {
            string jsonFilePath = $"{Application.dataPath}/Resources/MapData/attributes.json";
            
            if (!File.Exists(jsonFilePath))
            {
                Debug.LogError("JSON file not found.");
                return default;
            }

            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                FileDataListWrapper fileList = JsonUtility.FromJson<FileDataListWrapper>("{\"data\":" + jsonContent + "}");

                return fileList.FirstOrDefault(data => data.filePath == cdfFilePath);
            }
            catch (Exception e) when (e is ArgumentException or InvalidOperationException)
            {
                Debug.LogError($"Error parsing JSON content: {e.Message}");
            }

            return default;
        }
    }
}