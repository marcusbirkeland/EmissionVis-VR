using System.Collections.Generic;
using System.IO;
using Editor.NetCDF;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorWindowComponents
{
    /**
     * Class for loading the netCDF data into unity.
     */
    public class AllVariablesSelector
    {
        public delegate void OnDataCompleteHandler();

        public event OnDataCompleteHandler OnDataComplete;
        
        public string MapName { get; private set; }

        public bool DataRetrieved { get; private set; }
        
        public string BuildingCdfPath => _buildingData.SelectedVariable?.filePath;
        public string WindSpeedCdfPath => _windSpeed.SelectedVariable?.filePath;
        public string RadiationCdfPath => _radiationData.SelectedVariables.Count > 0 ? _radiationData.SelectedVariables[0].filePath : null;

        

        private List<FileData> _ncFiles = new();

        private readonly string _jsonFolderPath;
        
        private readonly FileSelector _fileSelector = new();
        
        private SingleVariableDropdown _buildingData;
        private SingleVariableDropdown _heightMap;
        private SingleVariableDropdown _windSpeed;
        private MultiVariableDropdown _radiationData;

        private bool _showWarning;
        
        

        public AllVariablesSelector()
        {
            _jsonFolderPath = $"{Application.dataPath}/Resources/MapData";
        }
        

        /**
         * Event function for drawing the GUI while the editor window is open.
         */
        public void Draw()
        {
            _fileSelector.Draw();
            
            if (GUILayout.Button("Get data", GUILayout.Width(400)))
            {
                GetVariables();
            }
            
            GUILayout.Space(10);

            //Prevents variable selection from appearing before the user has selected a dataset.
            if (_ncFiles.Count <= 0 || _buildingData == null) return;

            GUILayout.Label("Select variables to use", EditorStyles.boldLabel);
            MapName = EditorGUILayout.TextField("Map name:", MapName, GUILayout.Width(400));
                
            _buildingData.Draw();
            _heightMap.Draw();
            _windSpeed.Draw();
            _radiationData.Draw();
            
            // Display warning message if a variable isn't set
            if (_showWarning)
            {
                EditorGUILayout.HelpBox("Please select all variables before loading.", MessageType.Warning);
            }
        }

        public void LoadVariablesButton()
        {
            if (GUILayout.Button("Generate scenes", GUILayout.Width(400)))
            {
                if (_buildingData.SelectedVariable == null || _heightMap.SelectedVariable == null || _windSpeed.SelectedVariable == null || _radiationData.SelectedVariables.Count == 0)
                {
                    _showWarning = true;
                    return;
                }
                _showWarning = false;
                
                DataGenerator.CreateDataFiles(
                    MapName, 
                    (NcVariable) _buildingData.SelectedVariable, 
                    (NcVariable) _heightMap.SelectedVariable, 
                    (NcVariable) _windSpeed.SelectedVariable, 
                    _radiationData.SelectedVariables);
                
                Debug.Log("Finished making data files");
                
                OnDataComplete?.Invoke();
            }
        }


        /**
         * <summary>
         *  Loads every variable from the selected netCDF files into unity.
         *  Then, instantiates the dropdown menus with the list of variables.
         * </summary>
         */
        private void GetVariables()
        {
            DataGenerator.GenerateVariableAndAttributeJson(_fileSelector.NcFiles, _jsonFolderPath);
            
            List<NcVariable> allVariables = LoadVariables();

            _buildingData = new SingleVariableDropdown(allVariables, "Building data:");
            _heightMap = new SingleVariableDropdown(allVariables, "Heightmap:");
            _windSpeed = new SingleVariableDropdown(allVariables, "Wind speed:");
            _radiationData = new MultiVariableDropdown(allVariables, "Radiation Data:");

            DataRetrieved = true;
        }

        
        /**
         * <summary>
         *  Populates the _allVariables field with JSON data created earlier.
         * </summary>
         */
        private List<NcVariable> LoadVariables()
        {
            List<NcVariable> variables = new();

            string path = _jsonFolderPath + "/variables.json";

            string jsonString = File.ReadAllText(path);
            
            jsonString = "{\"fileDataList\":" + jsonString + "}";

            FileDataListWrapper fileDataListWrapper = JsonUtility.FromJson<FileDataListWrapper>(jsonString);
            _ncFiles = fileDataListWrapper.fileDataList;
            
            foreach (FileData fileData in _ncFiles)
            {
                foreach (string variable in fileData.variables)
                {
                    variables.Add(new NcVariable { filePath = fileData.filePath, variableName = variable });
                }
            }

            return variables;
        }
    }
}