using System;
using System.Collections.Generic;

namespace Editor.NetCDF
{
    /**
     *  Contains a netCDF filepath and a list of its variables. 
     */
    [Serializable]
    public struct FileData
    {
        public string filePath;
        public List<string> variables;
    }
}