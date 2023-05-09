using System.Collections.Generic;
using System.Linq;
using Editor.NetCDF.Types;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorWindowComponents
{
    /// <summary>
    /// A dropdown menu that allows the user to select multiple values. Extends the <see cref="BaseVariableDropdown"/>
    /// </summary>
    public class MultiVariableDropdown : BaseVariableDropdown
    {
        /// <summary>
        /// Gets a list of the currently selected NcVariables.
        /// </summary>
        public List<NcVariable> SelectedVariables => NcVariables.Where((_, index) => _selectedIndexes[index]).ToList();

        /// Label for displaying what variables are currently selected. Not to be confused with the Label property.
        private string _selectedVariablesLabel = string.Empty;
        private readonly List<bool> _selectedIndexes;
        
        
        /// <summary>
        /// Constructor that inherits from <see cref="BaseVariableDropdown"/>'s constructor.
        /// </summary>
        /// <param name="ncVariables">A List of ncVariables to populate the dropdown with.</param>
        /// <param name="label">The GUI label to indicate what the variable will be used for.</param>
        public MultiVariableDropdown(List<NcVariable> ncVariables, string label) : base(ncVariables, label)
        {
            _selectedIndexes = new List<bool>(new bool[ncVariables.Count]);
            UpdateSelectedVariablesLabel();
        }

        
        /// <summary>
        /// Draws the dropdowns GUI on the EditorWindow.
        /// </summary>
        /// <remarks>
        /// Must be used inside an EditorWindow to function.
        /// </remarks>
        public void Draw()
        {
            if (NcVariables == null || NcVariables.Count == 0)
            {
                return;
            }

            string[] labels = VariableLabels;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(Label, GUILayout.Width(150));
                if (GUILayout.Button(_selectedVariablesLabel, "Dropdown", GUILayout.Width(250)))
                {
                    GenericMenu menu = new();
                    for (int i = 0; i < labels.Length; i++)
                    {
                        int currentIndex = i;
                        menu.AddItem(new GUIContent(labels[i]), _selectedIndexes[i],
                            () => ToggleSelectionAtIndex(currentIndex));
                    }

                    menu.ShowAsContext();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        
        
        private void ToggleSelectionAtIndex(int index)
        {
            _selectedIndexes[index] = !_selectedIndexes[index];
            UpdateSelectedVariablesLabel();
        }

        
        /// <summary>
        /// Sets the dropdown menu's displayed value to every selected variable separated by a comma.
        /// If the length exceeds the GUI field, it instead displays "Multiple..."
        /// </summary>
        ///
        /// <remarks>
        /// TODO: replace max label length with dynamic value based on display size.
        /// </remarks>
        private void UpdateSelectedVariablesLabel()
        {
            _selectedVariablesLabel = string.Join(", ", SelectedVariables.Select(v => v.VariableName));
            if (string.IsNullOrEmpty(_selectedVariablesLabel))
            {
                _selectedVariablesLabel = "Select variables...";
            }
            else if (_selectedVariablesLabel.Length > 30) 
            {
                _selectedVariablesLabel = "Multiple...";
            }
        }
    }
}