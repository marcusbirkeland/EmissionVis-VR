using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.NetCDF
{
    /**
     * A dropdown menu that only allows the user to select a single value.
     */
    public class SingleVariableDropdown : BaseVariableDropdown
    {
        private int _selectedIndex;

        public NcVariable? SelectedVariable => _selectedIndex > 0 ? NcVariables[_selectedIndex - 1] : null;

        public SingleVariableDropdown(List<NcVariable> ncVariables, string label) : base(ncVariables, label) { }

        public override void Draw()
        {
            if (NcVariables == null || NcVariables.Count == 0) return;

            var varLabels = new[] { "None" }.Concat(VariableLabels).ToArray();
            
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Label, GUILayout.Width(150)); 
                _selectedIndex = EditorGUILayout.Popup(_selectedIndex, varLabels, GUILayout.Width(250)); 
            EditorGUILayout.EndHorizontal();
        }
    }
}