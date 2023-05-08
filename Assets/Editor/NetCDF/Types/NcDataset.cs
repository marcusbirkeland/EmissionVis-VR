using System.Collections.Generic;

namespace Editor.NetCDF.Types
{
    public readonly struct NcDataset
    {
        public readonly string MapName;
        public readonly NcVariable BuildingData;
        public readonly NcVariable HeightMap;
        public readonly NcVariable WindSpeed;
        public readonly List<NcVariable> RadiationData;

        public string BuildingCdfPath => BuildingData.filePath;
        public string WindSpeedCdfPath => WindSpeed.filePath;
        public string RadiationCdfPath => RadiationData.Count > 0 ? RadiationData[0].filePath : string.Empty;

        public NcDataset(
            string mapName,
            NcVariable buildingData,
            NcVariable heightMap,
            NcVariable windSpeed,
            List<NcVariable> radiationData)
        {
            MapName = mapName;
            BuildingData = buildingData;
            HeightMap = heightMap;
            WindSpeed = windSpeed;
            RadiationData = radiationData;
        }
    }
}
