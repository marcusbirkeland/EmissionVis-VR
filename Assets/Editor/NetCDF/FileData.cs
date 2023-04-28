using System;
using System.Collections.Generic;

namespace Editor.NetCDF
{
    /// <summary>
    /// Contains a netCDF filepath and a list of its variables.
    /// </summary> 
    [Serializable]
    public struct FileData
    {
        public string filePath;
        public List<string> variables;
    }
}