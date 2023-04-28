using System.Collections.Generic;
using System.Linq;
using Editor.NetCDF;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorWindowComponents
{
    /// <summary>
    /// A dropdown menu that only allows the user to select a single value.
    /// </summary>
    public class SingleVariableDropdown : BaseVariableDropdown
    {
        private int _selectedIndex;

        
        /// <summary>
        /// Gets the currently selected ncVariable if it exists.
        /// If no variable is selected, it returns null.
        /// </summary>
        public NcVariable? SelectedVariable => _selectedIndex > 0 ? NcVariables[_selectedIndex - 1] : null;

        
        /// <summary>
        /// Constructor. Inherited from BaseVariableDropdown.
        /// </summary>
        /// <param name="ncVariables">A List of ncVariables to populate the dropdown with.</param>
        /// <param name="label">The GUI label to indicate what the variable will be used for.</param>
        public SingleVariableDropdown(List<NcVariable> ncVariables, string label) : base(ncVariables, label) { }

        
        /// <summary>
        /// Draws the dropdowns GUI on the EditorWindow. Must be called via an OnGUI event.
        /// </summary>
        public void Draw()
        {
            if (NcVariables == null || NcVariables.Count == 0) return;

            string[] varLabels = new[] { "None" }.Concat(VariableLabels).ToArray();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Label, GUILayout.Width(150));
                _selectedIndex = EditorGUILayout.Popup(_selectedIndex, varLabels, GUILayout.Width(250));
            EditorGUILayout.EndHorizontal();
        }
    }
}