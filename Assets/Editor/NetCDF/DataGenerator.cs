using System.Collections.Generic;
using System.IO;
using UnityEditor.Scripting.Python;
using UnityEngine;

namespace Editor.NetCDF
{
    public static class DataGenerator
    {
        /**
         * <summary>
         * Generates a JSON file containing variable information for the given NetCDF files and saves it to the specified folder.
         * </summary>
         * <param name="ncFiles">A list of file paths for the NetCDF files to process.</param>
         * <param name="jsonFolderPath">The folder path where the generated JSON file will be saved.</param>
         * <remarks>
         * This method runs a Python script (variable_getter.py) to process the NetCDF files and generate the JSON output.
         * The input format for the Python script is a string consisting of any number of NetCDF file locations followed by an output path for the JSON data.
         * Each value must be separated by a "$" character. The method checks for valid file paths and only processes files with the ".nc" extension.
         * </remarks>
         */
        public static void GenerateVariableJson(List<string> ncFiles, string jsonFolderPath)
        {
            var inputString = "";

            //Adds every valid filepath to the inputstring
            foreach (var file in ncFiles)
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
            
            PythonRunner.RunFile($"{Application.dataPath}/Scripts/NetCdfReader/variable_getter.py", inputString);
        }


        public static void GenerateAllData(NcVariable variable, string dataPath, int interpolationFactor = 1)
        {
            GenerateCsv(variable, dataPath);
            GeneratePng(variable, dataPath, interpolationFactor);
        }

        public static void GenerateCsv(NcVariable variable, string dataPath)
        {
            var inputString = variable + dataPath;
            
            PythonRunner.RunFile($"{Application.dataPath}/Scripts/NetCdfReader/create_csv.py", inputString);
        }

        public static void GeneratePng(NcVariable variable, string dataPath, int interpolationFactor)
        {
            var inputString = variable + dataPath + "$" + interpolationFactor;

            PythonRunner.RunFile($"{Application.dataPath}/Scripts/NetCdfReader/create_png.py", inputString);
        }
    }
}