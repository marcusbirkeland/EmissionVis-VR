using System;

namespace Editor.NetCDF
{
    /**
     * A single netCDF variable and the file it belongs to
     */
    [Serializable]
    public struct NcVariable
    {
        public string filePath;
        public string variableName;

        /**
         * Changes the ToString method to return a string on the format expected by the python scripts.
         */
        public override string ToString()
        {
            return $"{filePath}${variableName}$";
        }
    }
}