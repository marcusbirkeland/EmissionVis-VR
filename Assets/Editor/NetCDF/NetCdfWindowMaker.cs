using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor.NetCDF
{
    /**
     * The main class for the unity extension. Creates the editor window and loads the data. 
     */
    public class NetCdfWindowMaker : EditorWindow
    {
        private readonly FileSelector _fileSelector = new();

        private List<FileData> _ncFiles = new();

        private string _jsonFolderPath;
        private string _mapName;
        
        private readonly List<NcVariable> _allVariables = new();
        
        private SingleVariableDropdown _buildingData;
        private SingleVariableDropdown _heightMap;
        private SingleVariableDropdown _windSpeed;
        private MultiVariableDropdown _radiationData;

        
        /**
         * <summary>
         *  Sets the file location for the JSON data.
         * </summary>
         */
        private void OnEnable()
        {
            _jsonFolderPath = $"{Application.dataPath}/Resources/MapData";
        }

        
        /**
         * <summary>
         *  Creates a menu button to open the editor window.
         * </summary>
         */
        [MenuItem("NetCDF/Open Window")]
        private static void ShowWindow()
        {
            var window = GetWindow<NetCdfWindowMaker>("Load netCDF data to unity");
            window.minSize = new Vector2(500, 300);
        }

        
        /**
         * Event function for drawing the GUI while the editor window is open.
         */
        void OnGUI()
        {
            _fileSelector.Draw();
            
            if (GUILayout.Button("Get data", GUILayout.Width(400)))
            {
                GetVariables();
            }
            
            GUILayout.Space(10);

            //Prevents variable selection from appearing before the user has selected a dataset.
            if (_ncFiles.Count > 0 && _buildingData != null)
            {
                GUILayout.Label("Select variables to use", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Map name:", GUILayout.Width(75)); 
                    _mapName = GUILayout.TextField(_mapName, GUILayout.Width(325)); 
                EditorGUILayout.EndHorizontal(); 
                
                _buildingData.Draw();
                _heightMap.Draw();
                _windSpeed.Draw();
                _radiationData.Draw();
                
                if (GUILayout.Button("Load variables", GUILayout.Width(400)))
                {
                    CreateDataFiles();
                }
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
            DataGenerator.GenerateVariableJson(_fileSelector.NcFiles, _jsonFolderPath);
            LoadVariables();
            
            _buildingData = new SingleVariableDropdown(_allVariables, "Building data:");
            _heightMap = new SingleVariableDropdown(_allVariables, "Heightmap:");
            _windSpeed = new SingleVariableDropdown(_allVariables, "Windspeed:");
            _radiationData = new MultiVariableDropdown(_allVariables, "Radiation Data:");
        }

        
        /**
         * <summary>
         *  Populates the _allVariables field with JSON data created earlier.
         * </summary>
         */
        private void LoadVariables()
        {
            var path = _jsonFolderPath + "/variables.json";

            string jsonString = File.ReadAllText(path);
            
            //A bit of a roundabout way of doing things but the deserializer can only create a single instance of an object.
            //It doesn't work with several netCDF files without adding the fileDataList key and wrapper.
            jsonString = "{\"fileDataList\":" + jsonString + "}";

            FileDataListWrapper fileDataListWrapper = JsonUtility.FromJson<FileDataListWrapper>(jsonString);
            _ncFiles = fileDataListWrapper.fileDataList;
            
            _allVariables.Clear();

            foreach (var fileData in _ncFiles)
            {
                foreach (var variable in fileData.variables)
                {
                    _allVariables.Add(new NcVariable { filePath = fileData.filePath, variableName = variable });
                }
            }
        }


        /**
         * <summary>
         *  Creates all the necessary data files and folder structures based on the variables selected by the user.
         * </summary>
         */
        private void CreateDataFiles()
        {
            if (_mapName.IsNullOrWhiteSpace())
            {
                Debug.Log("You need to select a map name");
                return;
            }
            
            GenerateBuildingData();
            GenerateHeightMap();
            GenerateWindSpeedData();
            GenerateRadiationData();
        }

        
        /**
         * <summary>
         *  Creates both csv and png files containing data from the selected _buildingData variable.
         * </summary>
         */
        private void GenerateBuildingData()
        {
            var outputPath = $"{Application.dataPath}/Resources/MapData/{_mapName}/BuildingData/buildingData";

            if (_buildingData.SelectedVariable != null)
                DataGenerator.GenerateAllData((NcVariable) _buildingData.SelectedVariable, outputPath);
        }

        
        /**
         * <summary>
         *  Creates both csv and png files containing data from the selected _heightMap variable.
         * </summary>
         */
        private void GenerateHeightMap()
        {
            var outputPath = $"{Application.dataPath}/Resources/MapData/{_mapName}/HeightMap/heightMap";

            if (_heightMap.SelectedVariable != null)
                DataGenerator.GenerateAllData((NcVariable) _heightMap.SelectedVariable, outputPath);
        }

        
        /**
         * <summary>
         *  Creates png files containing data from the selected _windSpeed variable.
         * </summary>
         */
        private void GenerateWindSpeedData()
        {
            var outputPath = $"{Application.dataPath}/Resources/MapData/{_mapName}/WindSpeed";

            if (_windSpeed.SelectedVariable != null)
                DataGenerator.GeneratePng((NcVariable) _windSpeed.SelectedVariable, outputPath, 10);
        }

        
        /**
         * <summary>
         *  Creates png files containing data for every selected _radiationData variable.
         * </summary>
         */
        private void GenerateRadiationData()
        {
            foreach (var variable in _radiationData.SelectedVariables)
            {
                var folderName = RemoveInvalidFilenameChars(variable.variableName);
                var outputPath = $"{Application.dataPath}/Resources/MapData/{_mapName}/Radiation/{folderName}";
                
                DataGenerator.GeneratePng(variable, outputPath, 30);
            }
        }

        
        /**
         * <summary>
         *  Removes every character from a string that cannot be used as a fileName on the current os.
         * </summary>
         *
         * <param name="input">The string to modify</param>
         *
         * <returns> A modified string without invalid characters.</returns>
         */
        private static string RemoveInvalidFilenameChars(string input)
        {
            var invalidChars = new List<char>(Path.GetInvalidFileNameChars());
            var result = new StringBuilder();

            foreach (var c in input)
            {
                if (invalidChars.Contains(c)) continue;
                
                result.Append(c);
            }

            return result.ToString();
        }
    }
}