using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Editor.NetCDF
{
    /// <summary>
    /// Provides functionality to get attribute data from NetCDF files.
    /// </summary>
    public static class AttributeDataGetter
    {
        [Serializable]
        public struct Size
        {
            public int x;
            public int y;
        }
        

        [Serializable]
        public struct FileAttributes
        {
            public string filePath;
            public Position position;
            public Size size;
        }
        

        [Serializable]
        private struct FileAttributeListWrapper
        {
            public List<FileAttributes> data;
        }

        
        /// <summary>
        /// Gets the file attributes of a specified NetCDF file.
        /// </summary>
        /// <param name="cdfFilePath">The path of the NetCDF file.</param>
        /// <returns>A FileAttributes struct containing file attribute information, or a default struct if not found.</returns>
        public static FileAttributes GetFileAttributes(string cdfFilePath)
        {
            string jsonFilePath = $"{Application.dataPath}/Resources/MapData/attributes.json";
            
            if (!File.Exists(jsonFilePath))
            {
                Debug.LogError("JSON file not found.");
                return default;
            }

            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                FileAttributeListWrapper fileList = JsonUtility.FromJson<FileAttributeListWrapper>("{\"data\":" + jsonContent + "}");

                return fileList.data.FirstOrDefault(data => data.filePath == cdfFilePath);
            }
            catch (Exception e) when (e is ArgumentException or InvalidOperationException)
            {
                Debug.LogError($"Error parsing JSON content: {e.Message}");
            }

            return default;
        }
    }
}