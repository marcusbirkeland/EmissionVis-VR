using System.Collections.Generic;
using System.Linq;
using Editor.NetCDF;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorWindowComponents
{
    /**
     * A dropdown menu that allows the user to select multiple values.
     */
    public class MultiVariableDropdown : BaseVariableDropdown
    {
        private readonly List<bool> _selectedIndexes;
        private string _selectedVariablesLabel = string.Empty;

        
        /**
         * <summary>
         *  Gets a list of selected NcVariables based on index.
         * </summary>
         */
        public List<NcVariable> SelectedVariables
        {
            get
            {
                return NcVariables.Where((_, index) => _selectedIndexes[index])
                    .ToList();
            }
        }


        /**
         * <summary>
         *  Constructor that inherits from BaseVariableDropdown.
         *  Also initializes the _selectedIndex Array with a length equal to the ncVariable list.
         * </summary>
         * 
         * <param name="ncVariables">A List of ncVariables to populate the dropdown with.</param>
         * <param name="label">The GUI label to indicate what the variable will be used for.</param>
         */
        public MultiVariableDropdown(List<NcVariable> ncVariables, string label) : base(ncVariables, label)
        {
            _selectedIndexes = new List<bool>(new bool[ncVariables.Count]);
            UpdateSelectedVariablesLabel();
        }

        
        /**
         * <summary>
         *  Draws the dropdowns GUI on the EditorWindow. Must be called via an OnGUI event.
         * </summary>
         */
        public void Draw()
        {
            if (NcVariables == null || NcVariables.Count == 0)
            {
                return;
            }

            string[] labels = VariableLabels;

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Label, GUILayout.Width(150));
                if (GUILayout.Button(_selectedVariablesLabel, "Dropdown", GUILayout.Width(250)))
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < labels.Length; i++)
                    {
                        int currentIndex = i;
                        menu.AddItem(new GUIContent(labels[i]), _selectedIndexes[i], () => ToggleSelection(currentIndex));
                    }
                    menu.ShowAsContext();
                }
            EditorGUILayout.EndHorizontal();
        }

        
        /**
         * <summary>
         *  Toggles whether the index is selected.
         * </summary>
         *
         * <param name="index">The index to toggle.</param>
         */
        private void ToggleSelection(int index)
        {
            _selectedIndexes[index] = !_selectedIndexes[index];
            UpdateSelectedVariablesLabel();
        }

        
        /**
         * <summary>
         *  Sets the dropdown menus displayed value to every selected variable separated by comma.
         *  If the length exceeds the GUI field, it instead displays "Multiple..."
         * </summary>
         */
        private void UpdateSelectedVariablesLabel()
        {
            _selectedVariablesLabel = string.Join(", ", SelectedVariables.Select(v => v.variableName));
            if (string.IsNullOrEmpty(_selectedVariablesLabel))
            {
                _selectedVariablesLabel = "Select variables...";
            }
            
            //TODO: Write method that determines if the label is larger than the GUI field instead of hard coding a size limit. 
            else if (_selectedVariablesLabel.Length > 30)
            {
                _selectedVariablesLabel = "Multiple...";
            }
        }
    }
}