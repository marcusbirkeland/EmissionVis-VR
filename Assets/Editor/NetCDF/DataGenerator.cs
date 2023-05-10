using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Editor.NetCDF.Types;
using Editor.Utilities;
using UnityEditor;
using UnityEditor.Scripting.Python;
using UnityEngine;

namespace Editor.NetCDF
{
    /// <summary>
    /// Static class responsible for using the python scripts to generate datafiles usable in a unity context from
    /// netCDF data.
    /// </summary>
    public static class DataGenerator
    {
        /// <summary>
        /// Generates and loads the Variables and Scopes JSON files based on the input data.
        /// </summary>
        /// <param name="netCdfFilePaths">A list of paths containing the NetCDF files to process.</param>
        /// <returns>A list of all <see cref="NcVariable"/> types contained in the dataset.</returns>
        public static List<NcVariable> GenerateAndLoadJsonFiles(List<string> netCdfFilePaths)
        {
            if (netCdfFilePaths.Count < 1)
            {
                Debug.LogError("No valid netCDF files selected!");
                return default;
            }
            
            try
            {
                GenerateVariableAndScopeJson(netCdfFilePaths);
            
                List<NcVariable> variables = new();

                //Gets the entire json file content wrapped inside a fileDataList wrapper.
                //NOTE: the fileDataList string must have the same name as the only field in the FileDataListWrapper.
                string jsonString = File.ReadAllText(FilepathSettings.DatafilesLocation + "variables.json");
                jsonString = "{\"fileDataList\":" + jsonString + "}";

                FileDataListWrapper fileDataListWrapper = JsonUtility.FromJson<FileDataListWrapper>(jsonString);
                List<FileData> ncFiles = fileDataListWrapper.fileDataList;
                
                foreach (FileData fileData in ncFiles)
                {
                    foreach (string variable in fileData.variables)
                    {
                        variables.Add(new NcVariable(variable, fileData.filePath));
                    }
                }
                
                return variables;
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                Console.WriteLine(e);
                throw;
            }
        }


        /// <summary>
        /// Generates and saves the necessary data files and folder structures based on the variables selected by the user.
        /// This method takes an NcDataset object as input and creates the required data files for buildings, terrain,
        /// clouds, and radiation data. It also updates the progress bar during the data creation process.
        /// </summary>
        /// <param name="data">
        /// The <see cref="NcDataset"/> type containing all the user selected data, including map name,
        /// building data, height map, wind speed, and radiation data.
        /// </param>
        /// <returns>
        /// Returns true if the data files and folder structures were created successfully, and false otherwise.
        /// If there was an issue during data creation, a warning dialog is displayed to inform the user.
        /// </returns>
        public static bool CreateDataFiles(NcDataset data)
        {
            EditorUtility.DisplayProgressBar("Creating datafiles", "Creating buildings datafiles", -1);
            GenerateBuildingData(data.MapName, data.BuildingData, data.HeightMap);
            
            EditorUtility.DisplayProgressBar("Creating datafiles", "Creating terrain datafiles", -1);
            GenerateHeightMap(data.MapName, data.HeightMap);
            
            EditorUtility.DisplayProgressBar("Creating datafiles", "Creating cloud datafiles", -1);
            GenerateWindSpeedData(data.MapName, data.WindSpeed);
            
            EditorUtility.DisplayProgressBar("Creating datafiles", "Creating radiation datafiles", -1);
            GenerateRadiationData(data.MapName, data.RadiationData);
            
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            
            if (DataFoldersExist(data.MapName)) return true;
            
            EditorUtility.DisplayDialog("Warning", "Something went wrong during data creation. Please make sure you selected the correct variables.", "OK");
            return false;
        }


        /// <summary>
        /// <para>Generates two JSON files (scopes.json and variables.json) for easier access to NetCDF data within Unity.</para>
        /// <para>The method runs the Python scripts (variable_getter.py and scopes_getter.py) to process the NetCDF files and generate the JSON files. If the JSON files already exist, they are overwritten.</para>
        /// <para>The scopes.json file contains positional data for every selected NetCDF file, while the variables.json file contains a list of all variables within each NetCDF file.</para>
        /// </summary>
        /// <remarks>
        /// The input format for both Python scripts is a string consisting of any number of NetCDF file locations followed by an output path for the JSON data. Each path must be separated by a "$" character.
        /// </remarks>
        /// <param name="netCdfFilePaths">A list of file paths for the NetCDF files to process.</param>
        private static void GenerateVariableAndScopeJson(List<string> netCdfFilePaths)
        {
            StringBuilder pythonInputStringBuilder = new();

            foreach (string file in netCdfFilePaths)
            {
                if (!pythonInputStringBuilder.ToString().Contains(file))
                {
                    pythonInputStringBuilder.Append(file).Append('$');
                }
            }

            pythonInputStringBuilder.Append(FilepathSettings.DatafilesLocation);
            
            PythonRunner.RunFile(FilepathSettings.PythonFilesLocation + "variable_getter.py", pythonInputStringBuilder.ToString());
            PythonRunner.RunFile(FilepathSettings.PythonFilesLocation + "scopes_getter.py", pythonInputStringBuilder.ToString());
            
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
            string outputPath = FilepathSettings.DatafilesLocation + $"{mapName}/BuildingData/buildingData";
            
            string pythonInputString = buildingsVariable + terrainHeightVariable + outputPath;

            PythonRunner.RunFile(FilepathSettings.PythonFilesLocation + "create_building_csv.py", pythonInputString);
        }


        /// <summary>
        /// Generates height map data for the given NetCDF variable and map name as a greyscale PNG.
        /// </summary>
        /// <param name="mapName">The name of the map for which the height map data is generated.</param>
        /// <param name="terrainHeightVariable">The selected variable for height map data generation.</param>
        /// <param name="interpolationFactor">The interpolation factor for generating height map data (optional, default value is 10).</param>
        private static void GenerateHeightMap(string mapName, NcVariable terrainHeightVariable, int interpolationFactor = 10)
        {
            string outputPath = FilepathSettings.DatafilesLocation + $"{mapName}/HeightMap/heightMap";
            
            string pythonInputString = terrainHeightVariable + outputPath + "$" + interpolationFactor;

            PythonRunner.RunFile(FilepathSettings.PythonFilesLocation + "create_2d_png.py", pythonInputString);
        }
        

        /// <summary>
        /// Generates wind speed data in PNG format for the given NetCDF variable and map name.
        /// </summary>
        /// <param name="mapName">The name of the map for which the wind speed data is generated.</param>
        /// <param name="windSpeedVariable">The selected variable for wind speed data generation.</param>
        /// <param name="interpolationFactor">The interpolation factor for generating wind speed data (optional, default value is 10).</param>
        private static void GenerateWindSpeedData(string mapName, NcVariable windSpeedVariable, int interpolationFactor = 10)
        {
            string outputPath = FilepathSettings.DatafilesLocation + $"{mapName}/WindSpeed";

            string pythonInputString = windSpeedVariable + outputPath + "$" + interpolationFactor;

            PythonRunner.RunFile(FilepathSettings.PythonFilesLocation + "create_4d_pngs.py", pythonInputString);
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
                string folderName = RemoveInvalidFilenameChars(variable.VariableName);
                string outputPath = FilepathSettings.DatafilesLocation + $"{mapName}/Radiation/{folderName}";

                string pythonInputString = variable + outputPath + "$" + interpolationFactor;

                PythonRunner.RunFile(FilepathSettings.PythonFilesLocation + "create_4d_pngs.py", pythonInputString);
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
        
        
        /// <summary>
        /// Checks whether the folders generated by the methods above exist.
        /// </summary>
        /// <param name="mapName">The the name of the map to check</param>
        /// <returns>True if the files exist. False otherwise.</returns>
        /// <remarks>
        /// This is a hacky workaround, and should be replaced in the future with a better way of checking if the python
        /// files executed without problems.
        /// </remarks>
        private static bool DataFoldersExist(string mapName)
        {
            string basePath = FilepathSettings.DatafilesLocation + $"{mapName}";

            string[] requiredFolders =
            {
                "BuildingData",
                "HeightMap",
                "Radiation",
                "WindSpeed"
            };

            return requiredFolders.Select(folder => Path.Combine(basePath, folder)).All(Directory.Exists);
        }
        
        
        /// <summary>
        /// Provides a serializable object for JSON deserialization.
        /// Contains a list of <see cref="FileData"/> objects.
        /// </summary>
        /// <remarks>
        /// Dont rename the <see cref="fileDataList"/> field. Its name is necessary for correct JSON deserialization.
        /// </remarks>
        [Serializable]
        private struct FileDataListWrapper
        {
            /// <summary>
            /// A list of <see cref="FileData"/> objects.
            /// </summary>
            public List<FileData> fileDataList;
        }
    }
}