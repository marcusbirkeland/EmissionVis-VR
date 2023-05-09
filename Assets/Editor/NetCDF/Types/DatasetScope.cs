using System;

namespace Editor.NetCDF.Types
{
    /// <summary>
    /// A struct containing the scope of the dataset.
    /// Used in JSON deserialization.
    /// </summary>
    [Serializable]
    public struct DatasetScope
    {
        /// <summary>
        /// The complete local path to the netCDF file.
        /// </summary>
        public string filePath;

        /// <summary>
        /// The origin position for the dataset.
        /// </summary>
        public Position position;

        /// <summary>
        /// The size of the dataset.
        /// </summary>
        public Size size;
    }
}