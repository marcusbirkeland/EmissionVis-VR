using System;

namespace Editor.NetCDF.Types
{
    /// <summary>
    /// Represents a single netCDF variable and the file it belongs to.
    /// </summary>
    [Serializable]
    public struct NcVariable
    {
        /// <summary>
        /// The file path of the netCDF file containing the variable.
        /// </summary>
        public string filePath;

        /// <summary>
        /// The name of the netCDF variable.
        /// </summary>
        public string variableName;

        /// <summary>
        /// Overrides the ToString method to return a string in the format expected by the Python scripts.
        /// </summary>
        /// <returns>A string representation of the NcVariable object.</returns>
        public override string ToString()
        {
            return $"{filePath}${variableName}$";
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