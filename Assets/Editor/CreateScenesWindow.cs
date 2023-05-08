using Editor.EditorWindowComponents;
using Editor.NetCDF;
using Editor.NetCDF.Types;
using Editor.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// The CreateScenesWindow class provides an Editor Window to generate Unity scenes based on netCDF data files.
    /// </summary>
    public class CreateScenesWindow : EditorWindow
    {
        private AllVariablesSelector _allVariablesSelector;
        private NcFilesSelector _ncFilesSelector;
        
        
        /// <summary>
        /// Adds an entry to the Unity menu, allowing the user to open the CreateScenesWindow.
        /// </summary>
        [MenuItem("Sintef/Create Scenes")]
        private static void ShowWindow()
        {
            CreateScenesWindow window = GetWindow<CreateScenesWindow>("Generate scenes from netCDF data");
            window.minSize = new Vector2(500, 300);
        }


        /// <summary>
        /// Initializes the window components when the window is enabled.
        /// </summary>
        private void OnEnable()
        {
            _ncFilesSelector = new NcFilesSelector();
        }

        
        /// <summary>
        /// Renders the window's GUI, including the NcFilesSelector and AllVariablesSelector components.
        /// </summary>
        private void OnGUI()
        {
            _ncFilesSelector?.Draw();
            
            if (GUILayout.Button("Get data", GUILayout.Width(400)))
            {
                if (_ncFilesSelector?.NcFiles.Count < 1) return;
                
                EditorUtility.DisplayProgressBar("Getting variables", "Loading CDF files...", -1);
                _allVariablesSelector = new AllVariablesSelector(_ncFilesSelector?.NcFiles, $"{Application.dataPath}/Resources/MapData");
                EditorUtility.ClearProgressBar();
            }
            
            GUILayout.Space(10);

            if (_allVariablesSelector == null) return;
            
            _allVariablesSelector.Draw();

            if (GUILayout.Button("Generate scenes", GUILayout.Width(400)))
            {
                CreateScenes();
            }
        }
        
        /// <summary>
        /// Creates scenes based on the selected variables.
        /// </summary>
        private void CreateScenes()
        {
            if (_allVariablesSelector.SelectedDataset == null)
            {
                _allVariablesSelector.ShowWarning = true;
                return;
            }
            
            if (DataGenerator.CreateDataFiles((NcDataset) _allVariablesSelector.SelectedDataset))
            {
                ScenesBuilder.CreateBothScenes((NcDataset) _allVariablesSelector.SelectedDataset);
            }
        }
    }
}