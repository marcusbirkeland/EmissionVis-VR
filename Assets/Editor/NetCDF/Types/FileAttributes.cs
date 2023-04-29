using System;

namespace Editor.NetCDF.Types
{
    /// <summary>
    /// A struct containing the scope of the dataset.
    /// </summary>
    [Serializable]
    public struct FileAttributes
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
    
    /// <summary>
    /// A struct containing the size of the dataset in X and Y direction.
    /// </summary>
    [Serializable]
    public struct Size
    {
        /// <summary>
        /// The datasets size in meters in the X direction.
        /// </summary>
        public int x;
        
        /// <summary>
        /// The datasets size in meters in the Y direction.
        /// </summary>
        public int y;
    }
}