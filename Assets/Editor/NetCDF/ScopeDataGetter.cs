using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor.NetCDF.Types;
using UnityEngine;

namespace Editor.NetCDF
{
    /// <summary>
    /// Static class responsible for getting values from the scopes.json file.
    /// </summary>
    public static class ScopeDataGetter
    {
        /// <summary>
        /// Retrieves the <see cref="DatasetScope"/> associated with a specified NetCDF file.
        /// </summary>
        /// <remarks>
        /// This method reads a JSON file containing file attribute information for multiple NetCDF files.
        /// It then attempts to find and return the <see cref="DatasetScope"/> for the specified NetCDF file path.
        /// If the JSON file is not found, or if there's an error in parsing its content, an exception is thrown.
        /// </remarks>
        /// <param name="cdfFilePath">The path of the NetCDF file for which attributes are to be retrieved.</param>
        /// <returns>A <see cref="DatasetScope"/> struct containing that netCDF files geographical scope.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the JSON file containing scope information is not found.</exception>
        /// <exception cref="ArgumentException">Thrown when there's an error in parsing the JSON content.</exception>
        /// <exception cref="InvalidOperationException">Thrown when there's an error in parsing the JSON content.</exception>
        public static DatasetScope GetDatasetScope(string cdfFilePath)
        {
            string jsonFilePath = FilepathSettings.DatafilesLocation + "scopes.json";

            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                DatasetScopeWrapper fileList =
                    JsonUtility.FromJson<DatasetScopeWrapper>("{\"data\":" + jsonContent + "}");

                return fileList.data.FirstOrDefault(data => data.filePath == cdfFilePath);
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError($"JSON file not found: {e.Message}");
                throw;            
            }
            catch (Exception e) when (e is ArgumentException or InvalidOperationException)
            {
                Debug.LogError($"Error parsing JSON content: {e.Message}");
                throw;
            }
        }
        
        
        /// <summary>
        /// Gets the center position of a specified NetCDF files dataset.
        /// </summary>
        /// <param name="cdfFilePath">The path of the NetCDF file.</param>
        /// <returns>
        /// A <see cref="Position"/> object with the calculated latitude and longitude coordinates,
        /// or a default position if not found.
        /// </returns>
        public static Position GetCenterPosition(string cdfFilePath)
        {
            DatasetScope datasetScope = GetDatasetScope(cdfFilePath);
            if (datasetScope.filePath == null)
            {
                Debug.LogError("File attributes not found.");
                return default;
            }

            return Position.GetOffsetPosition(
                (double) datasetScope.size.x / 2, (double) datasetScope.size.y / 2, datasetScope.position);
        }
        
        
        /// <summary>
        /// Local wrapper struct used for JSON deserialization.
        /// </summary>
        /// <remarks>
        /// Dont rename the <see cref="data"/> field, as name is necessary for correct JSON deserialization.
        /// </remarks>
        [Serializable]
        private struct DatasetScopeWrapper
        {
            public List<DatasetScope> data;
        }
    }
}