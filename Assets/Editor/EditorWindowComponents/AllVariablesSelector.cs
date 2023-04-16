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
        
        private readonly FileSelector _fileSelector = new();

        private List<FileData> _ncFiles = new();

        private readonly string _jsonFolderPath;
        public string MapName { get; private set; }
        
        private readonly List<NcVariable> _allVariables = new();
        
        private SingleVariableDropdown _buildingData;
        private SingleVariableDropdown _heightMap;
        private SingleVariableDropdown _windSpeed;
        private MultiVariableDropdown _radiationData;
        
        
        public bool DataRetrieved { get; private set; }


        public AllVariablesSelector()
        {
            _jsonFolderPath = $"{Application.dataPath}/Resources/MapData";
        }

        public string BuildingCdfPath => _buildingData.SelectedVariable?.filePath;
        public string WindSpeedCdfPath => _windSpeed.SelectedVariable?.filePath;
        public string RadiationCdfPath => _radiationData.SelectedVariables.Count > 0 ? _radiationData.SelectedVariables[0].filePath : null;


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
        }

        public void LoadVariablesButton()
        {
            if (GUILayout.Button("Load variables", GUILayout.Width(400)))
            {
                DataGenerator.CreateDataFiles(MapName, _buildingData, _heightMap, _windSpeed, _radiationData);
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
            AssetDatabase.Refresh();
            LoadVariables();
            
            _buildingData = new SingleVariableDropdown(_allVariables, "Building data:");
            _heightMap = new SingleVariableDropdown(_allVariables, "Heightmap:");
            _windSpeed = new SingleVariableDropdown(_allVariables, "Wind speed:");
            _radiationData = new MultiVariableDropdown(_allVariables, "Radiation Data:");

            DataRetrieved = true;
        }

        
        /**
         * <summary>
         *  Populates the _allVariables field with JSON data created earlier.
         * </summary>
         */
        private void LoadVariables()
        {
            string path = _jsonFolderPath + "/variables.json";

            string jsonString = File.ReadAllText(path);
            
            //A bit of a roundabout way of doing things but the deserializer can only create a single instance of an object.
            //It doesn't work with several netCDF files without adding the fileDataList key and wrapper.
            jsonString = "{\"fileDataList\":" + jsonString + "}";

            FileDataListWrapper fileDataListWrapper = JsonUtility.FromJson<FileDataListWrapper>(jsonString);
            _ncFiles = fileDataListWrapper.fileDataList;
            
            _allVariables.Clear();

            foreach (FileData fileData in _ncFiles)
            {
                foreach (string variable in fileData.variables)
                {
                    _allVariables.Add(new NcVariable { filePath = fileData.filePath, variableName = variable });
                }
            }
        }
    }
}