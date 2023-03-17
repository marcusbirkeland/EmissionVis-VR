using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Editor.NetCDF
{
    /**
     * Base class for the dropdown selection.
     */
    public abstract class BaseVariableDropdown
    {
        protected readonly List<NcVariable> NcVariables;
        protected readonly string Label;


        /**
         * <summary>
         *  Constructor.
         * </summary>
         * 
         * <param name="ncVariables">A List of ncVariables to populate the dropdown with.</param>
         * <param name="label">The GUI label to indicate what the variable will be used for.</param>
         */
        protected BaseVariableDropdown(List<NcVariable> ncVariables, string label)
        {
            NcVariables = ncVariables;
            Label = label;
        }
        
        
        /**
         * <summary>
         *  Gets an array of labels, one for each variable in the dropdown.
         *  If a variable exists in multiple files, the label includes the filename.
         * </summary>
         *
         * <returns>
         *  An array of variable labels.
         * </returns>
         */
        protected string[] VariableLabels =>
            NcVariables.Select(info =>
            {
                var varLabel = info.variableName;
                var count = NcVariables.Count(variableInfo => variableInfo.variableName == info.variableName);

                if (count > 1)
                {
                    varLabel += $" ({Path.GetFileName(info.filePath)})";
                }
                return varLabel;
                
            }).ToArray();
    }
}
