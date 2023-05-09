using System;

namespace Editor.NetCDF.Types
{
    /// <summary>
    /// A struct containing the size of the dataset in X and Y direction.
    /// Used in Json deserialization.
    /// </summary>
    [Serializable]
    public struct Size
    {
        /// <summary>
        /// The datasets size in meters in the X direction.
        /// </summary>
        /// <remarks>
        /// The X direction usually corresponds to the east.
        /// </remarks>
        public int x;

        /// <summary>
        /// The datasets size in meters in the Y direction.
        /// </summary>
        /// <remarks>
        /// The X direction usually corresponds to the east.
        /// </remarks>
        public int y;
    }
}