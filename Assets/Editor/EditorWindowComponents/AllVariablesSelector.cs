using System.Collections.Generic;
using Editor.NetCDF;
using Editor.NetCDF.Types;
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorWindowComponents
{
    /// <summary>
    /// Class responsible for letting the user select a dataset from the chosen netCDF files.
    /// </summary>
    public class AllVariablesSelector
    {
        /// <summary>
        /// Indicates whether the warning message should get displayed.
        /// </summary>
        public bool ShowWarning;

        private string _mapName;
        private readonly SingleVariableDropdown _buildingData;
        private readonly SingleVariableDropdown _heightMap;
        private readonly SingleVariableDropdown _windSpeed;
        private readonly MultiVariableDropdown _radiationData;

        /// <summary>
        /// Gets the selected dataset based on the user's chosen variables.
        /// </summary>
        /// <remarks>
        /// If all required variables have been selected, it returns an instance of the <see cref="NcDataset"/> struct
        /// containing the selected data. Otherwise, it returns null.
        /// </remarks>
        public NcDataset? SelectedDataset
        {
            get
            {
                if (AllVariablesSelected)
                    return new NcDataset(
                        _mapName,
                        _buildingData.SelectedVariable.Value,
                        _heightMap.SelectedVariable.Value,
                        _windSpeed.SelectedVariable.Value,
                        _radiationData.SelectedVariables);

                return null;
            }
        }
        
        /// <summary>
        /// Checks if all required variables have been selected.
        /// </summary>
        private bool AllVariablesSelected =>
            !_mapName.IsNullOrWhiteSpace() &&
            _buildingData.SelectedVariable != null && _heightMap.SelectedVariable != null &&
            _windSpeed.SelectedVariable != null && _radiationData.SelectedVariables.Count > 0;

        
        /// <summary>
        /// Initializes a new instance of the AllVariablesSelector class.
        /// </summary>
        /// <param name="files">A list of netCDF file paths.</param>
        /// <remarks>
        /// Generates the "scopes.json" and "variables.json" files.
        /// The files contents are used to generate the available options for the dropdown menus.
        /// </remarks>
        public AllVariablesSelector(List<string> files)
        {
            List<NcVariable> allVariables = DataGenerator.GenerateAndLoadJsonFiles(files);
            
            _buildingData = new SingleVariableDropdown(allVariables, "Building data:");
            _heightMap = new SingleVariableDropdown(allVariables, "Heightmap:");
            _windSpeed = new SingleVariableDropdown(allVariables, "Wind speed:");
            _radiationData = new MultiVariableDropdown(allVariables, "Radiation Data:");
        }
        
        
        /// <summary>
        /// Draws the GUI for the AllVariablesSelector component.
        /// </summary>
        /// <remarks>
        /// Only works inside an editor window.
        /// </remarks>
        public void Draw()
        {
            GUILayout.Label("Select variables to use", EditorStyles.boldLabel);
            _mapName = EditorGUILayout.TextField("Map name:", _mapName, GUILayout.Width(400));
                
            _buildingData.Draw();
            _heightMap.Draw();
            _windSpeed.Draw();
            _radiationData.Draw();
            
            if (ShowWarning)
            {
                EditorGUILayout.HelpBox("Please select all variables before generating scenes.", MessageType.Warning);
            }
        }
    }
}
