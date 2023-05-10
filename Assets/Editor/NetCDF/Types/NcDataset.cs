using System.Collections.Generic;

namespace Editor.NetCDF.Types
{
    /// <summary>
    /// A struct representing a NetCDF dataset, containing data for all the different variables used in map generation.
    /// </summary>
    public readonly struct NcDataset
    {
        /// <summary>
        /// The name of the map as chosen by the user. Will be used as the folder name.
        /// </summary>
        public readonly string MapName;

        /// <summary>
        /// The NetCDF variable containing building data.
        /// </summary>
        public readonly NcVariable BuildingData;

        /// <summary>
        /// The NetCDF variable containing height map data.
        /// </summary>
        public readonly NcVariable HeightMap;

        /// <summary>
        /// The NetCDF variable containing wind speed data.
        /// </summary>
        public readonly NcVariable WindSpeed;

        /// <summary>
        /// A list of NetCDF variables containing radiation data.
        /// </summary>
        public readonly List<NcVariable> RadiationData;

        /// <summary>
        /// The file path of the NetCDF file containing building data.
        /// </summary>
        public string BuildingCdfPath => BuildingData.FilePath;

        /// <summary>
        /// The file path of the NetCDF file containing wind speed data.
        /// </summary>
        public string WindSpeedCdfPath => WindSpeed.FilePath;

        /// <summary>
        /// The file path of the first NetCDF file containing radiation data, or an empty string if no radiation data is available.
        /// </summary>
        /// <remarks>
        /// Should be replaced when support for multiple radiation variables is added.
        /// </remarks>
        public string RadiationCdfPath => RadiationData.Count > 0 ? RadiationData[0].FilePath : string.Empty;

        
        /// <summary>
        /// Initializes a new instance of the <see cref="NcDataset"/> struct with the specified map name and data variables.
        /// </summary>
        /// <param name="mapName">The name of the map for which this dataset contains data.</param>
        /// <param name="buildingData">The NetCDF variable containing building data.</param>
        /// <param name="heightMap">The NetCDF variable containing height map data.</param>
        /// <param name="windSpeed">The NetCDF variable containing wind speed data.</param>
        /// <param name="radiationData">A list of NetCDF variables containing radiation data.</param>
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
