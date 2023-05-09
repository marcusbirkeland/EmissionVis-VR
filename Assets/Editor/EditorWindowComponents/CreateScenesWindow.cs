using Editor.NetCDF;
using Editor.NetCDF.Types;
using Editor.SceneBuilder;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorWindowComponents
{
    /// <summary>
    /// Main class for displaying the editor window responsible for scene generation.
    /// </summary>
    public class CreateScenesWindow : EditorWindow
    {
        private AllVariablesSelector _allVariablesSelector;
        private NcFilesSelector _ncFilesSelector;
        
        
        /// <summary>
        /// Adds an entry to the Unity menu, allowing the user to open the <see cref="CreateScenesWindow"/>.
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
        /// Renders the window's GUI, including the <see cref="NcFilesSelector"/> and <see cref="AllVariablesSelector"/> components.
        /// </summary>
        private void OnGUI()
        {
            _ncFilesSelector?.Draw();
            
            if (GUILayout.Button("Get data", GUILayout.Width(400)))
            {
                if (_ncFilesSelector?.NcFiles.Count < 1) return;
                
                EditorUtility.DisplayProgressBar("Getting variables", "Loading CDF files...", -1);
                _allVariablesSelector = new AllVariablesSelector(_ncFilesSelector?.NcFiles);
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
        /// Starts scene creation.
        /// </summary>
        /// <remarks>
        /// If one of the variables arent set, it instead enables the <see cref="AllVariablesSelector"/>s warning display.
        /// </remarks>
        private void CreateScenes()
        {
            if (_allVariablesSelector.SelectedDataset == null)
            {
                _allVariablesSelector.ShowWarning = true;
                return;
            }
            
            NcDataset dataset = (NcDataset) _allVariablesSelector.SelectedDataset;

            if (!DataGenerator.CreateDataFiles(dataset)) return;

            MiniatureSceneBuilder.CreateScene(dataset, () =>
            {
                FullScaleSceneBuilder.CreateScene(dataset);
            });
        }
    }
}