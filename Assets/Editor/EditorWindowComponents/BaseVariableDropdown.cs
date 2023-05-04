using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor.NetCDF.Types;

namespace Editor.EditorWindowComponents
{
    /// <summary>
    /// Base class for the dropdown selection.
    /// </summary>
    public abstract class BaseVariableDropdown
    {
        protected readonly List<NcVariable> NcVariables;
        protected readonly string Label;

        
        /// <summary>
        /// Base constructor for the variable dropdowns.
        /// </summary>
        /// <param name="ncVariables">A List of ncVariables to populate the dropdown with.</param>
        /// <param name="label">The GUI label to indicate what the variable will be used for.</param>
        protected BaseVariableDropdown(List<NcVariable> ncVariables, string label)
        {
            NcVariables = ncVariables;
            Label = label;
        }

        
        /// <summary>
        /// Gets an array of labels, one for each variable in the dropdown.
        /// If a variable exists in multiple files, the label includes the filename.
        /// </summary>
        /// <returns>
        /// An array of variable labels.
        /// </returns>
        protected string[] VariableLabels => NcVariables.Select(info =>
        {
            string variableName = info.variableName;
            int count = NcVariables.Count(variableInfo => variableInfo.variableName == info.variableName);

            return count > 1 ? $"{variableName} ({Path.GetFileName(info.filePath)})" : variableName;

        }).ToArray();
    }
}