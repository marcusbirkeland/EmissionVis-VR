using System;
using System.Collections.Generic;

namespace Editor.NetCDF
{
    /// <summary>
    /// Provides a serializable object for JSON deserialization.
    /// Contains a list of <see cref="FileData"/> objects.
    /// </summary>
    [Serializable]
    public struct FileDataListWrapper
    {
        /// <summary>
        /// A list of <see cref="FileData"/> objects.
        /// </summary>
        public List<FileData> fileDataList;
    }
}