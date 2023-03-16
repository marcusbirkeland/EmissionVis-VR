using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.NetCDF
{
    /**
     * A dropdown menu that allows the user to select multiple values.
     */
    public class MultiVariableDropdown : BaseVariableDropdown
    {
        private readonly List<bool> _selectedIndexes;
        private string _selectedVariablesLabel = string.Empty;

        public List<NcVariable> SelectedVariables
        {
            get
            {
                return NcVariables
                    .Where((_, index) => _selectedIndexes[index])
                    .ToList();
            }
        }

        public MultiVariableDropdown(List<NcVariable> ncVariables, string label) : base(ncVariables, label)
        {
            _selectedIndexes = new List<bool>(new bool[ncVariables.Count]);
            UpdateSelectedVariablesLabel();
        }

        public override void Draw()
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

        private void ToggleSelection(int index)
        {
            _selectedIndexes[index] = !_selectedIndexes[index];
            UpdateSelectedVariablesLabel();
        }

        private void UpdateSelectedVariablesLabel()
        {
            _selectedVariablesLabel = string.Join(", ", SelectedVariables.Select(v => v.variableName));
            if (string.IsNullOrEmpty(_selectedVariablesLabel))
            {
                _selectedVariablesLabel = "Select variables...";
            }
            
            //TODO: Write method that determines if the label is larger than the GUI field instead of hard coding it. 
            else if (_selectedVariablesLabel.Length > 30)
            {
                _selectedVariablesLabel = "Multiple...";
            }
        }
    }
}