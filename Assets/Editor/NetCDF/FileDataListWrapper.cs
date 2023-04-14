using System;
using System.Collections.Generic;

namespace Editor.NetCDF
{
    /**
     * Gives the Json deserializer a serializable object to create from Json data.
     * Contains a list of FileData objects.
     */
    [Serializable]
    public struct FileDataListWrapper
    {
        public List<FileData> fileDataList;
    }
}