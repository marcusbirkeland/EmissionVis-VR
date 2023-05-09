using System;

namespace Editor.NetCDF.Types
{
    /// <summary>
    /// Represents a single netCDF variable and the file it belongs to.
    /// </summary>
    [Serializable]
    public readonly struct NcVariable
    {
        /// <summary>
        /// The file path of the netCDF file containing the variable.
        /// </summary>
        public readonly string FilePath;

        /// <summary>
        /// The name of the netCDF variable.
        /// </summary>
        public readonly string VariableName;

        
        public NcVariable(string variableName, string filePath)
        {
            VariableName = variableName;
            FilePath = filePath;
        }

        
        /// <summary>
        /// Overrides the ToString method to return a string in the format expected by the Python scripts.
        /// </summary>
        /// <returns>A string representation of the NcVariable object.</returns>
        public override string ToString()
        {
            return $"{FilePath}${VariableName}$";
        }

        
        /// <summary>
        /// Allows implicit conversion from NcVariable to string.
        /// </summary>
        /// <param name="ncVariable">The NcVariable instance to convert.</param>
        public static implicit operator string(NcVariable ncVariable)
        {
            return ncVariable.ToString();
        }
    }
}