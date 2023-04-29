using System;
using Esri.GameEngine.Geometry;
using Microsoft.Geospatial;

namespace Editor.NetCDF.Types
{
    /// <summary>
    /// Represents a geographic position with latitude and longitude coordinates.
    /// </summary>
    [Serializable]
    public struct Position
    {
        /// <summary>
        /// The latitude of the position in decimal degrees.
        /// </summary>
        public double lat;

        /// <summary>
        /// The longitude of the position in decimal degrees.
        /// </summary>
        public double lon;
            
        
        /// <summary>
        /// Implicit conversion from Position to Bing maps LatLon.
        /// </summary>
        /// <param name="position">The Position instance to convert.</param>
        public static implicit operator LatLon(Position position)
        {
            return new LatLon(position.lat, position.lon);
        }

        
        /// <summary>
        /// Implicit conversion from Position to Bing maps LatLonAlt.
        /// </summary>
        /// <param name="position">The Position instance to convert.</param>
        public static implicit operator LatLonAlt(Position position)
        {
            return new LatLonAlt(position.lat, position.lon, 0.0);
        }

        
        /// <summary>
        /// Implicit conversion from Position to an ArcGISPoint.
        /// </summary>
        /// <param name="position">The Position instance to convert.</param>
        public static implicit operator ArcGISPoint(Position position)
        {
            return new ArcGISPoint(position.lon, position.lat, ArcGISSpatialReference.WGS84());
        }
            
        
        /// <summary>
        /// Calculates a new Position object based on given latitude and longitude offsets (in meters) from an original position.
        /// </summary>
        /// <param name="latOffsetMeters">The latitude offset in meters.</param>
        /// <param name="lonOffsetMeters">The longitude offset in meters.</param>
        /// <param name="originalPosition">The original Position instance to calculate the offset from.</param>
        /// <returns>A new Position object with the calculated latitude and longitude coordinates.</returns>
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
