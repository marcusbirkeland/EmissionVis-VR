using System;

namespace Editor.NetCDF
{
    /// <summary>
    /// Struct containing the positional data for a building.
    /// </summary>
    [Serializable]
    public readonly struct BuildingData
    {
        /// <summary>
        /// A number og meters in the X direction relative to the start position.
        /// </summary>
        public readonly double X;

        /// <summary>
        /// A number og meters in the Y direction relative to the start position.
        /// </summary>
        public readonly double Y;

        /// <summary>
        /// The buildings altitude above water in meters.
        /// </summary>
        public readonly double Altitude;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingData"/> struct with the specified X, Y, and altitude values.
        /// </summary>
        /// <param name="x">The X coordinate of the building.</param>
        /// <param name="y">The Y coordinate of the building.</param>
        /// <param name="altitude">The altitude of the building.</param>
        public BuildingData(double x, double y, double altitude)
        {
            X = x;
            Y = y;
            Altitude = altitude;
        }
    }
}