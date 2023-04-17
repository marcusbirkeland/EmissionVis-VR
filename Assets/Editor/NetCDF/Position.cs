using System;
using Esri.GameEngine.Geometry;
using Microsoft.Geospatial;

namespace Editor.NetCDF
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
            return new ArcGISPoint(position.lon, position.lat, ArcGISSpatialReference.WGS84());
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
}