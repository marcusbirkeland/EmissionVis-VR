using System.Collections.Generic;
using Editor.NetCDF;
using Editor.NetCDF.Types;
using Editor.SceneBuilder;
using NUnit.Framework;
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
                CreateAllScenes();
            }
        }
        
        
        /// <summary>
        /// Starts scene creation.
        /// </summary>
        /// <remarks>
        /// If one of the variables arent set, it instead enables the <see cref="AllVariablesSelector"/>s warning display.
        /// </remarks>
        private void CreateAllScenes()
        {
            if (_allVariablesSelector.SelectedDataset == null)
            {
                _allVariablesSelector.ShowWarning = true;
                return;
            }
            
            NcDataset dataset = (NcDataset) _allVariablesSelector.SelectedDataset;

            if (!DataGenerator.CreateDataFiles(dataset)) return;

            List<ISceneBuilder> sceneBuilders = new()
            {
                new MiniatureSceneBuilder(dataset),
                new FullScaleSceneBuilder(dataset)
                //Add more scene building implementations here.
            };
            
            CreateScenesRecursively(sceneBuilders);
        }


        /// <summary>
        /// Recursively creates scenes using the provided list of scene builders.
        /// </summary>
        /// <remarks>
        /// This method takes a list of scene builders and processes them one by one. It removes the current scene builder
        /// from the list and calls its BuildScene method, providing a callback that continues the recursion.
        /// The recursion stops when there are no more scene builders left in the list.
        /// </remarks>
        /// <param name="sceneBuilders">
        /// A list of ISceneBuilder objects that need to have their scenes created. The scene builders should be
        /// ordered in the desired sequence of scene creation.
        /// </param>
        private static void CreateScenesRecursively(IList<ISceneBuilder> sceneBuilders)
        {
            if (sceneBuilders.Count == 0) return;

            ISceneBuilder currentBuilder = sceneBuilders[0];
            sceneBuilders.RemoveAt(0);
            
            currentBuilder.BuildScene(() => CreateScenesRecursively(sceneBuilders));
        }
    }
}