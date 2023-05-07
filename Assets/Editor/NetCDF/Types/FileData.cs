using System;
using System.Collections.Generic;

namespace Editor.NetCDF.Types
{
    /// <summary>
    /// Contains a netCDF filepath and a list of its variables.
    /// </summary> 
    [Serializable]
    public struct FileData
    {
        /// <summary>
        /// The complete filepath to the netcdf file.
        /// </summary>
        public string filePath;
        
        /// <summary>
        /// A list of all available variables contained in the netCDF file.
        /// </summary>
        public List<string> variables;
    }
}