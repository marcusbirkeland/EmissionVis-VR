using System;

namespace Editor.NetCDF.Types
{
    /// <summary>
    /// Struct containing the positional data for a building.
    /// </summary>
    [Serializable]
    public readonly struct BuildingData
    {
        /// <summary>
        /// A number of meters in the X direction relative to the start position.
        /// </summary>
        /// <remarks>
        /// The X direction is usually east, but it can get offset by the rotation angle.
        /// </remarks>
        public readonly double X;

        /// <summary>
        /// A number of meters in the Y direction relative to the start position.
        /// </summary>
        /// <remarks>
        /// The Y direction is usually north, but it can get offset by the rotation angle.
        /// </remarks>
        public readonly double Y;

        /// <summary>
        /// The buildings altitude described by meters above sea level.
        /// </summary>
        public readonly double Altitude;

        
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingData"/> struct with the specified X, Y, and altitude values.
        /// </summary>
        /// <param name="x">The <see cref="X"/> coordinate of the building.</param>
        /// <param name="y">The <see cref="Y"/> coordinate of the building.</param>
        /// <param name="altitude">The <see cref="Altitude"/> of the building.</param>
        public BuildingData(double x, double y, double altitude)
        {
            X = x;
            Y = y;
            Altitude = altitude;
        }
    }
}