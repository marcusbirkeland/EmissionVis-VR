using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Editor.NetCDF;
using Editor.NetCDF.Types;
using Editor.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorWindowComponents
{
    /// <summary>
    /// Class for loading netCDF data into Unity and providing the user with an interface for selecting variables.
    /// </summary>
    public class AllVariablesSelector
    {
        public string MapName { get; private set; }
        
        public string BuildingCdfPath => _buildingData.SelectedVariable?.filePath;
        public string WindSpeedCdfPath => _windSpeed.SelectedVariable?.filePath;
        
        public string RadiationCdfPath => _radiationData.SelectedVariables.Count > 0 ? _radiationData.SelectedVariables[0].filePath : string.Empty;
        
        private readonly SingleVariableDropdown _buildingData;
        private readonly SingleVariableDropdown _heightMap;
        private readonly SingleVariableDropdown _windSpeed;
        private readonly MultiVariableDropdown _radiationData;

        private bool _showWarning;

        /// <summary>
        /// Initializes a new instance of the AllVariablesSelector class.
        /// </summary>
        /// <param name="files">A list of netCDF file paths.</param>
        /// <param name="jsonFolderPath">The folder path where JSON files will be saved.</param>
        public AllVariablesSelector(List<string> files, string jsonFolderPath)
        {
            List<NcVariable> allVariables = DataGenerator.GenerateAndLoadVariables(files, jsonFolderPath);
            
            _buildingData = new SingleVariableDropdown(allVariables, "Building data:");
            _heightMap = new SingleVariableDropdown(allVariables, "Heightmap:");
            _windSpeed = new SingleVariableDropdown(allVariables, "Wind speed:");
            _radiationData = new MultiVariableDropdown(allVariables, "Radiation Data:");
        }
        
        /// <summary>
        /// Draws the GUI for the AllVariablesSelector component while the Editor Window is open.
        /// </summary>
        public void Draw()
        {
            GUILayout.Label("Select variables to use", EditorStyles.boldLabel);
            MapName = EditorGUILayout.TextField("Map name:", MapName, GUILayout.Width(400));
                
            _buildingData.Draw();
            _heightMap.Draw();
            _windSpeed.Draw();
            _radiationData.Draw();
            
            // Display warning message if one of the variables isn't set
            if (_showWarning)
            {
                EditorGUILayout.HelpBox("Please select all variables before generating scenes.", MessageType.Warning);
            }
            
            if (GUILayout.Button("Generate scenes", GUILayout.Width(400)))
            {
                CreateScenes();
            }
        }

        /// <summary>
        /// Creates scenes based on the selected variables.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        private void CreateScenes()
        {
            if (!AllVariablesSelected)
            {
                _showWarning = true;
                return;
            }

            if (DataGenerator.CreateDataFiles(
                    MapName,
                    (NcVariable) _buildingData.SelectedVariable,
                    (NcVariable) _heightMap.SelectedVariable,
                    (NcVariable) _windSpeed.SelectedVariable,
                    _radiationData.SelectedVariables))
            {
                ScenesBuilder.CreateBothScenes(this);
            }
        }

        /// <summary>
        /// Checks if all required variables have been selected.
        /// </summary>
        private bool AllVariablesSelected =>
            _buildingData.SelectedVariable != null && _heightMap.SelectedVariable != null &&
             _windSpeed.SelectedVariable != null && _radiationData.SelectedVariables.Count > 0;
    }
}
