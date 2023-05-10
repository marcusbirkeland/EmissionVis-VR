using UnityEngine;

namespace Editor.Utilities
{
    /// <summary>
    /// Static class responsible for holder folder path data used in several files.
    /// </summary>
    public static class FilepathSettings
    {
        /// <summary>
        /// The name of the folder containing all the generated data.
        /// </summary>
        public static string DataFilesFolderName => "MapData";
        
        /// <summary>
        /// The folder containing all the generated datafiles from the netCDF dataset.
        /// Must be a subfolder of the Resources folder.
        /// </summary>
        public static string DatafilesLocation => $"{Application.dataPath}/Resources/{DataFilesFolderName}/";
        
        /// <summary>
        /// The folder containing the python scripts.
        /// </summary>
        public static string PythonFilesLocation => $"{Application.dataPath}/Editor/NetCDF/NetCdfReader/";
    }
}