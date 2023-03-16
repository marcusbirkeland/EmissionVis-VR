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
        
        public abstract void Draw();
        
        
        protected BaseVariableDropdown(List<NcVariable> ncVariables, string label)
        {
            NcVariables = ncVariables;
            Label = label;
        }
        
        
        /**
         * Creates a list of all available variables.
         * If a variable exists in several files, it also includes the filename. 
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
