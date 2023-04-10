using System.Collections.Generic;
using System.IO;
using System.Text;
using Editor.EditorWindowComponents;
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEditor.Scripting.Python;
using UnityEngine;

namespace Editor.NetCDF
{
    public static class DataGenerator
    {
        /**
         * <summary>
         *  Generates and saves the necessary data files and folder structures based on the variables selected by the user.
         * </summary>
         */
        public static void CreateDataFiles(string mapName, SingleVariableDropdown buildingData, SingleVariableDropdown heightMap, SingleVariableDropdown windSpeed, MultiVariableDropdown radiationData)
        {
            if (mapName.IsNullOrWhiteSpace())
            {
                Debug.Log("You need to select a map name");
                return;
            }

            GenerateBuildingData(mapName, buildingData);
            GenerateHeightMap(mapName, heightMap);
            GenerateWindSpeedData(mapName, windSpeed);
            GenerateRadiationData(mapName, radiationData);

            AssetDatabase.Refresh();
        }
        
        
        /**
         * <summary>
         * Generates building data in CSV format for the given NetCDF variable and map name using a Python script.
         * </summary>
         * 
         * <param name="mapName">The name of the map for which the building data is generated.</param>
         * <param name="buildingData">The selected variable for building data generation.</param>
         */
        private static void GenerateBuildingData(string mapName, SingleVariableDropdown buildingData)
        {
            string outputPath = $"{Application.dataPath}/Resources/MapData/{mapName}/BuildingData/buildingData";

            if (buildingData.SelectedVariable != null)
                GenerateBuildingCsv((NcVariable)buildingData.SelectedVariable, outputPath);
        }

        
        /**
         * <summary>
         * Generates height map data in CSV and PNG formats for the given NetCDF variable and map name.
         * </summary>
         * 
         * <param name="mapName">The name of the map for which the height map data is generated.</param>
         * <param name="heightMap">The selected variable for height map data generation.</param>
         */
        private static void GenerateHeightMap(string mapName, SingleVariableDropdown heightMap)
        {
            string outputPath = $"{Application.dataPath}/Resources/MapData/{mapName}/HeightMap/heightMap";

            if (heightMap.SelectedVariable != null)
                GenerateAllData((NcVariable)heightMap.SelectedVariable, outputPath);
        }

        
        /**
         * <summary>
         * Generates wind speed data in PNG format for the given NetCDF variable and map name.
         * </summary>
         * 
         * <param name="mapName">The name of the map for which the wind speed data is generated.</param>
         * <param name="windSpeed">The selected variable for wind speed data generation.</param>
         */
        private static void GenerateWindSpeedData(string mapName, SingleVariableDropdown windSpeed)
        {
            string outputPath = $"{Application.dataPath}/Resources/MapData/{mapName}/WindSpeed";

            if (windSpeed.SelectedVariable != null)
                GeneratePng((NcVariable)windSpeed.SelectedVariable, outputPath, 10);
        }

        
        /**
         * <summary>
         * Generates radiation data in PNG format for each selected variable in the given MultiVariableDropdown and map name.
         * </summary>
         * 
         * <param name="mapName">The name of the map for which the radiation data is generated.</param>
         * <param name="radiationData">The selected variables for radiation data generation.</param>
         */
        private static void GenerateRadiationData(string mapName, MultiVariableDropdown radiationData)
        {
            foreach (NcVariable variable in radiationData.SelectedVariables)
            {
                string folderName = RemoveInvalidFilenameChars(variable.variableName);
                string outputPath = $"{Application.dataPath}/Resources/MapData/{mapName}/Radiation/{folderName}";

                GeneratePng(variable, outputPath, 30);
            }
        }


        /**
         * <summary>
         * Generates a JSON file containing variable information for the given NetCDF files and saves it to the specified folder.
         * Runs Python scripts (variable_getter.py and attribute_getter.py) to process the NetCDF files and generate the JSON output.
         * </summary>
         * 
         * <param name="ncFiles">A list of file paths for the NetCDF files to process.</param>
         * <param name="jsonFolderPath">The folder path where the generated JSON file will be saved.</param>
         * 
         * <remarks>
         * The input format for the Python script is a string consisting of any number of NetCDF file locations followed by an output path for the JSON data.
         * Each value must be separated by a "$" character. The method checks for valid file paths and only processes files with the ".nc" extension.
         * </remarks>
         */
        public static void GenerateVariableAndAttributeJson(List<string> ncFiles, string jsonFolderPath)
        {
            string inputString = "";

            //Adds every valid filepath to the inputString
            foreach (string file in ncFiles)
            {
                if (File.Exists(file) && Path.GetExtension(file).ToLower() == ".nc" && !inputString.Contains(file))
                {
                    inputString += file + "$";
                }
            }

            if (inputString.Length == 0)
            {
                Debug.Log("No valid file paths");
                return;
            }
            
            inputString += jsonFolderPath;
            
            PythonRunner.RunFile($"{Application.dataPath}/Editor/NetCDF/NetCdfReader/variable_getter.py", inputString);
            PythonRunner.RunFile($"{Application.dataPath}/Editor/NetCDF/NetCdfReader/attribute_getter.py", inputString);
        }


        /**
         * <summary>
         * Generates both CSV and PNG files for the given NetCDF variable.
         * </summary>
         * 
         * <param name="variable">The NetCDF variable used to generate the data files.</param>
         * <param name="dataPath">The folder path where the generated files will be saved. Should not end in a "/"</param>
         * <param name="interpolationFactor">The interpolation factor used when generating PNG files (default is 1).</param>
         */
        private static void GenerateAllData(NcVariable variable, string dataPath, int interpolationFactor = 1)
        {
            GenerateCsv(variable, dataPath);
            GeneratePng(variable, dataPath, interpolationFactor);
        }

        
        /**
         * <summary>
         * Generates one or several CSV files for the given NetCDF variable using a Python script.
         * </summary>
         * 
         * <param name="variable">The NetCDF variable used to generate the CSV file(s).</param>
         * <param name="dataPath">The folder path where the generated CSV file will be saved. Should not end in a "/"</param>
         */
        private static void GenerateCsv(NcVariable variable, string dataPath)
        {
            string inputString = variable + dataPath;
            
            PythonRunner.RunFile($"{Application.dataPath}/Editor/NetCDF/NetCdfReader/create_csv.py", inputString);
        }
        
        
        /**
         * <summary>
         * Generates a building CSV file using a Python script to process the NetCDF data.
         * </summary>
         * 
         * <param name="variable">The NetCDF variable containing the building data.</param>
         * <param name="dataPath">The folder path where the generated PNG file will be saved. Should not end in a "/"</param>
         */
        private static void GenerateBuildingCsv(NcVariable variable, string dataPath)
        {
            string inputString = variable + dataPath;
            
            PythonRunner.RunFile($"{Application.dataPath}/Editor/NetCDF/NetCdfReader/create_building_csv.py", inputString);
        }

        
        /**
         * <summary>
         * Generates one or several PNG files for the given NetCDF variable.
         * Uses the specified interpolation factor to smooth the image.
         * </summary>
         * 
         * <param name="variable">The NetCDF variable used to generate the PNG file(s).</param>
         * <param name="dataPath">The folder path where the generated PNG file will be saved. Should not end in a "/"</param>
         * <param name="interpolationFactor">The interpolation factor used when generating the PNG file.</param>
         */
        private static void GeneratePng(NcVariable variable, string dataPath, int interpolationFactor)
        {
            string inputString = variable + dataPath + "$" + interpolationFactor;

            PythonRunner.RunFile($"{Application.dataPath}/Editor/NetCDF/NetCdfReader/create_png.py", inputString);
        }


        /**
         * <summary>
         * Removes every character from a string that cannot be used as a fileName on the current operating system.
         * </summary>
         *
         * <param name="input">The string to modify</param>
         *
         * <returns> A modified string without invalid characters.</returns>
         */
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