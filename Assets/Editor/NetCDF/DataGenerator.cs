using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Editor.NetCDF.Types;
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEditor.Scripting.Python;
using UnityEngine;

namespace Editor.NetCDF
{
    public static class DataGenerator
    {
        /// <summary>
        /// Provides a serializable object for JSON deserialization.
        /// Contains a list of <see cref="FileData"/> objects.
        /// </summary>
        [Serializable]
        private struct FileDataListWrapper
        {
            /// <summary>
            /// A list of <see cref="FileData"/> objects.
            /// </summary>
            public List<FileData> fileDataList;
        }
        
        
        /// <summary>
        /// Generates and loads the Variable and Attribute JSON files based on the input data.
        /// </summary>
        /// <param name="netCdfFilePaths">A list of file paths for the NetCDF files to process.</param>
        /// <param name="jsonFolderPath">The folder path where the generated JSON files will be saved.</param>
        /// <returns>A list of <see cref="NcVariable"/> objects containing the data.</returns>
        public static List<NcVariable> GenerateAndLoadVariables(List<string> netCdfFilePaths, string jsonFolderPath)
        {
            GenerateVariableAndAttributeJson(netCdfFilePaths, jsonFolderPath);
            
            List<NcVariable> variables = new();

            string path = jsonFolderPath + "/variables.json";

            string jsonString = File.ReadAllText(path);

            jsonString = "{\"fileDataList\":" + jsonString + "}";

            FileDataListWrapper fileDataListWrapper = JsonUtility.FromJson<FileDataListWrapper>(jsonString);
            List<FileData> ncFiles = fileDataListWrapper.fileDataList;

            foreach (FileData fileData in ncFiles)
            {
                foreach (string variable in fileData.variables)
                {
                    variables.Add(new NcVariable { filePath = fileData.filePath, variableName = variable });
                }
            }
            return variables;        
        }
        
        /// <summary>
        /// Generates and saves the necessary data files and folder structures based on the variables selected by the user.
        /// </summary>
        /// <param name="mapName">The name of the map for which the data files are generated.</param>
        /// <param name="buildingsVariable">The selected variable for building data generation.</param>
        /// <param name="terrainHeightVariable">The selected variable for height map data generation.</param>
        /// <param name="windSpeedVariable">The selected variable for wind speed data generation.</param>
        /// <param name="radiationVariables">The list of selected variables for radiation data generation.</param>
        public static void CreateDataFiles(string mapName, NcVariable buildingsVariable, NcVariable terrainHeightVariable, NcVariable windSpeedVariable, List<NcVariable> radiationVariables)
        {
            if (mapName.IsNullOrWhiteSpace())
            {
                Debug.Log("You need to select a map name");
                return;
            }
            
            EditorUtility.DisplayProgressBar("Creating datafiles", "Creating buildings datafiles", -1);
            GenerateBuildingData(mapName, buildingsVariable, terrainHeightVariable);
            
            EditorUtility.DisplayProgressBar("Creating datafiles", "Creating terrain datafiles", -1);
            GenerateHeightMap(mapName, terrainHeightVariable);
            
            EditorUtility.DisplayProgressBar("Creating datafiles", "Creating cloud datafiles", -1);
            GenerateWindSpeedData(mapName, windSpeedVariable);
            
            EditorUtility.DisplayProgressBar("Creating datafiles", "Creating radiation datafiles", -1);
            GenerateRadiationData(mapName, radiationVariables);
            
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        
        /// <summary>
        /// <para>Generates two JSON files (attributes.json and variables.json) for easier access to NetCDF data within Unity.</para>
        /// <para>The method runs Python scripts (variable_getter.py and attribute_getter.py) to process the NetCDF files and generate the JSON files, which are saved at the specified outputFolderPath. If the output path doesn't exist, it is created. If the JSON files already exist, they are overwritten.</para>
        /// <para>The attributes.json file contains positional data for every selected NetCDF file, while the variables.json file contains a list of all variables within each NetCDF file.</para>
        /// </summary>
        /// <remarks>
        /// The input format for both Python scripts is a string consisting of any number of NetCDF file locations followed by an output path for the JSON data. Each path must be separated by a "$" character. The method checks for valid file paths and only processes files with the ".nc" extension.
        /// </remarks>
        /// <param name="netCdfFilePaths">A list of file paths for the NetCDF files to process.</param>
        /// <param name="outputFolderPath">The folder path where the generated JSON files will be saved.</param>
        private static void GenerateVariableAndAttributeJson(List<string> netCdfFilePaths, string outputFolderPath)
        {
            StringBuilder inputStringBuilder = new();

            foreach (string file in netCdfFilePaths)
            {
                if (File.Exists(file) && Path.GetExtension(file).ToLower() == ".nc" && !inputStringBuilder.ToString().Contains(file))
                {
                    inputStringBuilder.Append(file).Append('$');
                }
            }
            
            string pythonInputString = inputStringBuilder.ToString();

            if (pythonInputString.IsNullOrEmpty())
            {
                Debug.Log("No valid file paths");
                return;
            }
            
            pythonInputString += outputFolderPath;
            
            
            //Creates variable JSON
            PythonRunner.RunFile($"{Application.dataPath}/Editor/NetCDF/NetCdfReader/variable_getter.py", pythonInputString);
            
            //Creates attribute JSON
            PythonRunner.RunFile($"{Application.dataPath}/Editor/NetCDF/NetCdfReader/attribute_getter.py", pythonInputString);
            
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Generates building data in CSV format used to spawn buildings on the map.
        /// </summary>
        /// <param name="mapName">The name of the map for which the building data is generated.</param>
        /// <param name="buildingsVariable">The data containing building positions.</param>
        /// <param name="terrainHeightVariable">The terrain height data.</param>
        private static void GenerateBuildingData(string mapName, NcVariable buildingsVariable, NcVariable terrainHeightVariable)
        {
            string outputPath = $"{Application.dataPath}/Resources/MapData/{mapName}/BuildingData/buildingData";
            
            string pythonInputString = buildingsVariable + terrainHeightVariable + outputPath;

            PythonRunner.RunFile($"{Application.dataPath}/Editor/NetCDF/NetCdfReader/create_building_csv.py", pythonInputString);
        }


        /// <summary>
        /// Generates height map data in CSV and PNG formats for the given NetCDF variable and map name.
        /// </summary>
        /// <param name="mapName">The name of the map for which the height map data is generated.</param>
        /// <param name="terrainHeightVariable">The selected variable for height map data generation.</param>
        /// <param name="interpolationFactor">The interpolation factor for generating height map data (optional, default value is 10).</param>
        private static void GenerateHeightMap(string mapName, NcVariable terrainHeightVariable, int interpolationFactor = 10)
        {
            string outputPath = $"{Application.dataPath}/Resources/MapData/{mapName}/HeightMap/heightMap";
            
            string pythonInputString = terrainHeightVariable + outputPath + "$" + interpolationFactor;

            PythonRunner.RunFile($"{Application.dataPath}/Editor/NetCDF/NetCdfReader/create_png.py", pythonInputString);
        }
        

        /// <summary>
        /// Generates wind speed data in PNG format for the given NetCDF variable and map name.
        /// </summary>
        /// <param name="mapName">The name of the map for which the wind speed data is generated.</param>
        /// <param name="windSpeedVariable">The selected variable for wind speed data generation.</param>
        /// <param name="interpolationFactor">The interpolation factor for generating wind speed data (optional, default value is 10).</param>
        private static void GenerateWindSpeedData(string mapName, NcVariable windSpeedVariable, int interpolationFactor = 10)
        {
            string outputPath = $"{Application.dataPath}/Resources/MapData/{mapName}/WindSpeed";

            string pythonInputString = windSpeedVariable + outputPath + "$" + interpolationFactor;

            PythonRunner.RunFile($"{Application.dataPath}/Editor/NetCDF/NetCdfReader/create_png.py", pythonInputString);
        }


        /// <summary>
        /// Generates radiation data in PNG format for each selected variable in the given MultiVariableDropdown and map name.
        /// </summary>
        /// <param name="mapName">The name of the map for which the radiation data is generated.</param>
        /// <param name="radiationDataVariables">The selected variables for radiation data generation.</param>
        /// <param name="interpolationFactor">The interpolation factor for generating radiation data (optional, default value is 10).</param>
        private static void GenerateRadiationData(string mapName, List<NcVariable> radiationDataVariables, int interpolationFactor = 10)
        {
            foreach (NcVariable variable in radiationDataVariables)
            {
                string folderName = RemoveInvalidFilenameChars(variable.variableName);
                string outputPath = $"{Application.dataPath}/Resources/MapData/{mapName}/Radiation/{folderName}";

                string pythonInputString = variable + outputPath + "$" + interpolationFactor;

                PythonRunner.RunFile($"{Application.dataPath}/Editor/NetCDF/NetCdfReader/create_png.py", pythonInputString);
            }
        }


        /// <summary>
        /// Removes every character from a string that cannot be used as a fileName on the current operating system.
        /// </summary>
        /// <param name="input">The string to modify.</param>
        /// <returns> A modified string without invalid characters.</returns>
        private static string RemoveInvalidFilenameChars(string input)
        {
            List<char> invalidChars = new(Path.GetInvalidFileNameChars());
            StringBuilder result = new();

            foreach (char c in input)
            {
                if (invalidChars.Contains(c)) continue;
                
                result.Append(c);
            }

            return result.ToString();
        }
    }
}